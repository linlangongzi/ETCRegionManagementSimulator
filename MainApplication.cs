using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ETCRegionManagementSimulator
{
    class MainApplication:IDisposable
    {
        private Window mainWindow=null;

        private MainPage mainPage;


        private bool disposedValue;
        public MainApplication() 
        {
            mainPage=new MainPage();
        }

        public async void StartApplication()
        {
            if (mainWindow == null)
            {
                if (mainPage != null)
                {
                    int currentViewId = ApplicationView.GetForCurrentView().Id;
                    await CoreApplication.CreateNewView().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        mainWindow = Window.Current;
                        var mainView = ApplicationView.GetForCurrentView();
                        mainView.Consolidated += MainView_Consolidated;
                        mainWindow.Content = mainPage;
                        mainWindow.Activate();
                        await ApplicationViewSwitcher.TryShowAsStandaloneAsync(ApplicationView.GetApplicationViewIdForWindow(Window.Current.CoreWindow), ViewSizePreference.Default, currentViewId, ViewSizePreference.Default);
                    });
                }
            }
        }
        private void MainView_Consolidated(ApplicationView sender, ApplicationViewConsolidatedEventArgs args)
        {
            sender.Consolidated -= MainView_Consolidated;
            mainWindow = null;
        }


        private protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: マネージド状態を破棄します (マネージド オブジェクト)
                }

                // TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、ファイナライザーをオーバーライドします
                // TODO: 大きなフィールドを null に設定します
                disposedValue = true;
            }
        }

         // TODO: 'Dispose(bool disposing)' にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします
         ~MainApplication()
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
