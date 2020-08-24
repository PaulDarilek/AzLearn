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
    public class VisionOcrTests
    {
        private const string LogFolder = @"..\..\..\Logs";
        private const string _TestFiles = @"..\..\..\_TestFiles\";

        [Fact]
        async Task VisionOcrV3_GetImageOCR_Handwritten()
        {
            var files = Directory.GetFiles(_TestFiles, "handwritten-*.*");

            foreach (var file in files)
            {
                string expected =
                    Path.GetFileNameWithoutExtension(file)
                    .Replace("handwritten-", "")
                    .Replace("-", " ");

                var service = CreateV3();
                var actual = await service.GetImageOCR(file);

                // Log results
                if (actual != null)
                {
                    // Log the result as JSON
                    var json = JsonConvert.SerializeObject(actual);
                    var jsonFmt = JToken.Parse(json).ToString();
                    LogResults(jsonFmt, file, ".json");
                }

                // Assert expectations.
                actual.Should().NotBeNull();
                actual.ErrorText.Should().BeNull();
                actual.AnalyzeResult?.ReadResults.Should().NotBeEmpty();
                actual.AnalyzeResult.ReadResults[0].Lines.Should().NotBeEmpty();
                actual.AnalyzeResult.ReadResults[0].Lines[0].Text.Should().Be(expected);
            }
        }

        [Fact]
        async Task VisionOcrV3_GetImageOCR_Printed()
        {
            var files = Directory.GetFiles(_TestFiles, "printed-*.*");

            foreach (var file in files)
            {
                string expected =
                    Path.GetFileNameWithoutExtension(file)
                    .Replace("printed-", "")
                    .Replace("-", " ");

                var service = CreateV3();
                var actual = await service.GetImageOCR(file);

                // Log results
                if (actual != null)
                {
                    // Log the result as JSON
                    var json = JsonConvert.SerializeObject(actual);
                    var jsonFmt = JToken.Parse(json).ToString();
                    LogResults(jsonFmt, file, ".json");
                }

                // Assert expectations.
                actual.Should().NotBeNull();
                actual.ErrorText.Should().BeNull();
                actual.AnalyzeResult?.ReadResults.Should().NotBeEmpty();
                actual.AnalyzeResult.ReadResults[0].Lines.Should().NotBeEmpty();
                if(actual.AnalyzeResult.ReadResults[0].Lines[0].Text != expected && actual.AnalyzeResult.ReadResults[0].Lines[0].Text == expected.Split(" ")[0])
                {
                    var allLines = string.Join(' ', actual.AnalyzeResult.ReadResults[0].Lines.Select(x => x.Text));
                    allLines.Should().Be(expected);
                }
                else
                {
                    actual.AnalyzeResult.ReadResults[0].Lines[0].Text.Should().Be(expected);
                }
            }
        }

        [Fact]
        async Task VisionOcrV2_GetImageOCR()
        {
            var files = Directory.GetFiles(_TestFiles, "printed-*.*");

            foreach (var file in files)
            {
                if (Path.GetExtension(file).Equals(".pdf", System.StringComparison.OrdinalIgnoreCase))
                    continue; // skip PDF files.

                string expected =
                    Path.GetFileNameWithoutExtension(file)
                    .Replace("printed-", "")
                    .Replace("-", " ");

                var service = CreateV2();
                var actual = await service.GetImageOCR(file);

                // Log results
                if (actual != null)
                {
                    // Log the result as JSON
                    var json = JsonConvert.SerializeObject(actual);
                    var jsonFmt = JToken.Parse(json).ToString();
                    LogResults(jsonFmt, file, ".json");
                }

                // Assert expectations.
                actual.Should().NotBeNull();
                actual.Regions.Should().NotBeEmpty();
                actual.Regions[0].Lines.Should().NotBeEmpty();
                actual.Regions[0].Lines[0].Words.Should().NotBeEmpty();
                var text = string.Join(' ', actual.Regions[0].Lines[0].Words.Select(w => w.Text).ToArray());

                // Different interpretation of spaces...
                if (expected != text && expected == text.Replace(" ", ""))
                {
                    text = text.Replace(" ", "");
                }
                text.Should().Be(expected);
            }
        }


        private VisionOcrV3 CreateV3()
        {
            var settings = ConfigurationService.GetConfigurationSettings<VisionSettings>();
            var service = new VisionOcrV3(settings);
            return service;
        }

        private VisionOcrV2 CreateV2()
        {
            var settings = ConfigurationService.GetConfigurationSettings<VisionSettings>();
            var service = new VisionOcrV2(settings);
            return service;
        }

        private void LogResults(string text, string fileName, string extension = ".txt", [CallerMemberName] string methodName = null)
        {
            fileName = Path.Combine(
                LogFolder, 
                $"{methodName}_{Path.GetFileNameWithoutExtension(fileName)}{extension}");
            
            File.WriteAllText(fileName, text);
        }
    }
}
