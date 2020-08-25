using Xunit;
using AzLearn.Core.Vision;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using FluentAssertions;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AzLearn.Core.Tests
{
    public class VisionRequestSepInfoTests : VisionOcrTests
    {
        [Fact]
        public async Task RequestForSeparationInformationTest()
        {
            var fileName = GetFiles("_handwritten-request-for-separation-info.tiff").First();

            var actual = await ProcessImageAsync(fileName);

            // Assert expectations.
            actual.Should().NotBeNull();
            actual.ErrorText.Should().BeNull();
            actual.AnalyzeResult?.ReadResults.Should().NotBeEmpty();
            actual.AnalyzeResult.ReadResults[0].Lines.Should().NotBeEmpty();
        }

        /// <summary>Process an image or use the cached result</summary>
        private async Task<VisionOcrV3Result> ProcessImageAsync(string fileName, [CallerMemberName] string methodName = null)
        {
            string logFile = GetLogFileName(fileName, ".json", methodName);
            if (File.Exists(logFile))
            {
                string json = await File.ReadAllTextAsync(logFile);
                var cached = JsonConvert.DeserializeObject<VisionOcrV3Result>(json);
                return cached;
            }

            var service = CreateV3();
            var actual = await service.GetImageOCR(fileName);

            // Log results
            if (actual != null)
            {
                // Log the result as JSON
                var json = JsonConvert.SerializeObject(actual);
                var jsonFmt = JToken.Parse(json).ToString();
                LogResults(jsonFmt, fileName, ".json");
            }

            return actual;
        
        }


    }
}
