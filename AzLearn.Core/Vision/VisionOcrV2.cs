using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace AzLearn.Core.Vision
{
    public class VisionOcrV2 : IVisionOcr<VisionOcrV2Result>
    {
        private VisionSettings Settings { get; }

        public VisionOcrV2(VisionSettings settings)
        {
            Settings = settings;
        }

        public async Task<VisionOcrV2Result> GetImageOCR(byte[] imageBytes, string language = "unk", bool detectOrientation = true)
        {
            if (string.IsNullOrWhiteSpace(language)) throw new ArgumentNullException(nameof(language));

            HttpClient client = new HttpClient();

            // Request headers.
            client.DefaultRequestHeaders.Add(
                "Ocp-Apim-Subscription-Key", Settings.VisionSubscriptionKey);

            // language: unk=Unknown (detect automatically), en=English
            // detectOrientation: true allows vision api to detects and correct text orientation before detecting text.
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["language"] = language; //The vision/v3.0/ocr allows Unknown language
            queryString["detectOrientation"] = detectOrientation.ToString();

            // Assemble the URI for the REST API method.
            string uri = $"{Settings.VisionOcrEndpoint}?{queryString}";

            HttpResponseMessage response;

            // Add the byte array as an octet stream to the request body.
            using (ByteArrayContent content = new ByteArrayContent(imageBytes))
            {
                // This example uses the "application/octet-stream" content type.
                // The other content types you can use are "application/json"
                // and "multipart/form-data".
                content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/octet-stream");

                // Asynchronously call the REST API method.
                response = await client.PostAsync(uri, content);
            }

            string contentString = await response.Content.ReadAsStringAsync();
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<VisionOcrV2Result>(contentString);
            return result;
        }

        /// <summary>Gets the text visible in the specified image file by using the Computer Vision REST API.</summary>
        /// <param name="imageFilePath">The image file with printed text.</param>
        public async Task<VisionOcrV2Result> GetImageOCR(string imageFilePath, string language = "unk", bool detectOrientation = true)
        {
            if (string.IsNullOrWhiteSpace(imageFilePath)) throw new ArgumentNullException(nameof(imageFilePath));

            if (Path.GetExtension(imageFilePath).Equals(".pdf", StringComparison.OrdinalIgnoreCase)) throw new ArgumentOutOfRangeException("PDF file extension not suppported");

            if (string.IsNullOrWhiteSpace(language)) throw new ArgumentNullException(nameof(language));

            byte[] imageBytes = await ReadImageAsync(imageFilePath);
            var result = await GetImageOCR(imageBytes, language, detectOrientation);
            return result;
        }

        private static async Task<byte[]> ReadImageAsync(string imageFilePath) => await VisionOcrV3.ReadImageAsync(imageFilePath);
    }
}
