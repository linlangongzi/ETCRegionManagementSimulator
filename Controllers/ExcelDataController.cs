using ETCRegionManagementSimulator.Events;
using ETCRegionManagementSimulator.Interfaces;
using System;
using System.Collections.Generic;

namespace ETCRegionManagementSimulator.Controllers
{
    public class ExcelDataController : IController, IDisposable
    {
        private readonly IDataModel _model;
        private readonly IView _view;
        private readonly IExcelService _excelService;

        private bool disposedValue;

        public ExcelDataController(IDataModel model, IView view, IExcelService excelService)
        {
            _model = model;
            _view = view;
            _excelService = excelService;
            _view.SetController(this);
            _view.SheetSelected += View_SheetSeleted;
        }
        private void View_SheetSeleted(object sender, SheetSelectedEventArgs e)
        {
            // Fetch data from the model for e.SheetName and update the view
            // Use ExcelService to load data from the selected sheet
            LoadDataFromSheet(e.SheetName);
        }

        public void LoadDataFromSheet(string sheetName)
        {
            // Implementation to load data from the specified sheet
            IEnumerable<ExcelRow> data = _excelService.ReadSheet(sheetName);
            _model.LoadData(data);
            _view.UpdateView();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _view.SheetSelected -= View_SheetSeleted;
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
