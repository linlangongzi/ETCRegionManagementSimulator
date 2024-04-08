using ETCRegionManagementSimulator.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETCRegionManagementSimulator.Interfaces
{
    public interface IDataModel
    {
        //  For notifying about data changes
        event EventHandler<DataChangedEventArgs> DataChanged;

        void LoadData(IEnumerable<ExcelRow> data);
        ExcelRow GetDataPerRowById(int id);

        IEnumerable<ExcelRow> GetAllData();
        void AddData(ExcelRow data);
        void UpdateData(ExcelRow data);
        void RemoveData(int id);
    }
}
