using System.Collections.Generic;
using System.IO;

namespace ETCRegionManagementSimulator.Interfaces
{
    public interface IExcelService
    {
        string ExcelFilePath { get; set; }
        Stream ExcelFileStream { get; set; }
        List<string> SheetNames { get; set; }

        IEnumerable<ExcelRow> ReadExcelFile();
        IEnumerable<ExcelRow> ReadSheet(string sheetName);
        // TODO: Add more methods for additional operations, like writing data
    }
}
