namespace Bookmark.Models;

public interface IBookmarkRepository
{
    Bookmark[] GetAll();
    Bookmark[] SearchBySubstring(string substring);
    Bookmark? GetById(int id);
    int Create(Bookmark bookmark);
    void Update(Bookmark bookmark);
    void Delete(int id);
    Bookmark[] GetByUser(int user);
}