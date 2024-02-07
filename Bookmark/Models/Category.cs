namespace Bookmark.Models;

public sealed class Category
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string CategoryUrl => "http://localhost:5000/categories/" + Id;
}