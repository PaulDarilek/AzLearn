using System.Threading.Tasks;

namespace AzLearn.Core.Vision
{
    public interface IVisionOcr<TResult>
    {
        Task<TResult> GetImageOCR(byte[] imageBytes, string language = "en", bool detectOrientation = true);
        Task<TResult> GetImageOCR(string imageFilePath, string language = "en", bool detectOrientation = true);
    }
}