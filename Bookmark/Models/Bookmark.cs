namespace Bookmark.Models;

public sealed class Bookmark
{
    public int Id { get; set; }
    public string Url { get; set; }
    public int UserId { get; set; }

    public string[] Categories { get; set; } = [];
}