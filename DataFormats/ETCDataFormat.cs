using ETCRegionManagementSimulator.Interfaces;

namespace ETCRegionManagementSimulator
{
    public class BCD : IDataFormat
    {
        private readonly byte[] data;

        public BCD(byte[] data)
        {
            this.data = data;
        }

        public byte[] ToBytes() => data;
    }

    public class Hex : IDataFormat
    {
        private readonly byte[] data;

        public Hex(byte[] data)
        {
            this.data = data;
        }

        public byte[] ToBytes() => data;
    }
}
