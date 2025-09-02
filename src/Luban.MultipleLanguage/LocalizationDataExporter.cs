using Luban.DataTarget;
using Luban.Defs;

namespace Luban.MultipleLanguage;

[DataExporter(LocalizationOptionNames.LocalizationFamily)]
public class LocalizationDataExporter : DataExporterBase
{
    protected override void ExportCustom(List<DefTable> tables, OutputFileManifest manifest, IDataTarget dataTarget)
    {
        
    }
}
