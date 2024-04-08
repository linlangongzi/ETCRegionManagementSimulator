namespace ETCRegionManagementSimulator.Models
{
    public class DisplayModel
    {
        public int FrameDataNo { get; set; }
        public string FrameDataTitle { get; set; }
        public int FrameDataLength { get; set; }
        public string FrameCommonHeaderSummary { get; set; } // Hex string
        public string FrameContentSummary { get; set; } // Hex string
        public string FullFrameDataSummary { get; set; } // Hex string
    }
}
