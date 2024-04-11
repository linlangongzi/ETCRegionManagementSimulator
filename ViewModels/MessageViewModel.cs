using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ETCRegionManagementSimulator.ViewModels
{
    //public class MessageViewModel : INotifyPropertyChanged
    //{
    //    private string _message;
    //    public string Message 
    //    {
    //        get => _message;
    //        set
    //        {
    //            _message = value;
    //            OnPropertyChanged();
    //        }
    //    }

    //    public event PropertyChangedEventHandler PropertyChanged;

    //    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    //    {
    //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    //    }
    //}

    public class MessageViewModel
    {
        public string Message { get; set; }
    }
}
