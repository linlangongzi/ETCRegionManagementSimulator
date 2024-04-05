using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETCRegionManagementSimulator.Events
{
    public class SheetSelectedEventArgs : EventArgs
    {
        public string SheetName { get; private set; }
        public SheetSelectedEventArgs(string sheetName) => SheetName = sheetName;
    }
}
