namespace Luban.MultipleLanguage.DataTarget;

public class LanguageTextCollection(string prefix)
{
    public readonly string Prefix = prefix;

    private readonly SortedSet<string> _keys = [];

    public ICollection<string> Keys
    {
        get { return _keys; }
    }

    public void AddKey(string key)
    {
        
        if (!string.IsNullOrWhiteSpace(key))
        {
            _keys.Add(key);
        }
    }
}
