using ETCRegionManagementSimulator.Events;
using ETCRegionManagementSimulator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETCRegionManagementSimulator.Models
{
    public class ExcelDataModel : IDataModel
    {
        private List<ExcelRow> _data = new List<ExcelRow>();

        public event EventHandler<DataChangedEventArgs> DataChanged;

        public void AddData(ExcelRow data)
        {
            _data.Add(data);
            OnDataChanged(new DataChangedEventArgs(DataChangedEventArgs.ChangeType.Added));
        }

        public ExcelRow GetDataById(int id)
        {
            return _data.FirstOrDefault(d => d.FrameDataNo == id);
        }

        public void LoadData(IEnumerable<ExcelRow> data)
        {
            _data.Clear();
            _data.AddRange(data);
            OnDataChanged(new DataChangedEventArgs(DataChangedEventArgs.ChangeType.Loaded)); ;
        }

        public void RemoveData(int id)
        {
            _ = _data.RemoveAll(d => d.FrameDataNo == id);
            OnDataChanged(new DataChangedEventArgs(DataChangedEventArgs.ChangeType.Removed));
        }

        public void UpdateData(ExcelRow data)
        {
            /// TODO: Implementation for updating data
            OnDataChanged(new DataChangedEventArgs(DataChangedEventArgs.ChangeType.Updated));
        }

        protected virtual void OnDataChanged(DataChangedEventArgs e)
        {
            DataChanged?.Invoke(this, e);
        }
    }
}
