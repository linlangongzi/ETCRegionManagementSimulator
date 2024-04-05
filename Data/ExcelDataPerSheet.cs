using ETCRegionManagementSimulator.Collections;
using ETCRegionManagementSimulator.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace ETCRegionManagementSimulator
{
    public struct ExcelRow
    {
        public int FrameDataNo { get; set; }
        public string FrameDataTitle { get; set; }
        public int FrameDataLength { get; set; }
        // Data Header Only
        public ETCDataFormatCollection<IDataFormat> FrameCommonHeader { get; set; }
        // Data content after Header but without Header
        public ETCDataFormatCollection<IDataFormat> FrameContent { get; set; }
        // Full Data that contains Header and actual content
        public IEnumerable<IDataFormat> FrameData { get; set; }

        public ExcelRow(int frameNo, string frameTitle, int frameLength,
            ETCDataFormatCollection<IDataFormat> frameHeader,
            ETCDataFormatCollection<IDataFormat> frameContent)
        {
            FrameDataNo = frameNo;
            FrameDataTitle = frameTitle;
            FrameDataLength = frameLength;
            FrameCommonHeader = frameHeader;
            FrameContent = frameContent;
            // Combine Header and Content as a whole inplace without extra memory consumption by using LINQ
            FrameData = FrameCommonHeader.GetAll().Concat(FrameContent.GetAll());
        }
    }
    public class ExcelDataPerSheet
    {
        public List<ExcelRow> DataPerSheet { get; set; }

        // TODO: Add more data process method ;
        // TODO: Consider use built-in DataTable Collection type to represent ExcelData in the future version 
    }
}
