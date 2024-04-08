using ETCRegionManagementSimulator.Controllers;
using ETCRegionManagementSimulator.Events;
using ETCRegionManagementSimulator.Interfaces;
using ETCRegionManagementSimulator.Models;
using ETCRegionManagementSimulator.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


namespace ETCRegionManagementSimulator
{
    public sealed partial class MainPage : Page, IView, IDisposable
    {
        private SettingPage settingPage;
        private FirstConnectionPage firstConnectionPage;
        private SecondConnectionPage secondConnectionPage;
        private ThirdConnectionPage thirdConnectionPage;
        private FourthConnectionPage fourthConnectionPage;
        private FifthConnectionPage fifthConnectionPage;

        /// TODO: use DI to replace code below
        private IExcelService excelService;
        IDataModel model = new ExcelDataModel();
        IView view;


        private bool disposedValue;

        private Server server;

        public event EventHandler<SheetSelectedEventArgs> SheetSelected;

        public MainPage()
        {
            this.InitializeComponent();

            settingPage = new SettingPage();
            firstConnectionPage = new FirstConnectionPage();
            secondConnectionPage = new SecondConnectionPage();
            thirdConnectionPage = new ThirdConnectionPage();
            fourthConnectionPage = new FourthConnectionPage();
            fifthConnectionPage = new FifthConnectionPage();

            view = this;
            excelService = new ExcelService();
            ExcelDataController controller = new ExcelDataController(model, view, excelService);
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
                        excelService.ExcelFilePath = file.Path;
                        excelService.ExcelFileStream = stream;
                        /// TODO: Code Here is not quite desirable; Consider a better way to make ReadExcelFile 
                        /// an actual Read method
                        excelService.ReadExcelFile();
                        foreach (string sheetName in excelService.SheetNames)
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

        private void Listbox_SheetsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedSheetName = listbox_SheetsList.SelectedItem.ToString();
            SheetSelected?.Invoke(this, new SheetSelectedEventArgs(selectedSheetName));
        }

        public void UpdateView()
        {
            List<DisplayModel> displayableData = new List<DisplayModel>();
            if (model != null)
            {
                /// TODO : Maybe consider to make the model convertion process as a member of DataModel
                IEnumerable<ExcelRow> enumerable = model.GetAllData();
                foreach (ExcelRow row in enumerable)
                {
                    displayableData.AddRange(DataFormatConverter.ConvertExcelRowToDisplayableList(row));
                }
                excelDataGrid.ItemsSource = displayableData;
            }
        }

        public void SetController(IController controller)
        {
            SheetSelected += (sender, e) => controller.LoadDataFromSheet(e.SheetName);
        }
    }
}
