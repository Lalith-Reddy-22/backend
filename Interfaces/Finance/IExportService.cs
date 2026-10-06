using Microsoft.AspNetCore.Mvc;

namespace Medtrix_PmTool_V2_Api.Interfaces.Finance
{
    public interface IExportService
    {
        FileContentResult ExportCsv<T>(IEnumerable<T> data, string fileName);

        FileContentResult ExportPdf<T>(IEnumerable<T> data, string fileName);
    }
}