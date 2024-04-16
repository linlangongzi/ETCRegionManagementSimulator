using ETCRegionManagementSimulator.Utilities;
using System;
using System.Diagnostics;
using System.Net;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace ETCRegionManagementSimulator
{
    public sealed partial class StartUpPage : Page, IDisposable
    {

        private Server server;
        private BackUpServer backupServer;

        private MainPage mainPage;
        private bool disposedValue;

        private static bool backupServerRunningState = false;
        private static bool serverRunningState = false;
        public StartUpPage()
        {
            this.InitializeComponent();
            server = ServerService.Instance.Server;
            server.Ports = Utility.GetAvailablePorts();
            mainPage = new MainPage();

            backupServerPort.Text = server.Ports[0].ToString();
            connectionPortSecond.Text = server.Ports[1].ToString();
            connectionPortThird.Text = server.Ports[2].ToString();
            connectionPortForth.Text = server.Ports[3].ToString();
            connectionPortFifth.Text = server.Ports[4].ToString();

        }

        private async void OnStart(object sender, RoutedEventArgs e)
        {
            if (!serverRunningState)
            {
                string ipAddressStr = ip_address.Text.Trim();
                
                IPAddress ipAddr;
                if (!IPAddress.TryParse(ipAddressStr, out ipAddr))
                {
                    System.Diagnostics.Debug.WriteLine($"{ipAddressStr} is an invalid ip address");
                    return;
                }
                else if (ipAddr.AddressFamily!= System.Net.Sockets.AddressFamily.InterNetwork) {
                    System.Diagnostics.Debug.WriteLine($"{ipAddressStr} is an invalid ip address");
                    return;
                }
                server.DefaultIPAddress = ipAddr;
                await server.Start();
                server.Running = true;
                serverRunningState = true;
            }
            else
            {
                Debug.WriteLine($"Shut down the server");
                serverRunningState = false;
                server.StopTasks();
            }

            if (serverRunningState)
            {
                _ = Frame.Navigate(typeof(MainPage), server);
            }

            if (!backupServerRunningState)
            {
                StartBackupServer();
            }
            else
            {
                backupServerRunningState = false;
                if (backupServer != null)
                {
                    Debug.WriteLine("Stop Backup Server \n");
                    backupServer.StopMainServerMonitoring();
                }
            }
            //runPython();

            updateUI(serverRunningState);
        }

        private void runPython()
        {
            string filepath = "D:\\MyPythonTools\\CSTest\\Client.py";
            string argv = "127.0.0.1 5000";
            PythonRunner.RunPythonScript(filepath, argv);
        }


        private void StartBackupServer()
        {
            Debug.WriteLine("Start Backup Server \n");
            string backupServerIp = backupip_address.Text.Trim();
            IPAddress backupIpAddr;
            int backupPort = Utility.GetAvailablePorts()[1];
            if (!IPAddress.TryParse(backupServerIp, out backupIpAddr))
            {
                Debug.WriteLine($" Backup Server IP : {backupServerIp} is an invalid ip address");
            }

            backupServer = new BackUpServer(server.DefaultIPAddress, backupIpAddr, backupPort);
            backupServer.StartMainServerMonitoring();
            backupServerRunningState = true;
            System.Diagnostics.Debug.WriteLine($"Server started successfully ");
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

        private void mainView_Consolidated(ApplicationView sender, ApplicationViewConsolidatedEventArgs args)
        {
            sender.Consolidated -= mainView_Consolidated;
        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: マネージド状態を破棄します (マネージド オブジェクト)
                    mainPage.Dispose();
                    server.Dispose();
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
