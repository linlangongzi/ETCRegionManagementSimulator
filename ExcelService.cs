using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using OfficeOpenXml;
using ETCRegionManagementSimulator.Interfaces;
using ETCRegionManagementSimulator.Collections;
using ETCRegionManagementSimulator.Utilities;

namespace ETCRegionManagementSimulator
{
    public class ExcelService : IExcelService, IDisposable
    {
        public List<string> SheetNames { get; set; }
        public Stream ExcelFileStream { get; set; }
        public string ExcelFilePath { get; set; }
        //private readonly ILogger _logger;
        //private readonly IConfiguration _configuration;

        private ExcelPackage _package;

        private bool disposedValue;

        public ExcelService()
        {
            ExcelFilePath = null;
            SheetNames = new List<string>();
            ExcelFileStream = null;
            _package = null;
        }
        public ExcelService(string excelFilePath)
        {
            SheetNames = new List<string>();
            ExcelFilePath = excelFilePath;
            // TODO: need to replace the File.Exists() check method from the one in Storage Object
            ExcelFileStream = File.Exists(ExcelFilePath) ? File.OpenRead(ExcelFilePath) : null;
            _package = ExcelFileStream != null ? new ExcelPackage(ExcelFileStream) : null;

        }

        public ExcelService(string excelFilePath, Stream excelStream)
        {
            ExcelFilePath = excelFilePath;
            ExcelFileStream = excelStream;
            SheetNames = new List<string>();
            _package = ExcelFileStream != null ? new ExcelPackage(ExcelFileStream) : null;
        }

        /// TODO: Need to optimize framework structure in the futiure to
        /// make the method below behave more a Read method 
        public IEnumerable<ExcelRow> ReadExcelFile()
        {
            List<ExcelRow> rows = new List<ExcelRow>();

            if (string.IsNullOrEmpty(ExcelFilePath))
            {
                throw new InvalidOperationException("ExcelFilePath has not been set.");
            }

            if (_package == null)
            {
                _package = ExcelFileStream != null ? new ExcelPackage(ExcelFileStream) : null;
            }
            //else
            //{
            //    System.Diagnostics.Debug.WriteLine("Excel file is not opened.");
            //}
            foreach (ExcelWorksheet worksheet in _package.Workbook.Worksheets)
            {
                if (string.Equals(worksheet.Name, "目次"))
                {
                    continue;
                }
                SheetNames.Add(worksheet.Name);
            }
          
            return rows;
        }

        public IEnumerable<ExcelRow> ReadSheet(string sheetName)
        {
            List<ExcelRow> rows = new List<ExcelRow>();
            // Get the number of rows and columns in the worksheet
            // TODO: Priority level: Very low.
            // Delete all the Debug output inside of this method after fully functioned
            if (!string.IsNullOrEmpty(sheetName))
            {
                System.Diagnostics.Debug.WriteLine($"Reading Sheet {sheetName}");
                if (_package != null)
                {
                    ExcelWorksheet worksheet = _package.Workbook.Worksheets.FirstOrDefault(ws => string.Equals(ws.Name, sheetName));
                    if (worksheet != null)
                    {

                        /// TODO: make following code into a seperated method
                        int rowCount = worksheet.Dimension?.Rows ?? 0;
                        int colCount = worksheet.Dimension?.Columns ?? 0;
                        
                        ETCDataFormatCollection<IDataFormat> frameCommonHeader = new ETCDataFormatCollection<IDataFormat>();
                        ETCDataFormatCollection<IDataFormat> frameContent = new ETCDataFormatCollection<IDataFormat>();

                        System.Diagnostics.Debug.WriteLine($" Column Counts: {colCount}  Row Counts : {rowCount}");
                        /// TODO : Change the Capital Writing local variable into
                        // something else that can be configured in the future
                        // Definitly Can NOT be hardcoded
                        
                        int ACTUAL_DATA_ROW_NO = 15;
                        int ACTUAL_DATA_COL_NO = 4;
                        int HEADER_TAIL_NO = 11;
                        for (int row = ACTUAL_DATA_ROW_NO; row <= rowCount; row++)
                        {
                            System.Diagnostics.Debug.WriteLine($" Assembling row: { row } ");
                            /// TODO: Use RAII to catch type convertion expections
                            int frameDataNo = Convert.ToInt32(worksheet.Cells[row, 1].Value);
                            string title = worksheet.Cells[row, 2].Value.ToString();
                            int frameDataLength = Convert.ToInt32(worksheet.Cells[row, 3].Value);
                            // Header
                            for (int col = ACTUAL_DATA_COL_NO; col <= HEADER_TAIL_NO; col++)
                            {
                                object cellValue = worksheet.Cells[row, col].Value;
                                byte[] bytes = DataFormatConverter.ObjectToByteArray(cellValue);
                                /// TODO: Add Data by its representitive Type BCD or HEX
                                // Here i made the all to bcd for test, modify it later
                                frameCommonHeader.Add(new BCD(bytes));
                                System.Diagnostics.Debug.WriteLine($" column : {col}, Cell : {cellValue}  ");
                            }
                            // Content
                            for (int col = HEADER_TAIL_NO + 1; col <= colCount; col++)
                            {
                                object cellValue = worksheet.Cells[row, col].Value;
                                byte[] bytes = DataFormatConverter.ObjectToByteArray(cellValue);
                                /// TODO: Add Data by its representitive Type BCD or HEX
                                // Here i made the all to bcd for test, modify it later
                                frameContent.Add(new BCD(bytes));
                                System.Diagnostics.Debug.WriteLine($" column : {col}, Cell : {cellValue}  ");
                            }

                            ExcelRow excelRow = new ExcelRow(frameDataNo, title, frameDataLength, frameCommonHeader, frameContent);
                            rows.Add(excelRow);
                            System.Diagnostics.Debug.WriteLine("\n");
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Worksheet with name '{sheetName}' not found.");
                    }
                }
            }
            return rows;
        }

        private List<string> SheetsFilter(in List<string> names)
        {
            List<string> finalList = new List<string>();
            foreach(string name in names)
            {
                if (!string.Equals(name, "目次"))
                {
                    finalList.Add(name);
                }
            }
            return finalList;
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
        ~ExcelService()
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
