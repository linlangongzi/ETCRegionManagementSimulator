using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETCRegionManagementSimulator.Models
{
    public class ByteRowModel
    {
        // using a dynamic approach where each row can adapt to the length of the byte array it represents. 
        public Dictionary<int, byte> Bytes { get; set; } = new Dictionary<int, byte>();
    }
}
