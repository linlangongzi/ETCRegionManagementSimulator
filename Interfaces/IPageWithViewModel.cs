using ETCRegionManagementSimulator.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETCRegionManagementSimulator.Interfaces
{
    public interface IPageWithViewModel
    {
        void SetViewModel(ClientViewModel viewModel);
    }
}
