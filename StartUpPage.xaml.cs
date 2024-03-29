using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ETCRegionManagementSimulator
{
    public sealed partial class StartUpPage : Page, IDisposable
    {

        private Server server;
        private MainPage mainPage;
        private bool disposedValue;

        public StartUpPage()
        {
            this.InitializeComponent();
            server = new Server();
            mainPage = new MainPage();
        }

        private async void OnStart(object sender, RoutedEventArgs e)
        {
            bool serverRunningState = false;
            if (!server.Running)
            {
                string ipAddressStr = ip_address.Text.Trim();
               // IPAddress ipAddress;
                await server.Start();
                server.Running = true;
                serverRunningState = true;
                Console.WriteLine($"Server is running");
            }
            else
            {
                Console.WriteLine($"Shutting down Running Server ");
                return;
            }
            updateUI(serverRunningState);

            startApplication();
        }

        private void updateUI(bool isServerRunning)
        {
            if (isServerRunning)
            {
                startServer.Content = "Running";
                startServer.Background = new SolidColorBrush(Colors.Green);
            }
            else
            {
                startServer.Content = "Start";
                startServer.Background = new SolidColorBrush(Colors.Red);
            }
        }

        private async void startApplication()
        {
            if (mainPage != null)
            {
                int currentViewId = ApplicationView.GetForCurrentView().Id;
                await CoreApplication.CreateNewView().Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    Window.Current.Content = new Frame();
                    ((Frame)Window.Current.Content).Navigate(typeof(MainPage), mainPage);
                    Window.Current.Activate();
                    await ApplicationViewSwitcher.TryShowAsStandaloneAsync(ApplicationView.GetApplicationViewIdForWindow(Window.Current.CoreWindow), ViewSizePreference.Default, currentViewId, ViewSizePreference.Default);
                });
            }
        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: マネージド状態を破棄します (マネージド オブジェクト)
                    mainPage.Dispose();
                }

                // TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、ファイナライザーをオーバーライドします
                // TODO: 大きなフィールドを null に設定します
                disposedValue = true;
            }
        }

        // TODO: 'Dispose(bool disposing)' にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします
        ~StartUpPage()
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
