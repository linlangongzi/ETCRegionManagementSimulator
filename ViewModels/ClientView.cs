using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ETCRegionManagementSimulator.Models
{
    public class ClientView : INotifyPropertyChanged
    {
        private string _clientId;
        private string _name;
        public enum ClientViewType
        {
            Standard,
            Special
        }

        private ClientViewType _type;

        public string ClientId
        {
            get => _clientId;
            set
            {
                if (_clientId != value)
                {
                    _clientId = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        public ClientViewType Type
        {
            get => _type;
            set
            {
                if (_type != value)
                {
                    _type = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
