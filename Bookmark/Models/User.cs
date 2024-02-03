namespace Bookmark.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string BookmarksUrl => "http://localhost:5000/users/" + Id + "/bookmarks";
}