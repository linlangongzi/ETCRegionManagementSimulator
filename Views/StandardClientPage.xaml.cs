using ETCRegionManagementSimulator.Events;
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
            DataContext = viewModel;
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
                // Update the UI with the received message specific to this client
                Debug.WriteLine($"{this} : {e.ClientId} Received Messages :{e.Message}");
                ClientViewModel viewModel = DataContext as ClientViewModel;
                viewModel?.ReceiveMessage(e.Message);
            });
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _clientId = e.Parameter as string;
            if (_clientViewModel == null)
            {
                _clientViewModel = new ClientViewModel(); // or fetch from a higher level
            }
            this.DataContext = _clientViewModel;
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }
            _clientViewModel = null;
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
