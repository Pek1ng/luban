// Copyright 2025 Code Philosophy
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using Luban.Datas;

namespace Luban.L10N.TextToKey;

[TextProvider("textToKey")]
public class TextToKeyTextProvider : ITextProvider
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    public readonly Dictionary<string, Dictionary<string, string>> TextToKeyMap = new();

    public void Load() { }

    public bool ConvertTextKeyToValue => true;

    public bool IsValidKey(string key)
    {
        return false;
    }

    public bool TryGetText(string key, out string text)
    {
        text = null;
        return false;
    }

    public void AddUnknownKey(string key) { }

    public void ProcessDatas()
    {
        var textToKeyTransformer = new TextToKeyTransformer();
        TextKey textKey = new();

        foreach (var table in GenerationContext.Current.Tables)
        {
            if (table.IndexList.Count == 1)
            {
                textKey.CurrentTableMap = new();
                TextToKeyMap.Add(table.FullName, textKey.CurrentTableMap);

                int index = table.IndexList[0].IndexFieldIdIndex;
                foreach (var record in GenerationContext.Current.GetTableAllDataList(table))
                {
                    DType dType = record.Data.Fields[index];

                    string value;
                    if (dType is DType<string> stringValue)
                    {
                        value = stringValue.Value;
                    }
                    else
                    {
                        value = dType.ToString();
                    }

                    string flag = "Table";
                    textKey.TableName = table.Name.Replace(flag, string.Empty);

                    textKey.IndexValue = value;

                    record.Data = (DBean)record.Data.Apply(textToKeyTransformer, table.ValueTType, textKey);
                }
            }
        }
    }
}
