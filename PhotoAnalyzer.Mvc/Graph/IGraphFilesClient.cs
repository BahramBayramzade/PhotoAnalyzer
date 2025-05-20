using Microsoft.Graph;

namespace PhotoAnalyzer.Mvc.Graph;

public interface IGraphFilesClient
{
    Task<List<DriveItem>> GetDriveItemsAsync();
    Task<string> GetFileThumbnailUrlAsync(string fileId);
    Task DeleteFileAsync(string fileId);
    Task UploadFileToRootAsync(string fileName, Stream stream);
    Task<DriveItem> GetFileInfoAsync(string fileId);
    Task DeleteFilesAsync(IEnumerable<string> fileIds);
}