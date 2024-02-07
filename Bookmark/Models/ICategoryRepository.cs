namespace Bookmark.Models;

public interface ICategoryRepository
{
    Category[] GetAll();

    Category[] SearchBySubstring(string substring);

    Category? GetById(int id);

    Bookmark[] GetBookmarksByCategory(Category category);

    int Create(Category category);
}