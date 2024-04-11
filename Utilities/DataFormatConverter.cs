using Newtonsoft.Json;
using System;
using System.Text;

namespace ETCRegionManagementSimulator.Utilities
{
    public static class DataFormatConverter
    {
        public static byte[] ObjectToJsonByteArray(object obj)
        {
            string jsonString = JsonConvert.SerializeObject(obj);
            // Or use JsonSerializer.Serialize(obj) in System.Text.Json
            return Encoding.UTF8.GetBytes(jsonString);
        }

        public static string ToHexString(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", " ");
        }
    }
}
