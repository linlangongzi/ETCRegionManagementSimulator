using ETCRegionManagementSimulator.Interfaces;
using ETCRegionManagementSimulator.ViewModels;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ETCRegionManagementSimulator.Views
{
    public sealed partial class StandardPage : Page, IClientPage
    {
        public ObservableCollection<MessageViewModel> SourceMessages { get; } = new ObservableCollection<MessageViewModel>();

        public ObservableCollection<string> TestMessagesCollection { get; } = new ObservableCollection<string>();
        public string ClientPageId { get;  set; }

        private static int i = 0;

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
                Debug.WriteLine($"------how many time we been here : { i++ }  ---- \n");
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

            if (e.Parameter is IClientPage clientPage)
            {
                // Initialize this page based on the passed clientPage instance
                // This might involve setting data context, loading data, etc.
                this.DataContext = clientPage;
                ClientPageId = clientPage.ClientPageId;
            }

            // Example of using navigation parameters
            if (e.Parameter is string clientId)
            {
                // Do something with the parameter, e.g., initialize your page or load data
                // This could be a client ID, a specific message to display initially, etc.
                StandardPage thisPage = (StandardPage)ClientPageFactory.Instance.GetPageById(clientId);
                this.DataContext = thisPage;
                ClientPageId = clientId;
                SourceMessages.Add(new MessageViewModel() { Message = clientId });
            }
            // Initialize or refresh your UI here if necessary
            // For instance, you might want to clear previous messages in certain scenarios
            // SourceMessages.Clear();
            // MessagesTextBlock.Text = string.Empty;
        }
    }
}
