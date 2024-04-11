using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace ETCRegionManagementSimulator
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class SettingPage : Page, IDisposable
    {
        private bool disposedValue;
        private Logger logger;

        public SettingPage()
        {
            this.InitializeComponent();
            logger = Logger.Instance();
        }

        private void Dispose(bool disposing)
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
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~SettingPage()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private async void openDir_Click(object sender, RoutedEventArgs e)
        {
            // Create a file name based on the specified convention
            string fileName = $"log_{DateTime.Now:yyyyMMddHHmmss}.log";

            // Create a file picker instance
            var filePicker = new FileSavePicker();
            filePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            filePicker.FileTypeChoices.Add("Log files", new string[] { ".log" });
            filePicker.DefaultFileExtension = ".log";
            filePicker.SuggestedFileName = fileName;

            // Show file picker and wait for user selection
            StorageFile file = await filePicker.PickSaveFileAsync();
            if (file != null)
            {
                logPath.Text = file.Path;
                await logger.Start(file);

                logger.Log("SYSTEM","LoggerBEGUN");
            }
        }
    }
}
