using Microsoft.Graph;

namespace PhotoAnalyzer.API.Graph;

public interface IGraphFilesClient
{
    Task<List<DriveItem>> GetDriveItemsAsync();
    Task UploadFileToRootAsync(string fileName, Stream stream);
    Task DeleteFileAsync(string fileId);
    Task<string> GetFileThumbnailUrlAsync(string fileId);
}