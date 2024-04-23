using ETCRegionManagementSimulator.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace ETCRegionManagementSimulator.ViewModels
{
    public class ClientViewModel : INotifyPropertyChanged
    {
        private ClientView _clientView;

        private ObservableCollection<string> _messages = new ObservableCollection<string>();

        public ClientView ClientView
        {
            get => _clientView;
            set
            {
                _clientView = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Messages
        {
            get => _messages;
            set
            {
                _messages = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void UpdateClientInfo(string info)
        {
            // Update logic here
            OnPropertyChanged(nameof(ClientView));
        }

        public void ReceiveMessage(string message)
        {
            Messages.Add(message);
            Debug.WriteLine($">>>>>>>>> {message} >>>>>>>");
        }
    }
}
