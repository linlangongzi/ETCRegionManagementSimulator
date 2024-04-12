using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using OfficeOpenXml;
using ETCRegionManagementSimulator.Interfaces;
using ETCRegionManagementSimulator.Collections;
using ETCRegionManagementSimulator.Utilities;
using System.Diagnostics;

namespace ETCRegionManagementSimulator
{
    public class ExcelReader : IExcelService, IDisposable
    {
        public List<string> SheetNames { get; set; }
        public Stream ExcelFileStream { get; set; }
        public string ExcelFilePath { get; set; }

        private enum CellDataType
        {
            bin,
            hex,
            bcd,
            x1
        }

        private readonly Dictionary<string, CellDataType> _typeMappings = new Dictionary<string, CellDataType>(StringComparer.OrdinalIgnoreCase)
        {
            { "bin", CellDataType.bin },
            { "bcd", CellDataType.bcd },
            // Assuming "hex" is the default, it does not need to be in the dictionary
        };

        //private readonly ILogger _logger;
        //private readonly IConfiguration _configuration;

        private ExcelPackage _package;

        private bool disposedValue;

        public ExcelReader()
        {
            ExcelFilePath = null;
            SheetNames = new List<string>();
            ExcelFileStream = null;
            _package = null;
        }
        public ExcelReader(string excelFilePath)
        {
            SheetNames = new List<string>();
            ExcelFilePath = excelFilePath;
            // TODO: need to replace the File.Exists() check method from the one in Storage Object
            ExcelFileStream = File.Exists(ExcelFilePath) ? File.OpenRead(ExcelFilePath) : null;
            _package = ExcelFileStream != null ? new ExcelPackage(ExcelFileStream) : null;
        }

        public ExcelReader(string excelFilePath, Stream excelStream)
        {
            ExcelFilePath = excelFilePath;
            ExcelFileStream = excelStream;
            SheetNames = new List<string>();
            _package = ExcelFileStream != null ? new ExcelPackage(ExcelFileStream) : null;
        }

        /// TODO: Need to optimize framework structure in the future to
        /// make the method below behave more like a Read File method 
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
            if (!string.IsNullOrEmpty(sheetName))
            {
                if (_package != null)
                {
                    ExcelWorksheet worksheet = _package.Workbook.Worksheets.FirstOrDefault(ws => string.Equals(ws.Name, sheetName));
                    if (worksheet != null)
                    {
                        int rowCount = worksheet.Dimension?.Rows ?? 0;
                        // Process the Actual Data
                        for (int row = FixedColumnIndex.ACTUAL_DATA_ROW; row <= rowCount; row++)
                        {
                            ExcelRow excelRow = AssemblyExcelRow(worksheet, row);
                            rows.Add(excelRow);
                        }
                    }
                    else
                    {
                        Debug.WriteLine($"Worksheet with name '{sheetName}' not found.\n");
                    }
                }
            }
            return rows;
        }

        private List<CellDataType> TypeList(ExcelWorksheet worksheet)
        {
            List<CellDataType> typeList = new List<CellDataType>();
            int colCount = worksheet.Dimension?.Columns ?? 0;

            for (int column = FixedColumnIndex.ACTUAL_DATA_COL; column <= colCount; column++)
            {
                string typeString = worksheet.Cells[FixedColumnIndex.TYPE_ROW, column].Value.ToString();
                if (_typeMappings.TryGetValue(typeString, out CellDataType cellType))
                {
                    typeList.Add(cellType);
                }
                else
                {
                    typeList.Add(CellDataType.hex); // Default to hex
                }
            }
            return typeList;
        }

        private ExcelRow AssemblyExcelRow(ExcelWorksheet worksheet, int row)
        {
            int frameDataNo = Convert.ToInt32(worksheet.Cells[row, FixedColumnIndex.FRAME_NO_COL].Value);
            string frameDataTitle = worksheet.Cells[row, FixedColumnIndex.FRAME_DATA_TITLE].Value.ToString();
            int frameDataLength = Convert.ToInt32(worksheet.Cells[row, FixedColumnIndex.FRAME_LENGTH].Value);

            ETCDataFormatCollection<IDataFormat> frameCommonHeader = new ETCDataFormatCollection<IDataFormat>();
            ETCDataFormatCollection<IDataFormat> frameContent = new ETCDataFormatCollection<IDataFormat>();

            /// TODO: Use RAII to catch type convertion expections
            List<CellDataType> typeList = TypeList(worksheet);
            // Header
            for (int col = FixedColumnIndex.ACTUAL_DATA_COL; col <= FixedColumnIndex.HEADER_TAIL; col++)
            {
                object cellValue = worksheet.Cells[row, col].Value;
                switch (typeList[col])
                {
                    case CellDataType.bcd:
                        frameCommonHeader.Add(new BCD(cellValue));
                        break;
                    case CellDataType.bin:
                        frameCommonHeader.Add(new Binary(cellValue));
                        break;
                    default:
                        frameCommonHeader.Add(new Hex(cellValue));
                        break;
                }
            }
            // Content
            int typeListOffset = FixedColumnIndex.ACTUAL_DATA_COL - 1;
            int colCount = worksheet.Dimension?.Columns ?? 0;
            for (int col = FixedColumnIndex.HEADER_TAIL + 1; col <= colCount; col++)
            {
                int typeIndex = col - typeListOffset - 1;
                if (typeIndex < 0 || typeIndex >= typeList.Count)
                {
                    continue; 
                }
                object cellValue = worksheet.Cells[row, col].Value;
                CellDataType cellType = typeList[typeIndex];
                switch (cellType)
                {
                    case CellDataType.bcd:
                        frameContent.Add(new BCD(cellValue));
                        break;
                    case CellDataType.bin:
                        frameContent.Add(new Binary(cellValue));
                        break;
                    default:
                        frameContent.Add(new Hex(cellValue));
                        break;
                }
            }

            ExcelRow excelRow = new ExcelRow(frameDataNo, frameDataTitle, frameDataLength, frameCommonHeader, frameContent);
            return excelRow;
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
