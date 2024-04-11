using ETCRegionManagementSimulator.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace ETCRegionManagementSimulator.DataFormats
{
    public class BCDStringConversionStrategy : IConversionStrategy
    {
        public byte[] ConvertToBytes(object data)
        {
            // Ensure the string length is even by prepending a '0' if necessary.
            if (data != null)
            {
                string bcdString = data.ToString();

                if (!Regex.IsMatch(bcdString, "^[0-9]+$"))
                {
                    Debug.WriteLine("Object does not match BCD pattern format.\n");
                    return new byte[0];
                }

                if (bcdString.Length % 2 != 0)
                {
                    bcdString = "0" + bcdString;
                }

                byte[] byteArray = new byte[bcdString.Length / 2];

                for (int i = 0; i < byteArray.Length; i++)
                {
                    // Extract two characters (one BCD digit pair) at a time.
                    string digitPair = bcdString.Substring(i * 2, 2);

                    // Convert the digit pair to a byte where:
                    // - The first digit is in the higher nibble (left 4 bits)
                    // - The second digit is in the lower nibble (right 4 bits)
                    try
                    {
                        byteArray[i] = Convert.ToByte(Convert.ToInt32(digitPair, 16));
                    }
                    catch (FormatException)
                    {
                        // Log or handle the parsing error
                        Debug.WriteLine($"Invalid BCD digit pair: {digitPair}. Using 0x00 as a fallback.\n");
                        byteArray[i] = 0x00; // Default/fallback value
                    }
                }

                return byteArray;
            }
            else
            {
                Debug.WriteLine("Object to be converted is null. \n return empty byte[]\n");
                return new byte[0];
            }
        }
    }

    public class BinaryStringConversionStrategy : IConversionStrategy
    {
        public byte[] ConvertToBytes(object data)
        {
            if (data != null)
            {
                string binaryString = data.ToString();
                // Normalize the string length to be a multiple of 8
                binaryString = binaryString.PadLeft((binaryString.Length + 7) / 8 * 8, '0');

                var byteArray = new List<byte>();

                for (int i = 0; i < binaryString.Length; i += 8)
                {
                    // Take 8 characters at a time
                    string byteString = binaryString.Substring(i, 8);

                    // Convert the 8-bit string chunk to a byte
                    byte b = Convert.ToByte(byteString, 2);
                    byteArray.Add(b);
                }

                return byteArray.ToArray();
            }
            else
            {
                Debug.WriteLine("Object to be converted is null. \n return empty byte[]\n");
                return new byte[0];
            }
        }
    }

    public class HexStringConversionStrategy : IConversionStrategy
    {
        public byte[] ConvertToBytes(object data)
        {
            if (data == null)
            {
                Debug.WriteLine("Object to be converted is null. \n return empty byte[]\n");
                return new byte[0];
            }

            string hexStr = data.ToString();
            // Ensure the string has an even length, if it is not then prefix it with a zero
            if (hexStr.Length % 2 != 0)
            {
                hexStr = "0" + hexStr;
            }

            return Enumerable.Range(0, hexStr.Length / 2)
                             .Select(x => Convert.ToByte(hexStr.Substring(x * 2, 2), 16))
                             .ToArray();
        }
    }

    public class IntConversionStrategy : IConversionStrategy
    {
        public byte[] ConvertToBytes(object data)
        {
            return data is int intValue ? BitConverter.GetBytes(intValue) : (new byte[0]);
        }
    }

    public class UintConversionStrategy : IConversionStrategy
    {
        public byte[] ConvertToBytes(object data)
        {
            return data is uint uintValue ? BitConverter.GetBytes(uintValue) : (new byte[0]);
        }
    }
}
