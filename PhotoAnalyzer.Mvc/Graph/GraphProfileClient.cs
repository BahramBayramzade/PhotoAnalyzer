using Microsoft.Graph;

namespace PhotoAnalyzer.Mvc.Graph;

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
            var user = await _graphServiceClient.Me.Request().GetAsync();
            return user;
        }
        catch (ServiceException ex)
        {
            Console.WriteLine($"Error getting user: {ex.Message}");
            throw;
        }
    }

    public async Task<byte[]> GetProfilePictureAsync(string userId)
    {
        try
        {
            var stream = await _graphServiceClient.Users[userId].Photo.Content.Request().GetAsync();
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }
        catch (ServiceException ex)
        {
            Console.WriteLine($"Error getting profile picture: {ex.Message}");
            throw;
        }
    }
}