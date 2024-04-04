using ETCRegionManagementSimulator.Interfaces;

namespace ETCRegionManagementSimulator
{
    public class BCD : ETCDataFormat
    {
        private readonly byte[] data;

        public BCD(byte[] data)
        {
            this.data = data;
        }

        public byte[] ToBytes() => data;
    }

    public class Hex : ETCDataFormat
    {
        private readonly byte[] data;

        public Hex(byte[] data)
        {
            this.data = data;
        }

        public byte[] ToBytes() => data;
    }
}
