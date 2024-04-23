using ETCRegionManagementSimulator.Interfaces;
using ETCRegionManagementSimulator.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ETCRegionManagementSimulator.Views
{
    public sealed partial class StandardPage : Page, IClientPage, IDisposable
    {
        public ObservableCollection<MessageViewModel> SourceMessages { get; } = new ObservableCollection<MessageViewModel>();
        /// TODO: Delete the TestMessagesCollection
        public ObservableCollection<string> TestMessagesCollection { get; } = new ObservableCollection<string>();
        public string ClientPageId { get;  set; }


        public StandardPage()
        {
            this.InitializeComponent();
            this.DataContext = this;
            SourceMessages.Add(new MessageViewModel() { Message = ClientPageId + "Title" });
        }

        public void UpdateMessageView(string message)
        {
            var _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                SourceMessages.Add(new MessageViewModel() { Message = message });
                foreach (var m in SourceMessages)
                {
                    Debug.WriteLine($"------Each Element {m.Message}  ---- \n");
                }
                TestMessagesCollection.Add(message);
                //MessagesTextBlock.Text += $"{message}\n";
                    Debug.WriteLine($"Updating {message} to MessageView on : {ClientPageId}");

                if (MessageView.Items?.Any() == true)
                {
                    var lastIndex = MessageView.Items.Count - 1;
                    MessageView.ScrollIntoView(MessageView.Items[lastIndex]);
                }
            });
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            //if (e.Parameter is IClientPage clientPage)
            //{
            //    // Initialize this page based on the passed clientPage instance
            //    // This might involve setting data context, loading data, etc.
            //    this.DataContext = clientPage;
            //    ClientPageId = clientPage.ClientPageId;
            //    SourceMessages.Add(new MessageViewModel() { Message = ClientPageId });
            //}
            // Initialize or refresh your UI here if necessary
            // For instance, you might want to clear previous messages in certain scenarios
            // SourceMessages.Clear();
            // MessagesTextBlock.Text = string.Empty;
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {

            }
            /// TODO: Unregister the EventHandler
            SourceMessages.Clear();
        }
        ~StandardPage()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
