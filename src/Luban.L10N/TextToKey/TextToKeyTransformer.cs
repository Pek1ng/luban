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
using Luban.DataVisitors;
using Luban.Types;

namespace Luban.L10N.TextToKey;

public class TextKey
{
    public string TableName;

    public string IndexValue;

    public string FieldName;

    public string CollectionKey;

    public Dictionary<string, string> CurrentTableMap;

    public override string ToString()
    {
        return $"{TableName}_{FieldName}_{IndexValue}".ToLower();
    }
}

public class TextToKeyTransformer : TextToKeyDataTransfomer, IDataFuncVisitor2<TextKey, DType>
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    public TextToKeyTransformer() { }

    DType IDataFuncVisitor2<TextKey, DType>.Accept(DString data, TType type, TextKey textKey)
    {
        if (string.IsNullOrEmpty(data.Value) || !type.HasTag("text"))
        {
            return data;
        }

        var map = textKey.CurrentTableMap;
        //检查表级别的文本是否有重复,有重复就拿生成过的多语言key作为当前的Key
        if (!map.TryGetValue(data.Value, out string key))
        {
            key = textKey.ToString();
            map.Add(data.Value, key);
        }

        return DString.ValueOf(type, key);
    }
}
