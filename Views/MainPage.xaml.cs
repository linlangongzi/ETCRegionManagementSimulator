﻿using ETCRegionManagementSimulator.Controllers;
using ETCRegionManagementSimulator.Events;
using ETCRegionManagementSimulator.Interfaces;
using ETCRegionManagementSimulator.Models;
using ETCRegionManagementSimulator.Utilities;
using ETCRegionManagementSimulator.ViewModels;
using ETCRegionManagementSimulator.Views;
using Microsoft.Extensions.DependencyInjection;
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

        private IStandardPageFactory _pageFactory;
        /// TODO: Implementa central event aggregator to manage All the EventHandlers in the future release 
        public event EventHandler<SheetSelectedEventArgs> SheetSelected;
        public static event EventHandler<SendSelectedDataEventArgs> SendSelectedData;
        //public event EventHandler<string> SendButtonClicked;

        private bool disposedValue;

        private Dictionary<string, NavigationViewItemModel> NavigationViewItemCollection = new Dictionary<string, NavigationViewItemModel>();

        /// TODO: use DI to replace code below
        private IExcelService _excelService;
        private IDataModel _excelDataModel = new ExcelDataModel();
        private IView _view;

        private Server _server;

        private ResourceLoader _resourceLoader;
        private SettingPage SettingPage;

        public MainPage()
        {
            InitializeComponent();
            //MainNavigation.ItemInvoked += OnMainNavigation_ItemInvoked;
            InitializeDependencies();
            InitializeServer();
            InitializeMainUI();
        }

        private void InitializeDependencies()
        {
            App app = (App)Application.Current;
            _pageFactory = app.ServiceProvider.GetRequiredService<IStandardPageFactory>();

            _view = this;
            _excelService = new ExcelReader();
            ExcelDataController controller = new ExcelDataController(_excelDataModel, _view, _excelService);
        }

        private void InitializeServer()
        {
            _server = ServerService.Instance.Server;
            _server.NewClientConnected += OnNewClientConneted;
            _server.ClientDisconnected += OnClientDisconnected;
        }
        private void InitializeMainUI()
        {
            ///TODO migrate JIS strings to resource dictionary
            TB_LocalHostIP.Text = $"本機IPアドレス: {_server.DefaultIPAddress}.";
            TB_OpenPorts.Text = $"オーペンポート: [ {string.Join(" ; ", _server.Ports.Select(i => i.ToString()))} ].";
            TB_Remote_Client.Text = $"接続されていない.";
            SettingPage = new SettingPage();
            _resourceLoader = ResourceLoader.GetForCurrentView();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private void OnNewClientConneted(object sender, ClientConnectedEventArgs eventArgs)
        {
            string clientId = eventArgs.ClientId;

            ClientView clientView = new ClientView
            {
                ClientId = clientId,
                Name = clientId.ToLower(),
                Type = ClientView.ClientViewType.Standard
            };

            StandardClientPage clientPage = (StandardClientPage)_pageFactory.CreateStandardPage(clientView);

            /// TODO: adjust the Page navigation logic here by giving the content of
            /// NavigationView clientPage
            /// TODO: maybe move the ClientView and NavifationViewItem creation within the factory
            /// 
            NavigationViewItem item = new NavigationViewItem
            {
                Content = clientId,
                Icon = new SymbolIcon(Symbol.MapDrive),
                Tag = clientId.ToLower()
            };

            NavigationViewItemCollection.Add(clientId, new NavigationViewItemModel(clientId, clientPage, item));
            MainNavigation.MenuItems.Add(item);
            MainNavigation.SelectedItem = item;

            if (clientPage is StandardClientPage standardPage)
            {
                Type pageType = standardPage.GetType();
                _ = ContentFrame.Navigate(pageType, clientId);
            }
        }

        private void OnClientDisconnected(object sender, ClientDisconnetedEventArgs eventArgs)
        {

            string disconnectedClientId = eventArgs.DisconnectClientId;
            if (NavigationViewItemCollection.TryGetValue(disconnectedClientId, out NavigationViewItemModel disconnectedItem))
            {
                if (disconnectedItem is IDisposable disposable)
                {
                    disposable.Dispose();
                }
                NavigationViewItemCollection.Remove(disconnectedClientId);
            }
        }

        private void MainNavigation_OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (!args.IsSettingsSelected)
            {
                NavigationViewItem selectedItem = args.SelectedItem as NavigationViewItem;
                if (selectedItem != null)
                {
                    string id = selectedItem.Content.ToString();
                    if (NavigationViewItemCollection.TryGetValue(id, out NavigationViewItemModel selectedItemModel))
                    {
                        Page currentPage = selectedItemModel.StandardPage;

                        if (currentPage.DataContext == null)
                        {
                            Debug.WriteLine("Error: DataContext should have been set by now.");
                            // Optionally handle this situation, such as re-setting DataContext or logging an error.
                        } else
                        {
                            _ = ContentFrame.Navigate(currentPage.GetType(), id);
                        }

                    }
                    else
                    {
                        Debug.WriteLine("Error: No matching item in NavigationViewItemCollection.");
                    }
                }
            }
            else
            {
                ContentFrame.Navigate(typeof(SettingPage), SettingPage);
            }
        }


        private void MainNavigation_OnLoaded(object sender, RoutedEventArgs e)
        {
            NavigationViewItem setting = (NavigationViewItem)MainNavigation.SettingsItem;
            setting.Content = $"{_resourceLoader.GetString("Nav_Settings")}";
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
                //Client.MessageReceived -= OnMessageReceived;
                disposedValue = true;
                this._server = null;
            }
        }

        private void MainNavigation_OnBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            Frame.GoBack();
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~MainPage()
        {
            SettingPage.Dispose();
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
                        _excelService.ExcelFilePath = file.Path;
                        _excelService.ExcelFileStream = stream;
                        /// TODO: Code Here is not quite desirable; Consider a better way to make ReadExcelFile 
                        /// an actual Read method
                        _excelService.ReadExcelFile();
                        foreach (string sheetName in _excelService.SheetNames)
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
            //int index = excelDataGrid.SelectedIndex;
            //ExcelRow excelRow = _excelDataModel.GetDataPerRowById(index + 1);
            //Debug.WriteLine($"----------LINE---: {excelRow.FrameContent}");
        }

        public void UpdateView()
        {
            List<DisplayModel> displayableData = new List<DisplayModel>();
            if (_excelDataModel != null)
            {
                /// TODO : Maybe consider to make the model convertion process as a member of DataModel
                IEnumerable<ExcelRow> enumerable = _excelDataModel.GetAllData();
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
                //TestSource.Add($" {_resourceLoader.GetString("CONTROL_LINE")} {_resourceLoader.GetString("Log_OnSendComplete")}: { d.FullFrameDataSummary}");
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
