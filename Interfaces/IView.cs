using ETCRegionManagementSimulator.Controllers;
using ETCRegionManagementSimulator.Events;
using System;

namespace ETCRegionManagementSimulator.Interfaces
{
    public interface IView
    {
        event EventHandler<SheetSelectedEventArgs> SheetSelected;
        void UpdateView();
        void SetController(IController controller);
    }
}
