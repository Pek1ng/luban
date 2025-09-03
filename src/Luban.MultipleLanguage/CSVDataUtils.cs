using ExcelDataReader;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace Luban.MultipleLanguage;

internal static partial class CSVDataUtil
{
    private static DataSet _transData;

    public static DataSet TransData
    {
        get
        {
            _transData ??= LoadCSVData();
            return _transData;
        }
    }

    private static Dictionary<string, Dictionary<string, string>> _textToKey;

    public static IReadOnlyDictionary<string, Dictionary<string, string>> TextToKey
    {
        get
        {
            if (_textToKey == null)
            {
                if (!EnvManager.Current.TryGetOption(LocalizationOptionNames.LocalizationFamily, LocalizationOptionNames.DefaultLang, false, out string defaultLang))
                {
                    throw new NotSupportedException();
                }

                _textToKey = [];
                string langKeyLine = $"{CommonString.TableValue}@{defaultLang}";

                foreach (DataTable dataTable in TransData.Tables)
                {
                    Dictionary<string, string> tableDic = [];
                    _textToKey.TryAdd(dataTable.TableName, tableDic);

                    // 除了表头的第一行是定义
                    for (int i = 1; i < dataTable.Rows.Count; i++)
                    {
                        var row = dataTable.Rows[i];
                        string key = (string)row[CommonString.TableKey];
                        string text = (string)row[langKeyLine];
                        tableDic.TryAdd(text, key);
                    }
                }
            }
            return _textToKey;
        }
    }


    public static bool TryGetKey(string table, string text, out string key)
    {
        if (TextToKey.TryGetValue(table, out var tableDic))
        {
            if (tableDic.TryGetValue(text, out key))
            {
                return true;
            }
        }

        key = string.Empty;
        return false;
    }


    public static DataSet LoadCSVData()
    {
        DataSet dataSet = new();

        if (!EnvManager.Current.TryGetOption(LocalizationOptionNames.CSVDataTarget, BuiltinOptionNames.OutputDataDir, false, out string outputPath))
        {
            return dataSet;
        }

        foreach (var path in Directory.GetFiles(outputPath, "*.csv"))
        {
            DataTable dt = new()
            {
                TableName = Path.GetFileNameWithoutExtension(path)
            };

            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

            try
            {
                using var reader = ExcelReaderFactory.CreateCsvReader(stream, new ExcelReaderConfiguration
                {
                    FallbackEncoding = GetFileEncodeType(path),
                });

                int colNums = 0;
                do
                {
                    while (reader.Read())
                    {
                        if (colNums == 0)
                        {
                            colNums = reader.FieldCount;
                            for (int i = 0; i < colNums; i++)
                            {
                                string name = reader.GetString(i);
                                if (i > 4)
                                {
                                    name = $"{name}_{i - 4}";
                                }
                                dt.Columns.Add(name);
                            }
                            dt.PrimaryKey = [dt.Columns[CommonString.TableKey]];
                        }
                        else
                        {
                            DataRow row = dt.NewRow();
                            for (int i = 0; i < colNums; i++)
                            {
                                row[i] = reader.GetString(i);
                            }
                            dt.Rows.Add(row);
                        }
                    }
                } while (reader.NextResult());

                dataSet.Tables.Add(dt);
            }
            catch
            {
                NLog.LogManager.GetCurrentClassLogger().Error($"文件{path}可能编码格式不对，请以Utf-8 保存");
                throw;
            }
        }

        return dataSet;
    }

    public static string ConvertDataTableToCSV(DataTable dt)
    {
        StringBuilder sb = new();

        foreach (DataColumn column in dt.Columns)
        {
            string columnName = CustopmRegex().Replace(column.ColumnName, "");
            sb.Append(columnName + ",");
        }
        sb.Remove(sb.Length - 1, 1); // 移除最后一个逗号
        sb.AppendLine();

        foreach (DataRow row in dt.Rows)
        {
            foreach (var item in row.ItemArray)
            {
                sb.Append(item.ToString() + ",");
            }
            sb.Remove(sb.Length - 1, 1); // 移除最后一个逗号
            sb.AppendLine();
        }

        return sb.ToString();
    }

    public static Encoding GetFileEncodeType(string filename)
    {
        using FileStream fs = new(filename, FileMode.Open, FileAccess.Read);
        BinaryReader br = new(fs);
        byte[] buffer = br.ReadBytes(2);

        if (buffer[0] >= 0xEF)
        {
            if (buffer[0] == 0xEF && buffer[1] == 0xBB)
            {
                return Encoding.UTF8;
            }
            else if (buffer[0] == 0xFE && buffer[1] == 0xFF)
            {
                return Encoding.BigEndianUnicode;
            }
            else if (buffer[0] == 0xFF && buffer[1] == 0xFE)
            {
                return Encoding.Unicode;
            }
        }

        return Encoding.GetEncoding("GB2312");
    }

    [GeneratedRegex(@"_\d+$")]
    private static partial Regex CustopmRegex();
}
