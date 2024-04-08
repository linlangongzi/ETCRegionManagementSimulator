using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETCRegionManagementSimulator.Events
{
    public class DataChangedEventArgs : EventArgs
    {
        public enum ChangeType
        {
            Loaded,
            Added,
            Updated,
            Removed
        }

        public ChangeType TypeOfChange { get; private set; }

        // Include information about the data affected by the change
        // the ID of an ExcelRow that was added, updated, or removed
        public int AffectedDataId { get; private set; }
        public DataChangedEventArgs(ChangeType changeType) => TypeOfChange = changeType;
        public DataChangedEventArgs(ChangeType changeType, int affectedDataId)
        {
            TypeOfChange = changeType;
            AffectedDataId = affectedDataId;
        }
    }
}
