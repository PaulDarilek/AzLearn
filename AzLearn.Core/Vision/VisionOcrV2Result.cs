using System;
using System.Collections.Generic;
using System.Text;

namespace AzLearn.Core.Vision
{
    /// <summary>Result format</summary>
    /// <remarks>Documentation <see cref="https://westcentralus.dev.cognitive.microsoft.com/docs/services/5adf991815e1060e6355ad44/operations/56f91f2e778daf14a499e1fc"/></remarks>
    public class VisionOcrV2Result
    {
        public string Language { get; set; }
        public float TextAngle { get; set; }
        public string Orientation { get; set; }
        public RegionV2[] Regions { get; set; }
    }

    public class RegionV2
    {
        public string BoundingBox { get; set; }
        public LineV2[] Lines { get; set; }
    }

    public class LineV2
    {
        public string BoundingBox { get; set; }
        public WordV2[] Words { get; set; }
    }

    public class WordV2
    {
        public string BoundingBox { get; set; }
        public string Text { get; set; }
    }

}
