using System;
using System.Collections.Generic;
using System.Text;

namespace AzLearn.Core.Vision
{
    public class VisionSettings
    {
        public string VisionSubscriptionKey { get; set; }
        public string VisionEndPoint { get; set; }



        /// <summary>
        ///     Useful for Printed text, but not for handwritten text.
        ///     Language can be provided, or can be set to Unknown (unk).
        ///     Syncronous POST returns the results.
        /// </summary>
        public string VisionOcrEndpoint =>
            string.IsNullOrEmpty(VisionEndPoint) ?
            null :
            VisionEndPoint.EndsWith("/") ?
            $"{VisionEndPoint}{VisionOcrPath}" :
            $"{VisionEndPoint}/{VisionOcrPath}";

        /// <summary>
        ///     Useful for Handwritten and Printed text.  
        ///     Language must be provided.  English (en) will be used as default.
        ///     Asyncronoous Processing:
        ///         POST returns a Result URL in the headers.
        ///         Get of Result URL (after a delay) will return the results.
        /// </summary>
        public string VisionOcrReadAnalyzeEndpoint =>
            string.IsNullOrEmpty(VisionEndPoint) ?
            null :
            VisionEndPoint.EndsWith("/") ?
            $"{VisionEndPoint}{VisionReadAnalyzePath}" :
            $"{VisionEndPoint}/{VisionReadAnalyzePath}";

        //private const string VisionOcrPath_2_1 = "vision/v2.1/ocr";
        private const string VisionOcrPath= "vision/v3.0/ocr";
        private const string VisionReadAnalyzePath = "vision/v3.0/read/analyze";

    }
}
