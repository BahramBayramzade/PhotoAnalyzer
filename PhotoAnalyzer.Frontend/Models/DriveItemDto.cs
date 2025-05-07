namespace PhotoAnalyzer.Frontend.Models;

public class DriveItemDto
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public long Size { get; set; }
    public string? WebUrl { get; set; }
    public bool IsSelected { get; set; }
}