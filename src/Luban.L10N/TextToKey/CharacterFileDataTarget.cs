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
using Luban.DataVisitors;
using Luban.Defs;
using Luban.L10N.DataTarget;
using System.Text;

namespace Luban.L10N.TextToKey;

[DataTarget("character-file")]
internal class CharacterFileDataTarget : DataTargetBase
{
    protected override string DefaultOutputFileExt => "txt";

    public override bool ExportAllRecords => true;

    public override AggregationType AggregationType => AggregationType.Tables;

    public override OutputFile ExportTable(DefTable table, List<Record> records)
    {
        throw new NotImplementedException();
    }

    public override OutputFile ExportTables(List<DefTable> tables)
    {

        HashSet<char> charSet = [];

        if (GenerationContext.Current.TextProvider is TextToKeyTextProvider keyTextProvider)
        {
            foreach (var item in keyTextProvider.TextToKeyMap)
            {
                foreach (var item1 in item.Value)
                {
                    foreach (var item2 in item1.Key)
                    {
                        charSet.Add(item2);
                    }
                }
            }
        }
        else
        {
            var textCollection = new TextKeyCollection();

            var visitor = new DataActionHelpVisitor2<TextKeyCollection>(TextKeyListCollectorVisitor.Ins);

            foreach (var table in tables)
            {
                TableVisitor.Ins.Visit(table, visitor, textCollection);
            }

            var keys = textCollection.Keys;

            foreach (var item in keys)
            {
                foreach (var c in item)
                {
                    charSet.Add(c);
                }
            }
        }

        StringBuilder sb = new(charSet.Count);
        foreach (char c in charSet)
        {
            sb.Append(c);
        }
        string content = sb.ToString();
        return CreateOutputFile("CharacterFile.txt", content);
    }
}
