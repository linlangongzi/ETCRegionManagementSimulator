﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
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
                System.Diagnostics.Debug.WriteLine($"Server is running");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Shutting down Running Server ");
                //TO DO: Stop server.
                //return;
            }
            updateUI(serverRunningState);
            if (server.Running)
            {
                _ = Frame.Navigate(typeof(MainPage), server);
            }
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
