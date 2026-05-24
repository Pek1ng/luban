# Luban

[Luban](https://github.com/focus-creative-games/luban)

## 新增特性

- 添加了将原文本替换成多语言Key的功能(只有text和单键表才会自动生成),需要要指定`ITextProvider`为`textToKey`。
- 可以生成从多语言key映射到原文本的csv。需要加上生成的数据目标`text-map`。

```
dotnet Luban.dll -t all -d text-map ^
--conf D:\workspace2\luban_examples\DataTables\luban.conf ^
--validationFailAsError ^
-x text-map.outputDataDir=D:\workspace2\luban_examples\Projects\GenerateDatas\text ^
-x l10n.provider=textToKey ^
-x l10n.textFile.keyFieldName=Key ^
-x l10n.textFile.languageFieldName="Chinese (Simplified)(zh-Hans)"
```

## 修改内容

功能实现全在`src\Luban.L10N\TextToKey`目录下