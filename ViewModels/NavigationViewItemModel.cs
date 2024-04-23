using ETCRegionManagementSimulator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace ETCRegionManagementSimulator.Views
{
    public class NavigationViewItemModel : IDisposable
    {

        public int ItemIndex { get; set; }
        public string ClientId { get; set; }
        public StandardClientPage StandardPage { get; set; }
        public NavigationViewItem NavigationItem { get; set; }

        public NavigationViewItemModel(int index, string ClientId, StandardClientPage Page, NavigationViewItem navigationViewItem)
        {
            this.ItemIndex = index;
            this.ClientId = ClientId;
            this.StandardPage = Page;
            this.NavigationItem = navigationViewItem;
        }

        public NavigationViewItemModel(string ClientId, StandardClientPage Page, NavigationViewItem navigationViewItem)
        {
            this.ClientId = ClientId;
            this.StandardPage = Page;
            this.NavigationItem = navigationViewItem;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                (StandardPage as IDisposable)?.Dispose();
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~NavigationViewItemModel()
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
