using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETCRegionManagementSimulator.Controllers
{
    public interface IController
    {
        void LoadDataFromSheet(string sheetName);

        /// TODO: add more control interfaces
    }
}
