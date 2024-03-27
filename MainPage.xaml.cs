using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    public sealed partial class MainPage : Page
    {

        private Server server;

        public MainPage()
        {
            this.InitializeComponent();
            server = new Server();
        }

        private async void OnStart(object sender, RoutedEventArgs e)
        {
            bool serverRunningState = false;
            if (!server.Running)
            {
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
    }
}
