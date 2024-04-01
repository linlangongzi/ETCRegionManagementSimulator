﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using System.IO;

namespace ETCRegionManagementSimulator
{
    class ExcelReader : IDisposable
    {
        public string ExcelFilePath { get; set; }
        public List<string> SheetNames { get; set; }

        private ExcelPackage _package;

        private bool disposedValue;

        public ExcelReader()
        {
            ExcelFilePath = null;
            SheetNames = new List<string>();
        }
        public ExcelReader(string excelFilePath)
        {
            ExcelFilePath = excelFilePath;
            SheetNames = new List<string>();
        }


        public void OpenExcelFile()
        {
            var file = new FileInfo(ExcelFilePath);
            // Check if the file exists
   
                // Open the Excel file
                _package = new ExcelPackage(file);
                if (_package != null)
                {
                    System.Diagnostics.Debug.WriteLine($"##########Package is not null##########");
                }
            
        }

        public void ReadExcelFile()
        {
            if (_package == null)
            {
                System.Diagnostics.Debug.WriteLine("Excel file is not opened.");

                return;
            }

            System.Diagnostics.Debug.WriteLine($"---------------{_package.Workbook.Worksheets.Count}-------------------------.");
            // Read the Excel file
            foreach (ExcelWorksheet worksheet in _package.Workbook.Worksheets)
            {
                SheetNames.Add(worksheet.Name);
                System.Diagnostics.Debug.WriteLine($" Sheet added{ worksheet.Name}");
                ReadSheet(worksheet);
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

                // Iterate through each cell in the worksheet
                for (int row = 1; row <= rowCount; row++)
                {
                    for (int col = 1; col <= colCount; col++)
                    {
                        // Get the value of the cell
                        object cellValue = worksheet.Cells[row, col].Value;

                        // Do something with the cell value
                        Console.Write(cellValue + "\t");
                    }
                    System.Diagnostics.Debug.WriteLine("\n"); // Move to the next row
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
