using ETCRegionManagementSimulator.Collections;
using ETCRegionManagementSimulator.Interfaces;
using ETCRegionManagementSimulator.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ETCRegionManagementSimulator.Utilities
{
    public static class DataFormatConverter
    {
        public static List<ByteRowModel> ConvertToDisplayableList(ETCDataFormatCollection<ETCDataFormat> collection)
        {
            List<ByteRowModel> displayableList = new List<ByteRowModel>();

            foreach (ETCDataFormat item in collection.GetAll())
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

        // Adaptation for ExcelRow
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

        public static string ToHexString(byte[] bytes)
        {
            // From byte[] to readable string for display purpose
            return BitConverter.ToString(bytes).Replace("-", " ");
        }

        public static string ConvertToDisplayableString(ETCDataFormatCollection<ETCDataFormat> collection)
        {
            return ConvertToDisplayableString(collection.GetAll());
        }

        public static string ConvertToDisplayableString(IEnumerable<ETCDataFormat> enumerable)
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

        private static string SummarizeDataFormatCollection(ETCDataFormatCollection<ETCDataFormat> collection)
        {
            // Example summarization: simply return the count of items
            return $"Items: {collection.Count}";
        }
        private static string SummarizeDataFormatEnumerable(IEnumerable<ETCDataFormat> dataFormats)
        {
            // Example summarization: return a combined byte length
            int totalBytes = dataFormats.Sum(df => df.ToBytes().Length);
            return $"Total Bytes: {totalBytes}";
        }

    }
}
