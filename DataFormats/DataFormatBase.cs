using ETCRegionManagementSimulator.Interfaces;

namespace ETCRegionManagementSimulator.DataFormats
{
    public abstract class DataFormatBase : IDataFormat
    {
        public abstract byte[] ToBytes();

        protected DataFormatBase(object data) { }
    }
}
