using ETCRegionManagementSimulator.Events;
using ETCRegionManagementSimulator.Interfaces;
using ETCRegionManagementSimulator.ViewModels;
using System;
using System.Diagnostics;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ETCRegionManagementSimulator.Views
{
    public sealed partial class StandardClientPage : Page, IDisposable
    {
        private string _clientId;

        private ClientViewModel _clientViewModel;
        public StandardClientPage()
        {
            this.InitializeComponent();
            _clientViewModel = new ClientViewModel();
            this.DataContext = _clientViewModel;

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        public StandardClientPage(ClientViewModel viewModel)
        {
            this.InitializeComponent();
            DataContext = viewModel;
            _clientViewModel = viewModel;
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        private void OnUnloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            MessageService.Instance.Unsubscribe(_clientId, StandardClientPage_MessageReceivedAsync);
        }

        private void OnLoaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            MessageService.Instance.Subscribe(_clientId, StandardClientPage_MessageReceivedAsync);
        }

        private async void StandardClientPage_MessageReceivedAsync(object sender, MessageReceivedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (DataContext != null && _clientViewModel != null)
                {
                    _clientViewModel.ReceiveMessage(e.Message);
                }
            });
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _clientId = e.Parameter as string;

            Debug.WriteLine($" StandardClientPage OnNavigatedTo  DataContext  {this.DataContext} ");

            //if (App.ViewModelState.ContainsKey(_clientId))
            //{
            //    _clientViewModel = App.ViewModelState[_clientId];
            //    DataContext = _clientViewModel;
            //}
            //else
            //{
            //    _clientViewModel = new ClientViewModel();
            //    DataContext = _clientViewModel;
            //}
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            Debug.WriteLine($" StandardClientPage FROM  DataContext  {this.DataContext} ");
            // Save state here, e.g., to local settings or a static property
            //App.ViewModelState[_clientId] = _clientViewModel;
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
                _clientViewModel = null;
            }
        }

        ~StandardClientPage()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
