using ETCRegionManagementSimulator.Controllers;
using ETCRegionManagementSimulator.Events;
using ETCRegionManagementSimulator.Interfaces;
using ETCRegionManagementSimulator.Models;
using ETCRegionManagementSimulator.Utilities;
using ETCRegionManagementSimulator.ViewModels;
using ETCRegionManagementSimulator.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ETCRegionManagementSimulator
{
    public sealed partial class MainPage : Page, IView, IDisposable
    {
        //public event EventHandler<string> SendButtonClicked;

        private SettingPage settingPage;

        /// TODO: use DI to replace code below
        private IExcelService excelService;
        IDataModel model = new ExcelDataModel();
        IView view;

        private bool disposedValue;

        private Server server;
        /// TODO: Implementa central event aggregator to manage All the EventHandlers in the future release 
        public event EventHandler<SheetSelectedEventArgs> SheetSelected;
        public static event EventHandler<SendSelectedDataEventArgs> SendSelectedData;

        public ObservableCollection<string> testSource { get; } = new ObservableCollection<string>();
        ResourceLoader resourceLoader;
        public MainPage()
        {
            InitializeComponent();

            server = ServerService.Instance.Server;
            server.NewClientConnected += OnNewClientConneted;
            server.ClientDisconnected += OnClientDisconnected;

            Client.MessageReceived += OnMessageReceived;
            //MainNavigation.ItemInvoked += OnMainNavigation_ItemInvoked;

            ///TODO migrate JIS strings to resource dictionary
            TB_LocalHostIP.Text = $"本機IPアドレス: {server.DefaultIPAddress}.";
            TB_OpenPorts.Text = $"オーペンポート: [ {string.Join(" ; ", server.Ports.Select(i => i.ToString()))} ].";
            TB_Remote_Client.Text = $"接続されていない.";
            settingPage = new SettingPage();

            view = this;
            excelService = new ExcelReader();

            ExcelDataController controller = new ExcelDataController(model, view, excelService);
            resourceLoader= ResourceLoader.GetForCurrentView();
        }

        //private void OnMainNavigation_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        //{
        //    NavigationViewItem invokedItem = args.InvokedItem as NavigationViewItem;
        //    if (invokedItem != null)
        //    {
        //        //switch (invokedItem.Tag)
        //        //{
        //        //    case "home":
        //        //        ContentFrame.Navigate(typeof(StandardPage));
        //        //        break;
        //        //    case "contacts":
        //        //        ContentFrame.Navigate(typeof(StandardPage));
        //        //        break;
        //        //    case "settings":
        //        //        ContentFrame.Navigate(typeof(SettingPage));
        //        //        break;
        //        //        // Add more cases as needed for other menu items
        //        //}
        //        if (invokedItem.is)
        //        ContentFrame.Navigate(typeof(StandardPage));
        //    }
        //}

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            /// TODO: delete the reduntant code below
            if (e.Parameter != null)
            {
                server = (Server)e.Parameter;
            }
        }

        private void OnNewClientConneted(object sender, ClientConnectedEventArgs eventArgs)
        {
            string clientId = eventArgs.ClientId;

            StandardPage clientPage = (StandardPage) ClientPageFactory.Instance.CreateClientPage(clientId);

            NavigationViewItem menuItem = new NavigationViewItem
            {
                Content = clientId,
                Icon = new SymbolIcon(Symbol.MapDrive),
                Tag = clientId.ToLower()
            };
            Debug.WriteLine($"A New Client :  {clientId} is connected \n");
            // Add it to the NavigationView
            MainNavigation.MenuItems.Add(menuItem);
            MainNavigation.SelectedItem = menuItem;
            ContentFrame.Navigate(typeof(StandardPage), clientId);
        }

        private void OnClientDisconnected(object sender, ClientDisconnetedEventArgs eventArgs)
        {
            /// TODO: Update UI to reduce the NavigationItem in NavigationView
            testSource.Add($"Disconneted from client : {eventArgs.DisconnectClientId}");
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            string message = e.Message;
            string senderId = e.ClientId;
            UpdateClientPageMessageView(senderId, message);
        }

        private async void UpdateClientPageMessageView(string senderId, string message)
        {
            StandardPage currentPage = (StandardPage) ClientPageFactory.Instance.GetPageById(senderId);
            if (currentPage != null)
            {
                Debug.WriteLine($" >> Sender ID : {senderId} \n >>  CurrentPage is  : {currentPage.ClientPageId} Receive : {message} \n");
                currentPage.UpdateMessageView(message);
            }
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                //    /// TODO: update UI element with the messages here
                //    currentPage.SourceMessages.Add(new MessageViewModel() { Message = message });
                testSource.Add(message);

                //foreach (string t in testSource)
                //{
                //    Debug.WriteLine($" Test source messages: {t} \n");
                //}
            });
        }
        private void MainNavigation_OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            //TODO: Remove IsSettingsSelected check
            if (!args.IsSettingsSelected)
            {
                NavigationViewItem selectedItem = args.SelectedItem as NavigationViewItem;
                if (selectedItem != null)
                {
                    string v = selectedItem.Content.ToString();
                    StandardPage currentPage = (StandardPage)ClientPageFactory.Instance.GetPageById(v);
                    _ = ContentFrame.Navigate(typeof(StandardPage), v);
                }
            }
            else
            {
                ContentFrame.Navigate(typeof(SettingPage), settingPage);
            }
        }

        private void MainNavigation_OnLoaded(object sender, RoutedEventArgs e)
        {
            //MainNavigation.SelectedItem = MainNavigation.MenuItems[0];
            var setting = (NavigationViewItem)MainNavigation.SettingsItem;
            setting.Content = $"{resourceLoader.GetString("Nav_Settings")}";
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
                Client.MessageReceived -= OnMessageReceived;
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
            settingPage.Dispose();
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
                    Debug.WriteLine($"file {file.Path} not permitted");
                }
                catch (FileNotFoundException)
                {
                    // File not found, could be due to incorrect path or missing file
                    Debug.WriteLine($"file {file.Path} not exist");
                }
            }

        }

        private void Listbox_SheetsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedSheetName = listbox_SheetsList.SelectedItem.ToString();
            SheetSelected?.Invoke(this, new SheetSelectedEventArgs(selectedSheetName));
            int index = excelDataGrid.SelectedIndex;
            ExcelRow excelRow = model.GetDataPerRowById(index + 1);
            Debug.WriteLine($"----------LINE---: {excelRow.FrameContent}");
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
                    displayableData.AddRange(ExcelDisplayAdaptor.ConvertToDisplayableList(row));
                }
                excelDataGrid.ItemsSource = displayableData;
            }
        }

        public void SetController(IController controller)
        {
            SheetSelected += (sender, e) => controller.LoadDataFromSheet(e.SheetName);
        }

        private void SendRow_Click(object sender, RoutedEventArgs e)
        {
            int index = excelDataGrid.SelectedIndex;
            object selectedRow = excelDataGrid.SelectedItem;
            if (selectedRow is DisplayModel d)
            {
                Debug.WriteLine($"Click on at Item: {d.FullFrameDataSummary} at index : {index} \n");
                /// TODO: Use Share Model to Send Data in future release!
                /// Priority: High!
                SendSelectedData?.Invoke(this, new SendSelectedDataEventArgs(index, d.FullFrameDataSummary));
                /// TODO REPLACE CLIENT ID
                testSource.Add($" {resourceLoader.GetString("CONTROL_LINE")} {resourceLoader.GetString("Log_OnSendComplete")}: { d.FullFrameDataSummary}");
            }
        }

        private void excelDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object selectedRow = excelDataGrid.SelectedItem;

            int index = excelDataGrid.SelectedIndex;

            var selectedRows = excelDataGrid.SelectedItems;

            if (selectedRow is DisplayModel s)
            {
                Debug.WriteLine($"Selected item: {s.FullFrameDataSummary}");
            }
            Debug.WriteLine($"Selected item: {selectedRow} at {index}");

        }

        private void excelDataGrid_AutoGeneratingColumn(object sender, Microsoft.Toolkit.Uwp.UI.Controls.DataGridAutoGeneratingColumnEventArgs e)
        {
            switch (e.Column.Header.ToString())
            {
                case "FrameDataNo":
                    e.Column.Header = "No";
                    break;
                case "FrameDataTitle":
                    e.Column.Header = "Type";
                    break;
                case "FrameDataLength":
                    e.Column.Header = "Length";
                    break;
                case "FrameCommonHeaderSummary":
                    e.Column.Header = "CommonHeader";
                    break;
                case "FrameContentSummary":
                    e.Column.Header = "Content";
                    break;
                case "FullFrameDataSummary":
                    e.Column.Header = "FrameData";
                    break;
            }
        }
    }
}
