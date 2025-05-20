using Microsoft.Graph;

namespace PhotoAnalyzer.Mvc.Graph;

public class GraphFilesClient : IGraphFilesClient
{
    private readonly GraphServiceClient _graphClient;
    private readonly ILogger<GraphFilesClient> _logger;

    public GraphFilesClient(GraphServiceClient graphClient, ILogger<GraphFilesClient> logger)
    {
        _graphClient = graphClient;
        _logger = logger;
    }

    public async Task<List<DriveItem>> GetDriveItemsAsync()
    {
        try
        {
            var driveItems = await _graphClient.Me.Drive.Root.Children
                .Request()
                // Добавляем выборку размера файла
                .Select("id,name,file,size,lastModifiedDateTime")
                .GetAsync();

            var items = driveItems.CurrentPage
                .Where(item => item.File != null && item.File.MimeType.StartsWith("image/"))
                .ToList();

            while (driveItems.NextPageRequest != null)
            {
                driveItems = await driveItems.NextPageRequest.GetAsync();
                items.AddRange(driveItems.CurrentPage
                    .Where(item => item.File != null && item.File.MimeType.StartsWith("image/")));
            }

            return items;
        }
        catch (ServiceException ex)
        {
            _logger.LogError(ex, "Error retrieving OneDrive items");
            throw;
        }
    }

    public async Task<string> GetFileThumbnailUrlAsync(string fileId)
    {
        try
        {
            var thumbnails = await _graphClient.Me.Drive.Items[fileId].Thumbnails
                .Request()
                .GetAsync();

            var thumbnailUrl = thumbnails.CurrentPage.FirstOrDefault()?.Large?.Url;
            return thumbnailUrl!;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error getting thumbnail URL: {ExMessage}", ex.Message);
            return null!;
        }
    }

    public async Task UploadFileToRootAsync(string fileName, Stream stream)
    {
        try
        {
            var uploadedItem = await _graphClient.Me.Drive.Root
                .ItemWithPath(fileName)
                .Content
                .Request()
                .PutAsync<DriveItem>(stream);
            _logger.LogInformation(
                "File '{FileName}' uploaded successfully with ID: {UploadedItemId}", fileName, uploadedItem.Id);
        }
        catch (ServiceException ex)
        {
            _logger.LogError("Error uploading file: {ExMessage}", ex.Message);
            throw;
        }
    }

    public async Task<DriveItem> GetFileInfoAsync(string fileId)
    {
        try
        {
            var fileInfo = await _graphClient.Me.Drive.Items[fileId]
                .Request()
                .Select("id,name,file,size,lastModifiedDateTime")
                .GetAsync();

            return fileInfo;
        }
        catch (ServiceException ex)
        {
            _logger.LogError("Error retrieving file info: {ExMessage}", ex.Message);
            throw;
        }
    }

    public async Task DeleteFilesAsync(IEnumerable<string> fileIds)
    {
        try
        {
            foreach (var fileId in fileIds)
            {
                await _graphClient.Me.Drive.Items[fileId].Request().DeleteAsync();
            }
        }
        catch (ServiceException ex)
        {
            _logger.LogError("Error deleting files: {ExMessage}", ex.Message);
            throw;
        }
    }

    public async Task DeleteFileAsync(string fileId)
    {
        try
        {
            await _graphClient.Me.Drive.Items[fileId].Request().DeleteAsync();
        }
        catch (ServiceException ex)
        {
            _logger.LogError("Error deleting file: {ExMessage}", ex.Message);
            throw;
        }
    }
}