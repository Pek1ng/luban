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

public abstract class TextToKeyDataTransfomer : IDataFuncVisitor2<TextKey, DType>
{
    public DType Accept(DBool data, TType type, TextKey t)
    {
        return data;
    }

    public DType Accept(DByte data, TType type, TextKey t)
    {
        return data;
    }

    public DType Accept(DShort data, TType type, TextKey t)
    {
        return data;
    }

    public DType Accept(DInt data, TType type, TextKey t)
    {
        return data;
    }

    public DType Accept(DLong data, TType type, TextKey t)
    {
        return data;
    }

    public DType Accept(DFloat data, TType type, TextKey t)
    {
        return data;
    }

    public DType Accept(DDouble data, TType type, TextKey t)
    {
        return data;
    }

    public DType Accept(DEnum data, TType type, TextKey t)
    {
        return data;
    }

    public DType Accept(DString data, TType type, TextKey t)
    {
        return data;
    }

    public DType Accept(DDateTime data, TType type, TextKey t)
    {
        return data;
    }

    public DType Accept(DBean data, TType type, TextKey t)
    {
        var defFields = data.ImplType.HierarchyFields;
        int i = 0;
        List<DType> newFields = null;
        foreach (var fieldValue in data.Fields)
        {
            if (fieldValue == null)
            {
                i++;
                continue;
            }
            var defField = defFields[i];
            var fieldType = defField.CType;

            t.FieldName = defField.Name;
            DType newFieldValue = fieldValue.Apply(this, fieldType, t);
            if (newFieldValue != fieldValue)
            {
                if (newFields == null)
                {
                    newFields = new List<DType>(data.Fields);
                }
                newFields[i] = newFieldValue;
            }
            ++i;
        }
        return newFields == null ? data : new DBean(data.TType, data.ImplType, newFields);
    }

    public DType Accept(DArray data, TType type, TextKey t)
    {
        TType eleType = type.ElementType;
        List<DType> newDatas = null;
        int index = 0;
        foreach (var ele in data.Datas)
        {
            if (ele == null)
            {
                ++index;
                continue;
            }
            DType newEle = ele.Apply(this, eleType, t);
            if (newEle != ele)
            {
                if (newDatas == null)
                {
                    newDatas = new List<DType>(data.Datas);
                }
                newDatas[index] = newEle;
            }
            ++index;
        }
        return newDatas == null ? data : new DArray(data.Type, newDatas);
    }

    public DType Accept(DList data, TType type, TextKey t)
    {
        TType eleType = type.ElementType;
        List<DType> newDatas = null;
        int index = 0;
        foreach (var ele in data.Datas)
        {
            if (ele == null)
            {
                ++index;
                continue;
            }
            DType newEle = ele.Apply(this, eleType, t);
            if (newEle != ele)
            {
                if (newDatas == null)
                {
                    newDatas = new List<DType>(data.Datas);
                }
                newDatas[index] = newEle;
            }
            ++index;
        }
        return newDatas == null ? data : new DList(data.Type, newDatas);
    }

    public DType Accept(DSet data, TType type, TextKey t)
    {
        TType eleType = type.ElementType;
        List<DType> newDatas = null;
        int index = 0;
        foreach (var ele in data.Datas)
        {
            if (ele == null)
            {
                ++index;
                continue;
            }
            DType newEle = ele.Apply(this, eleType, t);
            if (newEle != ele)
            {
                if (newDatas == null)
                {
                    newDatas = new List<DType>(data.Datas);
                }
                newDatas[index] = newEle;
            }
            ++index;
        }
        return newDatas == null ? data : new DSet(data.Type, newDatas);
    }

    public DType Accept(DMap data, TType type, TextKey t)
    {
        TMap mapType = (TMap)type;
        bool dirty = false;
        foreach (var ele in data.DataMap)
        {
            DType newKey = ele.Key.Apply(this, mapType.KeyType, t);
            DType newValue = ele.Value.Apply(this, mapType.ValueType, t);
            if (newKey != ele.Key || newValue != ele.Value)
            {
                dirty = true;
                break;
            }
        }
        if (!dirty)
        {
            return data;
        }

        var newDatas = new Dictionary<DType, DType>();
        foreach (var ele in data.DataMap)
        {
            DType newKey = ele.Key.Apply(this, mapType.KeyType, t);
            DType newValue = ele.Value.Apply(this, mapType.ValueType, t);
            newDatas[newKey] = newValue;
        }
        return new DMap(data.Type, newDatas);
    }
}
