using Luban.Datas;
using Luban.DataVisitors;
using Luban.Types;

namespace Luban.MultipleLanguage.DataTarget;


/// <summary>
/// 检查 相同key的text,原始值必须相同
/// </summary>
public class LanguageTextCollectorVisitor : IDataActionVisitor2<LanguageTextCollection>
{
    public static LanguageTextCollectorVisitor Ins { get; } = new LanguageTextCollectorVisitor();

    public void Accept(DBool data, TType type, LanguageTextCollection x)
    {

    }

    public void Accept(DByte data, TType type, LanguageTextCollection x)
    {

    }

    public void Accept(DShort data, TType type, LanguageTextCollection x)
    {

    }

    public void Accept(DInt data, TType type, LanguageTextCollection x)
    {

    }

    public void Accept(DLong data, TType type, LanguageTextCollection x)
    {

    }

    public void Accept(DFloat data, TType type, LanguageTextCollection x)
    {

    }

    public void Accept(DDouble data, TType type, LanguageTextCollection x)
    {

    }

    public void Accept(DEnum data, TType type, LanguageTextCollection x)
    {

    }

    public void Accept(DString data, TType type, LanguageTextCollection x)
    {
        if (data != null && type.HasTag("text"))
        {
            x.AddKey(data.Value);
        }
    }

    public void Accept(DDateTime data, TType type, LanguageTextCollection x)
    {

    }

    public void Accept(DBean data, TType type, LanguageTextCollection x)
    {

    }

    public void Accept(DArray data, TType type, LanguageTextCollection x)
    {

    }

    public void Accept(DList data, TType type, LanguageTextCollection x)
    {

    }

    public void Accept(DSet data, TType type, LanguageTextCollection x)
    {

    }

    public void Accept(DMap data, TType type, LanguageTextCollection x)
    {

    }
}
