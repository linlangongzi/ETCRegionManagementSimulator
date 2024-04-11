using ETCRegionManagementSimulator.DataFormats;
using ETCRegionManagementSimulator.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ETCRegionManagementSimulator
{
    public enum DataFormat
    {
        X1,
        BCD,
        BIN,
        HEX
    }
    public class Binary : DataFormatBase
    {
        private readonly object data;
        private readonly IConversionStrategy conversionStrategy;
        public Binary(object data) : base(data)
        {
            this.data = data;

            switch (data)
            {
                case int _:
                    conversionStrategy = new IntConversionStrategy();
                    break;
                case uint _:
                    conversionStrategy = new UintConversionStrategy();
                    break;
                default:
                    conversionStrategy = new BinaryStringConversionStrategy();
                    break;
            }
        }

        public override byte[] ToBytes()
        {
            return conversionStrategy.ConvertToBytes(data);
        }

        public override string ToString()
        {
            return data.ToString();
        }

    }

    public class BCD : DataFormatBase
    {
        private readonly object data;
        private readonly IConversionStrategy conversionStrategy;

        public BCD(object data) : base(data)
        {
            this.data = data;

            switch (data)
            {
                case int _:
                    conversionStrategy = new IntConversionStrategy();
                    break;
                case uint _:
                    conversionStrategy = new UintConversionStrategy();
                    break;
                default:
                    conversionStrategy = new BCDStringConversionStrategy();
                    break;
            }

        }

        public override byte[] ToBytes()
        {
            return conversionStrategy.ConvertToBytes(data);
        }

        //This method assumes that each byte in data contains two decimal digits:
        //one in the high nibble and one in the low nibble.
        //It extracts these digits and converts them to their decimal string representation
        public string ToDecimalString()
        {
            byte[] bytes = ToBytes();
            return string.Concat(bytes.SelectMany(b => new[] { (b >> 4) & 0x0F, b & 0x0F })
                                      .Select(n => n.ToString()));
        }

        public override string ToString()
        {
            return ToDecimalString();
        }
    }

    public class Hex : DataFormatBase
    {
        private readonly object data;
        private readonly IConversionStrategy conversionStrategy;

        public Hex(object data) : base(data)
        {
            this.data = data;
            conversionStrategy = new HexStringConversionStrategy();
        }

        public override byte[] ToBytes()
        {
            return conversionStrategy.ConvertToBytes(data);
        }
        //converts the byte array to a string of hexadecimal pairs
        public string ToHexString()
        {
            byte[] bytes = ToBytes();
            return string.Concat(bytes.Select(b => b.ToString("X2")));
        }

        public override string ToString()
        {
            return ToHexString();
        }
    }
}
