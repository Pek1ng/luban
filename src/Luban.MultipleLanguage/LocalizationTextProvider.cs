using Luban.Datas;
using Luban.L10N;
using Luban.Utils;
using System.Diagnostics;

namespace Luban.MultipleLanguage;

[TextProvider(LocalizationOptionNames.LocalizationFamily)]
public class LocalizationTextProvider : ITextProvider
{
    public bool ConvertTextKeyToValue => _convertTextKeyToValue;

    private bool _convertTextKeyToValue;

    public void AddUnknownKey(string key)
    {
        Debugger.Launch();
        throw new NotImplementedException();
    }

    public bool IsValidKey(string key)
    {
        throw new NotImplementedException();
    }

    public void Load()
    {
        EnvManager env = EnvManager.Current;
        _convertTextKeyToValue = DataUtil.ParseBool(env.GetOptionOrDefault(LocalizationOptionNames.LocalizationFamily, LocalizationOptionNames.ConvertTextToKey, false, "false"));
    }

    public void ProcessDatas()
    {
        if (ConvertTextKeyToValue)
        {
            var trans = new TextToKeyTransformer(this);
            foreach (var table in GenerationContext.Current.Tables)
            {
                trans.SetTableName(table.OutputDataFile);
                foreach (var record in GenerationContext.Current.GetTableAllDataList(table))
                {
                    record.Data = (DBean)record.Data.Apply(trans, table.ValueTType);
                }
            }
        }
    }

    public bool TryGetText(string key, out string text)
    {
        throw new NotImplementedException();
    }
}
