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

using Luban.DataTarget;
using Luban.Defs;
using System.Text;

namespace Luban.L10N.TextToKey;

[DataTarget("text-map")]
internal class TextKeyListDataTarget : DataTargetBase
{
    protected override string DefaultOutputFileExt => "csv";

    public override OutputFile ExportTable(DefTable table, List<Record> records)
    {
        if (GenerationContext.Current.TextProvider is not TextToKeyTextProvider keyTextProvider)
        {
            throw new Exception($"'-x {BuiltinOptionNames.L10NFamily}.{BuiltinOptionNames.L10NProviderName} must set textToKey'");
        }

        EnvManager env = EnvManager.Current;
        string keyFieldName = env.GetOptionOrDefault(BuiltinOptionNames.L10NFamily, BuiltinOptionNames.L10NTextFileKeyFieldName, false, "Key");
        string languageFieldName = env.GetOptionOrDefault(BuiltinOptionNames.L10NFamily, BuiltinOptionNames.L10NTextFileLanguageFieldName, false, "Chinese (Simplified)(zh-Hans)");

        StringBuilder stringBuilder = new();

        if (!keyTextProvider.TextToKeyMap.TryGetValue(table.FullName, out var map) || map.Count == 0)
        {
            return null;
        }

        var array = new KeyValuePair<string, string>[map.Count];
        int index = 0;
        foreach (var kvp in map)
        {
            array[index++] = kvp;
        }
        Array.Sort(array, static (a, b) => a.Key.CompareTo(b.Key));

        stringBuilder.Append(keyFieldName);
        stringBuilder.Append(',');
        stringBuilder.Append(languageFieldName);
        stringBuilder.AppendLine();

        foreach (var item in array)
        {
            stringBuilder.Append(item.Key);
            stringBuilder.Append(',');
            stringBuilder.Append('"');
            stringBuilder.Append(item.Value.Replace("\"", "\"\""));
            stringBuilder.Append('"');
            stringBuilder.AppendLine();
        }

        return CreateOutputFile($"{table.FullName}.csv", stringBuilder.ToString());
    }
}
