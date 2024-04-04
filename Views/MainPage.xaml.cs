using ETCRegionManagementSimulator.Collections;
using ETCRegionManagementSimulator.Interfaces;
using ETCRegionManagementSimulator.Models;
using ETCRegionManagementSimulator.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


namespace ETCRegionManagementSimulator
{

    public sealed partial class MainPage : Page, IDisposable
    {
        private SettingPage settingPage;
        private FirstConnectionPage firstConnectionPage;
        private SecondConnectionPage secondConnectionPage;
        private ThirdConnectionPage thirdConnectionPage;
        private FourthConnectionPage fourthConnectionPage;
        private FifthConnectionPage fifthConnectionPage;
        private ExcelReader workBook;

        private bool disposedValue;

        private Server server;

        public MainPage()
        {
            this.InitializeComponent();

            settingPage = new SettingPage();
            firstConnectionPage = new FirstConnectionPage();
            secondConnectionPage = new SecondConnectionPage();
            thirdConnectionPage = new ThirdConnectionPage();
            fourthConnectionPage = new FourthConnectionPage();
            fifthConnectionPage = new FifthConnectionPage();

            // TODO: this is data for test; Delete there in the future
            ETCDataFormatCollection<ETCDataFormat> collection1 = new ETCDataFormatCollection<ETCDataFormat>();
            collection1.Add(new BCD(new byte[] { 0x01, 0x23 }));
            collection1.Add(new BCD(new byte[] { 0xAA, 0xBB, 0xCC }));
            collection1.Add(new BCD(new byte[] { 0x11, 0x22, 0x33, 0x44 }));
            collection1.Add(new BCD(new byte[] { 0xDE, 0xAD, 0xBE, 0xEF, 0x00 }));
            collection1.Add(new Hex(new byte[] { 0x88, 0x99 }));
            collection1.Add(new Hex(new byte[] { 0x55, 0x66, 0x77, 0x88, 0x99 }));
            collection1.Add(new Hex(new byte[] { 0x00, 0x11, 0x22, 0x33 }));

            ETCDataFormatCollection<ETCDataFormat> collection2 = new ETCDataFormatCollection<ETCDataFormat>();
            collection2.Add(new Hex(new byte[] { 0x45, 0x67 }));
            collection2.Add(new Hex(new byte[] { 0x88, 0x99 }));
            collection2.Add(new Hex(new byte[] { 0x55, 0x66, 0x77, 0x88, 0x99 }));
            collection2.Add(new Hex(new byte[] { 0x00, 0x11, 0x22, 0x33 }));
            collection2.Add(new BCD(new byte[] { 0xAA, 0xBB, 0xCC }));
            collection2.Add(new BCD(new byte[] { 0x11, 0x22, 0x33, 0x44 }));
            collection2.Add(new BCD(new byte[] { 0xDE, 0xAD, 0xBE, 0xEF, 0x00 }));

            ExcelDataPerSheet testExcelData = new ExcelDataPerSheet
            {
                DataPerSheet = new List<ExcelRow>
                {
                    // Initialize with sample data
                    new ExcelRow(1, "Row Title 1", 100, collection1, collection2),
                    new ExcelRow(2, "Row Title 2", 150, collection1, collection2),
                    new ExcelRow(3, "Row Title 3", 200, collection1, collection2),
                    new ExcelRow(4, "Row Title 4", 250, collection1, collection2),
                    new ExcelRow(5, "Row Title 5", 300, collection1, collection2),
                    new ExcelRow(6, "Row Title 6", 350, collection1, collection2),
                    new ExcelRow(7, "Row Title 7", 400, collection1, collection2),
                    new ExcelRow(8, "Row Title 8", 450, collection1, collection2)
                }
            };

            List<DisplayModel> displayableData = new List<DisplayModel>();
            foreach (var row in testExcelData.DataPerSheet)
            {
                displayableData.AddRange(DataFormatConverter.ConvertExcelRowToDisplayableList(row));
            }

            excelDataGrid.ItemsSource = displayableData;
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
                ExcelReader excelReader = new ExcelReader(file.Path);

                System.Diagnostics.Debug.WriteLine($"Test Excel Reader...{excelFilePath}....");
                // Read the Excel file
                excelReader.ReadExcelFile();

                // Close the Excel file
                excelReader.CloseExcelFile();
            }    

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter != null)
            {
                server = (Server)e.Parameter;
            }
        }

        private void OnNewClientConneted(object sender, ClientConnectedEventArgs eventArgs)
        {
            string clientId = eventArgs.ClientId;
            // TODO: Create New Page associated with new client
        }

        private void MainNavigation_OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            //TODO: Remove IsSettingsSelected check
            if (args.IsSettingsSelected)
            {
                ContentFrame.Navigate(typeof(SettingPage),settingPage);
                Grid.SetColumnSpan(MainNavigation, 2);
                Grid_SendMsg.Visibility = Visibility.Collapsed;
            }
            else 
            {
                Grid.SetColumnSpan(MainNavigation, 1);
                NavigationViewItem selectedItem = args.SelectedItem as NavigationViewItem;
                switch(selectedItem.Name.ToString())
                {
                    case "TabFirstPort":
                        ContentFrame.Navigate(typeof(FirstConnectionPage), firstConnectionPage);
                        break;
                    case "TabSecondPort":
                        ContentFrame.Navigate(typeof(SecondConnectionPage), secondConnectionPage);
                        break;
                    case "TabThirdPort":
                        ContentFrame.Navigate(typeof(ThirdConnectionPage), thirdConnectionPage);
                        break;
                    case "TabFourthPort":
                        ContentFrame.Navigate(typeof(FourthConnectionPage), fourthConnectionPage);
                        break;
                    case "TabFifthPort":
                        ContentFrame.Navigate(typeof(FifthConnectionPage), fifthConnectionPage);
                        break;
                    default:

                        break;
                }
                Grid_SendMsg.Visibility = Visibility.Visible;

            }
        }
        private void MainNavigation_OnLoaded(object sender, RoutedEventArgs e)
        {
            MainNavigation.SelectedItem = MainNavigation.MenuItems[0];
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
                this.server = null;
            }
        }

        private void MainNavigation_OnBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            Frame.GoBack();
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~MainPage()
        {
            //settingPage.Dispose();
            firstConnectionPage.Dispose();
            //secondConnectionPage.Dispose();
            thirdConnectionPage.Dispose();
            fourthConnectionPage.Dispose();
            fifthConnectionPage.Dispose();
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private async void button_FilePicker_OnClick(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".xlsx");

            StorageFile file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                textbox_filePath.Text = file.Path;
                try
                {
                    using (Stream stream = (await file.OpenReadAsync()).AsStreamForRead())
                    {
                        workBook = new ExcelReader(file.Path, stream);
                        workBook.ReadExcelFile();
                        foreach (string sheetName in workBook.SheetNames)
                        {
                            listbox_SheetsList.Items.Add(sheetName);
                        }
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    // Access to the file is denied due to lack of permissions
                    System.Diagnostics.Debug.WriteLine($"file {file.Path} not permitted");
                }
                catch (FileNotFoundException)
                {
                    // File not found, could be due to incorrect path or missing file
                    System.Diagnostics.Debug.WriteLine($"file {file.Path} not exist");
                }
            }
        }

        private void listbox_SheetsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
