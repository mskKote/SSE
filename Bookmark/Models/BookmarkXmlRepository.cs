namespace Bookmark.Models;

public sealed class BookmarkXmlRepository(IHostEnvironment env) : IBookmarkRepository
{
    private readonly string _xmlFile = env.ContentRootPath + Database.XmlFile;

    public Bookmark[] GetAll()
    {
        return Database.Deserialize(_xmlFile).Bookmarks;
    }

    public Bookmark[] SearchBySubstring(string substring)
    {
        return Database
            .Deserialize(_xmlFile)
            .Bookmarks
            .Where(x => x.Url.Contains(substring))
            .ToArray();
    }

    public Bookmark? GetById(int id)
    {
        return Database
            .Deserialize(_xmlFile)
            .Bookmarks
            .FirstOrDefault(x => x.Id == id);
    }

    public int Create(Bookmark bookmark)
    {
        var database = Database.Deserialize(_xmlFile);
        var bookmarks = database.Bookmarks;

        bookmark.Id = bookmarks.Length > 0
            ? bookmarks.Select(b => b.Id).Max() + 1
            : 1;

        database.Bookmarks = bookmarks.Append(bookmark).ToArray();
        database.Serialize(_xmlFile);
        return bookmark.Id;
    }

    public void Update(Bookmark bookmark)
    {
        var database = Database.Deserialize(_xmlFile);
        var bookmarks = database.Bookmarks
            .Where(b => b.Id != bookmark.Id);
        database.Bookmarks = bookmarks.Append(bookmark).ToArray();
        database.Serialize(_xmlFile);
    }

    public void Delete(int id)
    {
        var database = Database.Deserialize(_xmlFile);
        database.Users = database.Users.Where(b => b.Id != id).ToArray();
        database.Serialize(_xmlFile);
    }

    public Bookmark[] GetByUser(int user)
    {
        return Database
            .Deserialize(_xmlFile)
            .Bookmarks
            .Where(b => b.UserId == user)
            .ToArray();
    }
}