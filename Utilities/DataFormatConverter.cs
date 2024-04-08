using ETCRegionManagementSimulator.Collections;
using ETCRegionManagementSimulator.Interfaces;
using ETCRegionManagementSimulator.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace ETCRegionManagementSimulator.Utilities
{
    public static class DataFormatConverter
    {
        public static List<ByteRowModel> ConvertToDisplayableList(ETCDataFormatCollection<IDataFormat> collection)
        {
            List<ByteRowModel> displayableList = new List<ByteRowModel>();

            foreach (IDataFormat item in collection.GetAll())
            {
                byte[] bytes = item.ToBytes();
                ByteRowModel rowModel = new ByteRowModel { Bytes = new Dictionary<int, byte>() };
                for (int i = 0; i < bytes.Length; i++)
                {
                    rowModel.Bytes[i] = bytes[i];
                }
                displayableList.Add(rowModel);
            }

            return displayableList;

        }

        // Adaptor for ExcelRow
        public static List<DisplayModel> ConvertExcelRowToDisplayableList(ExcelRow row)
        {
            List<DisplayModel> displayList = new List<DisplayModel>
            {
                new DisplayModel
                {
                    FrameDataNo = row.FrameDataNo,
                    FrameDataTitle = row.FrameDataTitle,
                    FrameDataLength = row.FrameDataLength,
                    FrameCommonHeaderSummary = ConvertToDisplayableString(row.FrameCommonHeader),
                    FrameContentSummary = ConvertToDisplayableString(row.FrameContent),
                    FullFrameDataSummary = ConvertToDisplayableString(row.FrameData)
                }
            };

            return displayList;
        }

        public static byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
            {
                return null;
            }
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static byte[] ObjectToJsonByteArray(object obj)
        {
            string jsonString = JsonConvert.SerializeObject(obj); // Or use JsonSerializer.Serialize(obj) in System.Text.Json
            return Encoding.UTF8.GetBytes(jsonString);
        }
        public static string ToHexString(byte[] bytes)
        {
            // From byte[] to readable string for display purpose
            return BitConverter.ToString(bytes).Replace("-", " ");
        }

        public static string ConvertToDisplayableString(ETCDataFormatCollection<IDataFormat> collection)
        {
            return ConvertToDisplayableString(collection.GetAll());
        }

        public static string ConvertToDisplayableString(IEnumerable<IDataFormat> enumerable)
        {
            //List<string> readableByteArray = new List<string>();
            //foreach (ETCDataFormat item in enumerable)
            //{
            //    byte[] bytes = item.ToBytes();
            //    readableByteArray.Add(ToHexString(bytes));
            //}
            //string result = string.Join(" ", readableByteArray);
            //return result;
            if (!enumerable.Any())
            {
                return ""; // TODO: maybe return some default message indicating empty data
            }

            return enumerable
                .Select(item => ToHexString(item.ToBytes()))
                .Aggregate((acc, next) => $"{acc}, {next}");
        }

        private static string SummarizeDataFormatCollection(ETCDataFormatCollection<IDataFormat> collection)
        {
            // Example summarization: simply return the count of items
            return $"Items: {collection.Count}";
        }
        private static string SummarizeDataFormatEnumerable(IEnumerable<IDataFormat> dataFormats)
        {
            // Example summarization: return a combined byte length
            int totalBytes = dataFormats.Sum(df => df.ToBytes().Length);
            return $"Total Bytes: {totalBytes}";
        }

    }
}
