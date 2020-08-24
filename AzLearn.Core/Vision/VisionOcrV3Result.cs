
using System;

namespace AzLearn.Core.Vision
{
    public class VisionOcrV3Result
    {
        /// <summary>Status moves through: 'notStarted', 'running', 'succeeded', or 'failed'. Or may be a HTTP Status Code </summary>
        public string Status { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime LastUpdatedDateTime { get; set; }
        public AnalyzeResultV3 AnalyzeResult { get; set; }

        /// <summary>Elapsed time of processing</summary>
        public TimeSpan ProcessTime { get; set; }

        /// <summary>Errors occured in processing</summary>
        public string ErrorText { get; set; }
    }

    public class AnalyzeResultV3
    {
        public string Version { get; set; }
        public ReadresultV3[] ReadResults { get; set; }
    }

    public class ReadresultV3
    {
        public int Page { get; set; }
        public string Language { get; set; }
        public decimal Angle { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public string Unit { get; set; }
        public LineV3[] Lines { get; set; }
    }

    public class LineV3
    {
        public decimal[] BoundingBox { get; set; }
        public string Text { get; set; }
        public WordV3[] Words { get; set; }
    }

    public class WordV3
    {
        public decimal[] BoundingBox { get; set; }
        public string Text { get; set; }
        public decimal Confidence { get; set; }
    }

}
