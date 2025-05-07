using Microsoft.Graph;

namespace PhotoAnalyzer.API.Graph;

public interface IGraphProfileClient
{
    Task<User> GetUserAsync();
    Task<byte[]> GetProfilePictureAsync(string userId);
}