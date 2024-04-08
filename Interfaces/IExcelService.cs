using System.Collections.Generic;

namespace ETCRegionManagementSimulator.Interfaces
{
    public interface IExcelService
    {
        string ExcelFilePath { get; set; }
        IEnumerable<ExcelRow> ReadExcelFile();
        IEnumerable<ExcelRow> ReadSheet(string sheetName);
        // TODO: Add more methods for additional operations, like writing data
    }
}
