using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace ETCRegionManagementSimulator.Interfaces
{
    public interface IStandardPageFactory
    {
        Page CreateStandardPage(object parameter);
    }
}
