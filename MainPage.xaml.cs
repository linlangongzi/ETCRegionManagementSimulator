using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace ETCRegionManagementSimulator
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page, IDisposable
    {
        private SettingPage settingPage;
        private FirstConnectionPage firstConnectionPage;
        private SecondConnectionPage secondConnectionPage;
        private ThirdConnectionPage thirdConnectionPage;
        private FourthConnectionPage fourthConnectionPage;
        private FifthConnectionPage fifthConnectionPage;

        private bool disposedValue;

        public MainPage()
        {
            this.InitializeComponent();

            settingPage = new SettingPage();
            firstConnectionPage = new FirstConnectionPage();
            secondConnectionPage = new SecondConnectionPage();
            thirdConnectionPage = new ThirdConnectionPage();
            fourthConnectionPage = new FourthConnectionPage();
            fifthConnectionPage = new FifthConnectionPage();

        }

        private void MainNavigation_OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if(args.IsSettingsSelected)
            {
                ContentFrame.Navigate(typeof(SettingPage),settingPage);
            }
            else 
            {
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
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~MainPage()
        {
            settingPage.Dispose();
            firstConnectionPage.Dispose();
            secondConnectionPage.Dispose();
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

        
    }
}
