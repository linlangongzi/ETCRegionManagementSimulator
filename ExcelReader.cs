using System;
using System.Collections.Generic;
using OfficeOpenXml;
using System.IO;

namespace ETCRegionManagementSimulator
{
    class ExcelReader : IDisposable
    {
        public string ExcelFilePath { get; set; }
        public List<string> SheetNames { get; set; }
        public Stream ExcelFileStream { get; set; }

        private ExcelPackage _package;

        private bool disposedValue;

        public ExcelReader()
        {
            ExcelFilePath = null;
            SheetNames = new List<string>();
            ExcelFileStream = null;
        }
        public ExcelReader(string excelFilePath)
        {
            SheetNames = new List<string>();
            ExcelFilePath = excelFilePath;
            // TODO: need to replace the File.Exists() check method from the one in Storage Object
            ExcelFileStream = File.Exists(excelFilePath) ? File.OpenRead(excelFilePath) : null;
        }

        public ExcelReader(string excelFilePath, Stream excelStream)
        {
            ExcelFileStream = excelStream;
            ExcelFilePath = excelFilePath;
            SheetNames = new List<string>();
        }

        public void ReadExcelFile()
        {
            if (ExcelFileStream != null)
            {
                _package = new ExcelPackage(ExcelFileStream);
                if (_package != null)
                {
                    foreach (ExcelWorksheet worksheet in _package.Workbook.Worksheets)
                    {
                        SheetNames.Add(worksheet.Name);
                        ReadSheet(worksheet);
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Excel file is not opened.");
                    return;
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Read Excel File failed.");
            }
        }

        private void ReadSheet(ExcelWorksheet worksheet)
        {
            // Get the number of rows and columns in the worksheet
            System.Diagnostics.Debug.WriteLine($"Reading Sheet {worksheet.Name}");
            if (worksheet != null)
            {
                int rowCount = worksheet.Dimension.Rows;
                int colCount = worksheet.Dimension.Columns;

                System.Diagnostics.Debug.WriteLine($" Column Counts: {colCount}  Row Counts : {rowCount}");

                // Iterate through each cell in the worksheet
                for (int row = 1; row <= rowCount; row++)
                {
                    for (int col = 1; col <= colCount; col++)
                    {
                        // Get the value of the cell
                        object cellValue = worksheet.Cells[row, col].Value;
                        // Do something with the cell value
                    }
                }
            }
        }

        public void CloseExcelFile()
        {
            if (_package != null)
            {
                _package.Dispose();
                _package = null;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
                SheetNames.Clear();
                CloseExcelFile();
            }
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~ExcelReader()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
