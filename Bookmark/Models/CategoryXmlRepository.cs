namespace Bookmark.Models;

public sealed class CategoryXmlRepository(IHostEnvironment env) : ICategoryRepository
{
    private readonly string _xmlFile = env.ContentRootPath + Database.XmlFile;

    public Category[] GetAll()
    {
        return Database.Deserialize(_xmlFile).Categories;
    }

    public Category[] SearchBySubstring(string substring)
    {
        return Database
            .Deserialize(_xmlFile)
            .Categories
            .Where(x => x.Name.Contains(substring, StringComparison.CurrentCultureIgnoreCase))
            .ToArray();
    }

    public Category? GetById(int id)
    {
        return Database
            .Deserialize(_xmlFile)
            .Categories
            .FirstOrDefault(x => x.Id == id);
    }

    public Bookmark[] GetBookmarksByCategory(Category category)
    {
        return Database
            .Deserialize(_xmlFile)
            .Bookmarks
            .Where(x => x.Categories.Contains(category.CategoryUrl))
            .ToArray();
    }

    public int Create(Category category)
    {
        var database = Database.Deserialize(_xmlFile);
        var categories = database.Categories;

        category.Id = categories.Length > 0
            ? categories.Select(b => b.Id).Max() + 1
            : 1;

        database.Categories = categories.Append(category).ToArray();
        database.Serialize(_xmlFile);

        return category.Id;
    }
}