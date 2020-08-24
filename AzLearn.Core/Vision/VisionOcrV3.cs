using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace AzLearn.Core.Vision
{
    // Adapted from: https://docs.microsoft.com/en-us/azure/cognitive-services/computer-vision/quickstarts/csharp-print-text
    // Adapted from: https://docs.microsoft.com/en-us/azure/cognitive-services/computer-vision/quickstarts/csharp-hand-text
    // See also: https://docs.microsoft.com/en-us/azure/cognitive-services/computer-vision/tutorials/csharptutorial

    /// <summary>Performs Optical Character Recognition (OCR) from an Image, including handwritten text</summary>
    /// <remarks>Uses the Azure Cognitive Computer Vision REST api</remarks>
    public class VisionOcrV3 : IVisionOcr<VisionOcrV3Result>
    {
        private VisionSettings Settings { get; }

        /// <summary>Constructor</summary>
        public VisionOcrV3(VisionSettings settings)
        {
            Settings = settings;
        }

        /// <summary>Gets the text visible in the image file bytes by using the Computer Vision REST API.</summary>
        public async Task<VisionOcrV3Result> GetImageOCR(byte[] imageBytes, string language = "en", bool detectOrientation = true)
        {
            if (string.IsNullOrWhiteSpace(language)) throw new ArgumentNullException(nameof(language));
            if ("unk" == language.ToLower()) throw new ArgumentOutOfRangeException($"Language '{language}' is invalid for written OCR");

            VisionOcrV3Result result = null;
            var timer = Stopwatch.StartNew();
            try
            {
                HttpClient client = new HttpClient();

                // Request headers.
                client.DefaultRequestHeaders.Add(
                    "Ocp-Apim-Subscription-Key", Settings.VisionSubscriptionKey);

                // language: unk=Unknown (detect automatically), en=English
                // detectOrientation: true allows vision api to detects and correct text orientation before detecting text.
                var queryString = HttpUtility.ParseQueryString(string.Empty);
                queryString["language"] = language; //Path /vision/v3.0/read/analyze requires a language / not unknown.
                queryString["detectOrientation"] = detectOrientation.ToString();

                // Assemble the URI for the REST API method.
                string uri = $"{Settings.VisionOcrReadAnalyzeEndpoint}?{queryString}";

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

                // The response header for the Batch Read method contains the URI
                // of the second method, Read Operation Result, which
                // returns the results of the process in the response body.
                // The Batch Read operation does not return anything in the response body.
                if (response.IsSuccessStatusCode)
                {
                    // If the first REST API method completes successfully, the second REST API method retrieves the text written in the image.
                    // Operation-Location stores the URI of the second REST API method, returned by the first REST API method.
                    // format will be: https://{endpoint}/vision/v3.0/read/analyzeResults/{operationId}
                    uri = response.Headers.GetValues("Operation-Location").FirstOrDefault();
                }
                else
                {
                    // Display the JSON error data.
                    string contentString = await response.Content.ReadAsStringAsync();
                    result = new VisionOcrV3Result { Status = response.StatusCode.ToString(), ErrorText = contentString };
                    return result;
                }

                // Note: The response may not be immediately available. 
                // Text recognition is an asynchronous operation that can take a variable amount of time depending on the length of the text.
                // The GET operation may need to be retried.
                int delayMilliseconds = PollingSchedule.GetDelayMilliseconds(timer.Elapsed);
                bool isFinal = false;
                while (!isFinal && timer.Elapsed < PollingSchedule.Timeout)
                {
                    await Task.Delay(delayMilliseconds);
                    response = await client.GetAsync(uri);

                    // set the next delay.
                    delayMilliseconds = PollingSchedule.GetDelayMilliseconds(timer.Elapsed);

                    if (response.IsSuccessStatusCode)
                    {
                        string contentString = await response.Content.ReadAsStringAsync();
                        result = Newtonsoft.Json.JsonConvert.DeserializeObject<VisionOcrV3Result>(contentString); 
                        switch (result.Status)
                        {
                            case "succeeded":   //  succeeded: The operation has succeeded.
                                isFinal = true;
                                break;

                            case "running":     //  running: The operation is being processed.
                                                // wait the scheduled time.
                                break;

                            case "notStarted":  //  notStarted: The operation has not started.
                                                // wait a multiplier of the scheduled time.
                                delayMilliseconds *= 3;
                                break;

                            case "failed":      //  failed: The operation has failed.
                                isFinal = true;
                                result.ErrorText = "AnalyzeResult failed.";
                                break;

                            default:
                                isFinal = true;
                                result.ErrorText = "Unknown Status";
                                break;
                        }
                    }
                    else
                    {
                        isFinal = true;
                        // Display the JSON error data.
                        string contentString = await response.Content.ReadAsStringAsync();
                        result = new VisionOcrV3Result { Status = response.StatusCode.ToString(), ErrorText = contentString };
                    }

                }
            }
            finally
            {
                if (result != null)
                {
                    // Total Time for request.
                    result.ProcessTime = timer.Elapsed;

                    if (result.ProcessTime > PollingSchedule.Timeout)
                    {
                        var msg = $"Timeout: {PollingSchedule.Timeout: HH:mm:ss}";
                        result.ErrorText =
                            string.IsNullOrWhiteSpace(result.ErrorText) ?
                            msg :
                            result.ErrorText + "\r" + msg;
                    }
                }
            }
            return result;
        }

        /// <summary>Gets the text visible in the specified image file by using the Computer Vision REST API.</summary>
        /// <param name="imageFilePath">The image file with printed text.</param>
        public async Task<VisionOcrV3Result> GetImageOCR(string imageFilePath, string language = "en", bool detectOrientation = true)
        {
            if (string.IsNullOrWhiteSpace(language)) throw new ArgumentNullException(nameof(language));
            if ("unk" == language.ToLower()) throw new ArgumentOutOfRangeException($"Language '{language}' is invalid for written OCR");

            byte[] imageBytes = await ReadImageAsync(imageFilePath);
            var result = await GetImageOCR(imageBytes, language, detectOrientation);
            return result;
        }


        /// <summary>Read a binary Image file asyncronously</summary>
        /// <param name="imageFilePath">full path to the file name</param>
        [DebuggerStepThrough()]
        internal static async Task<byte[]> ReadImageAsync(string imageFilePath)
        {
            byte[] imageBytes;

            // Open a read-only file stream for the specified file with permission for others to read also.
            using (FileStream stream =
                new FileStream(imageFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                // allocate buffer
                imageBytes = new byte[stream.Length];

                // read available bytes until all are read.
                int readPosition = 0;
                for (int readBytes = -1; readPosition < imageBytes.Length && readBytes != 0; readPosition += readBytes)
                {
                    // returned value could be entire file or only part of the file.
                    readBytes = await stream.ReadAsync(imageBytes, readPosition, imageBytes.Length - readPosition);
                }

                // verify
                if (readPosition != imageBytes.Length) throw new IOException($"File {imageFilePath} read failed after {readPosition} of {imageBytes.Length} bytes.");
            }

            return imageBytes;
        }

    }
}
