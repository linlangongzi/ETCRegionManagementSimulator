using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace ETCRegionManagementSimulator
{

    public sealed partial class MainPage : Page, IDisposable
    {
        private bool disposedValue;



        public MainPage()
        {
            this.InitializeComponent();
            //TestExcelReader();
        }

        public void TestExcelReader()
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".xlsx");
            IAsyncOperation<StorageFile> asyncFilePick = picker.PickSingleFileAsync();

            StorageFile file = asyncFilePick.GetResults();

            if (file != null)
            {
                string excelFilePath = file.Path;
                // Create an instance of ExcelReader
                ExcelReader excelReader = new ExcelReader(excelFilePath);

                System.Diagnostics.Debug.WriteLine($"Test Excel Reader...{excelFilePath}....");
                // Open the Excel file
                excelReader.OpenExcelFile();

                // Read the Excel file
                excelReader.ReadExcelFile();

                // Close the Excel file
                excelReader.CloseExcelFile();
            }    

        }

        private void MainNavigation_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if(args.IsSettingsSelected)
            {
                
            }
            else 
            {
            
            }
        }


        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: マネージド状態を破棄します (マネージド オブジェクト)
                }

                // TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、ファイナライザーをオーバーライドします
                // TODO: 大きなフィールドを null に設定します
                disposedValue = true;
            }
        }

        // TODO: 'Dispose(bool disposing)' にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします
        ~MainPage()
        {
            // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

    }
}
