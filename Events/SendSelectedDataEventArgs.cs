using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETCRegionManagementSimulator.Events
{
    public class SendSelectedDataEventArgs : EventArgs
    {
        public string SelectedData { get; }

        public int SelectedRowIndex { get; }
        public SendSelectedDataEventArgs(string selectedData)
        {
            SelectedData = selectedData;
        }
        public SendSelectedDataEventArgs(int selectedIndex)
        {
            SelectedRowIndex = selectedIndex;
        }
        public SendSelectedDataEventArgs(int selectedIndex, string selectedData)
        {
            SelectedRowIndex = selectedIndex;
            SelectedData = selectedData;
        }
    }
}
