using ETCRegionManagementSimulator.Collections;
using ETCRegionManagementSimulator.Interfaces;
using ETCRegionManagementSimulator.Models;
using System.Collections.Generic;
using System.Linq;

namespace ETCRegionManagementSimulator.Utilities
{
    // Adaptor for ExcelRow to Display Purpose
    public static class ExcelDisplayAdaptor
    {
        public static List<DisplayModel> ConvertToDisplayableList(ExcelRow row)
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

        public static string ConvertToDisplayableString(ETCDataFormatCollection<IDataFormat> collection)
        {
            return string.Join(", ", collection.GetAll().Select(item => item.ToString()));
        }

        public static string ConvertToDisplayableString(IEnumerable<IDataFormat> enumerable)
        {
            return string.Join(", ", enumerable.Select(item => item.ToString()));
        }

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
    }

}