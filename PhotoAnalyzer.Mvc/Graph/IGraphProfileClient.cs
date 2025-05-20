using Microsoft.Graph;

namespace PhotoAnalyzer.Mvc.Graph;

public interface IGraphProfileClient
{
    Task<User> GetUserAsync();
    Task<byte[]> GetProfilePictureAsync(string userId);
}