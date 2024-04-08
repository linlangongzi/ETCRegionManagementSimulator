using ETCRegionManagementSimulator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ETCRegionManagementSimulator
{

    public class Binary : IDataFormat
    {
        private readonly uint data;

        public Binary(uint data)
        {
            this.data = data;
        }

        // ensures the byte[] is in big-endian order regardless of the system's native byte order
        public byte[] ToBytes()
        {
            byte[] bytes = BitConverter.GetBytes(data);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return bytes;
        }
        public string ToBinaryString()
        {
            // Convert to a binary string and pad with leading zeros for a full 32-bit representation
            return Convert.ToString(data, 2).PadLeft(32, '0');
        }
        public override string ToString()
        {
            return data.ToString();
        }
    }

    public class BCD : IDataFormat
    {
        private readonly byte[] data;

        public BCD(byte[] data)
        {
            this.data = data;
        }

        public BCD(int number)
        {
            byte[] digits = number.ToString().Select(digit => Convert.ToByte(digit.ToString())).ToArray();
            List<byte> bcdList = new List<byte>();

            // pack two BCD digits into each byte by default,
            // where the first digit occupies the high nibble and the second digit the low nibble.
            for (int i = 0; i < digits.Length; i += 2)
            {
                byte high = i < digits.Length ? (byte)(digits[i] << 4) : (byte)0;
                byte low = (i + 1) < digits.Length ? digits[i + 1] : (byte)0;
                bcdList.Add((byte)(high | low));
            }

            this.data = bcdList.ToArray();
        }

        public byte[] ToBytes() => data;

        //This method assumes that each byte in data contains two decimal digits:
        //one in the high nibble and one in the low nibble.
        //It extracts these digits and converts them to their decimal string representation
        public string ToDecimalString()
        {
            return string.Concat(data.SelectMany(b => new[] { (b >> 4) & 0x0F, b & 0x0F })
                                      .Select(n => n.ToString()));
        }

        public override string ToString()
        {
            return ToDecimalString();
        }
    }

    public class Hex : IDataFormat
    {
        private readonly byte[] data;

        public Hex(byte[] data)
        {
            this.data = data;
        }

        public byte[] ToBytes() => data;

        //converts the byte array to a string of hexadecimal pairs
        public string ToHexString() => string.Concat(data.Select(b => b.ToString("X2")));

        public override string ToString()
        {
            return ToHexString();
        }
    }
}
