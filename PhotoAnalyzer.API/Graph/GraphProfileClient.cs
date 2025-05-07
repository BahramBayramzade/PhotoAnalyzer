using Microsoft.Graph;

namespace PhotoAnalyzer.API.Graph;

public class GraphProfileClient : IGraphProfileClient
{
    private readonly GraphServiceClient _graphServiceClient;

    public GraphProfileClient(GraphServiceClient graphServiceClient)
    {
        _graphServiceClient = graphServiceClient;
    }

    public async Task<User> GetUserAsync()
    {
        try
        {
            var user = await _graphServiceClient.Me
                .Request()
                .GetAsync();

            return user;
        }
        catch (ServiceException ex)
        {
            // Handle exceptions as needed
            throw new Exception($"Error retrieving user: {ex.Message}", ex);
        }
    }

    public async Task<byte[]> GetProfilePictureAsync(string userId)
    {
        try
        {
            // Получаем поток изображения из Microsoft Graph
            var photoStream = await _graphServiceClient.Users[userId].Photo.Content.Request().GetAsync();
        
            // Читаем поток в массив байтов
            using var memoryStream = new MemoryStream();
            await photoStream.CopyToAsync(memoryStream);
            var imageBytes = memoryStream.ToArray();
            
            // Возвращаем массив байтов изображения
            return imageBytes;
        }
        catch (ServiceException ex)
        {
            // Handle exceptions as needed
            throw new Exception($"Error retrieving user profile picture: {ex.Message}", ex);
        }
    }
}