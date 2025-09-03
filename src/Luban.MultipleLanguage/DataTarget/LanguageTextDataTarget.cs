using Luban.DataTarget;
using Luban.DataVisitors;
using Luban.Defs;
using NLog;
using System.Data;
using System.Xml;

namespace Luban.MultipleLanguage.DataTarget;

[DataTarget(LocalizationOptionNames.CSVDataTarget)]
public class LanguageTextDataTarget : DataTargetBase, IDataTarget
{
    protected override string DefaultOutputFileExt => "csv";

    private bool _hasDefaultLangKey = false;

    private string _defaultLangKey = string.Empty;

    private List<string> _files = new();

    void IDataTarget.ProcessDataTargetBegin()
    {
        _hasDefaultLangKey = EnvManager.Current.TryGetOption(LocalizationOptionNames.LocalizationFamily, LocalizationOptionNames.DefaultLang, false, out _defaultLangKey);

        DataSet ds = CSVDataUtil.TransData;
        LogManager.GetCurrentClassLogger().Info("初始表数量:" + ds.Tables.Count);
    }

    void IDataTarget.ProcessDataTargetEnd()
    {
        if (!EnvManager.Current.TryGetOption(LocalizationOptionNames.CSVDataTarget, BuiltinOptionNames.OutputDataDir, false, out string outputPath))
        {
            return;
        }

        XmlDocument doc = new XmlDocument();

        XmlElement root = doc.CreateElement("module");
        doc.AppendChild(root);


        var child = doc.CreateElement("table");
        root.AppendChild(child);

        XmlAttribute nameAttr = doc.CreateAttribute("name");
        nameAttr.Value = "TranslationTable";
        child.Attributes.Append(nameAttr);

        XmlAttribute valueAttr = doc.CreateAttribute("value");
        valueAttr.Value = "TranslationDef";
        child.Attributes.Append(valueAttr);

        XmlAttribute readSchemaFromFileAttr = doc.CreateAttribute("readSchemaFromFile");
        readSchemaFromFileAttr.Value = "1";
        child.Attributes.Append(readSchemaFromFileAttr);

        List<string> inputs = [];
        foreach (var item in _files)
        {
            inputs.Add($"{item}.{OutputFileExt}");
        }

        XmlAttribute inputAttr = doc.CreateAttribute("input");
        inputAttr.Value = string.Join(",", inputs);
        child.Attributes.Append(inputAttr);


        XmlElement enumElement = doc.CreateElement("enum");
        root.AppendChild(enumElement);

        XmlAttribute enumNameAttr = doc.CreateAttribute("name");
        enumNameAttr.Value = "LangKey";
        enumElement.Attributes.Append(enumNameAttr);

        foreach (var item in CSVDataUtil.TextToKey)
        {
            foreach (var pair in item.Value)
            {
                XmlElement varElement = doc.CreateElement("var");
                enumElement.AppendChild(varElement);

                XmlAttribute varNameAttr = doc.CreateAttribute("name");
                varNameAttr.Value = pair.Value;
                varElement.Attributes.Append(varNameAttr);
            }
        }

        doc.Save(Path.Combine(outputPath, "TableDef.xml"));
    }

    public override OutputFile ExportTable(DefTable table, List<Record> records)
    {
        if (!_hasDefaultLangKey)
        {
            return null;
        }

        var visitor = new DataActionHelpVisitor2<LanguageTextCollection>(LanguageTextCollectorVisitor.Ins);
        LanguageTextCollection textKeyCollection = new(table.Name);
        TableVisitor.Ins.Visit(table, visitor, textKeyCollection);

        if (textKeyCollection.Keys.Count > 0)
        {
            string fileName = table.OutputDataFile;

            DataSet ds = CSVDataUtil.TransData;
            DataTable dt;

            string langKeyLine = $"{CommonString.TableValue}@{_defaultLangKey}";

            lock (ds.Tables.SyncRoot)
            {
                if (ds.Tables.Contains(fileName))
                {
                    dt = ds.Tables[fileName];
                }
                else
                {
                    dt = new DataTable
                    {
                        TableName = fileName
                    };
                    ds.Tables.Add(dt);

                    dt.Columns.Add(CommonString.TableDefKey);
                    dt.Columns.Add(CommonString.TableKey);
                    dt.Columns.Add(CommonString.TableValue);
                    dt.Columns.Add(CommonString.ChangedFlagKey);

                    dt.Columns.Add(langKeyLine);

                    var row = dt.NewRow();
                    row[CommonString.TableDefKey] = "##type";
                    row[CommonString.TableKey] = "LangKey";
                    row[CommonString.TableValue] = "string";
                    dt.Rows.Add(row);

                    dt.PrimaryKey = [dt.Columns[CommonString.TableKey]];
                }
            }

            var keyArray = textKeyCollection.Keys.ToArray();
            for (int i = 0; i < keyArray.Length; i++)
            {
                string liKey = $"{textKeyCollection.Prefix}{i}";
                string liVal = keyArray[i];

                lock (dt.Rows.SyncRoot)
                {
                    var row = dt.Rows.Find(liKey);
                    if (row == null)
                    {
                        row = dt.NewRow();
                        row[CommonString.TableKey] = liKey;
                        dt.Rows.Add(row);
                    }

                    var oldChangeFlag = row[CommonString.ChangedFlagKey];
                    string oldValue = row[langKeyLine] == DBNull.Value ? string.Empty : (string)row[langKeyLine];
                    if (oldChangeFlag == DBNull.Value ||
                        oldChangeFlag is string str && str != true.ToString().ToUpper() && oldValue != liVal)
                    {
                        row[CommonString.ChangedFlagKey] = true.ToString().ToUpper();
                    }
                    row[langKeyLine] = liVal;
                }
            }

            _files.Add(fileName);

            string path = $"{fileName}.{OutputFileExt}";
            return CreateOutputFile(path, CSVDataUtil.ConvertDataTableToCSV(dt));
        }
        else
        {
            return null;
        }
    }
}
