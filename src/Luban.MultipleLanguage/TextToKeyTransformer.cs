using Luban.Datas;
using Luban.DataTransformer;
using Luban.DataVisitors;
using Luban.Types;

namespace Luban.MultipleLanguage;

public class TextToKeyTransformer : DataTransfomerBase, IDataFuncVisitor2<DType>
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    private readonly LocalizationTextProvider _provider;

    private string _tableName;

    public TextToKeyTransformer(LocalizationTextProvider provider)
    {
        _provider = provider;
    }

    public void SetTableName(string tableName)
    {
        _tableName = tableName;
    }

    DType IDataFuncVisitor2<DType>.Accept(DString data, TType type)
    {
        if (string.IsNullOrEmpty(data.Value) || !type.HasTag("text"))
        {
            return data;
        }

        if (CSVDataUtil.TryGetKey(_tableName, data.Value, out var text))
        {
            return DString.ValueOf(type, text);
        }
        s_logger.Error("can't find target language text of text id:{} ", data.Value);

        return data;
    }
}
