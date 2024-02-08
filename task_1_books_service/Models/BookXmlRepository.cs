namespace task_1_books_service.Models;

public sealed class BookXmlRepository(
    IHostEnvironment env)
    : IBookRepository
{
    private const string XmlFile = "/data.xml";
    private readonly string _xmlFile = env.ContentRootPath + XmlFile;

    public Book[] GetAll()
    {
        return Database.Deserialize(_xmlFile).Books;
    }

    public Book? GetById(int id)
    {
        return Database
            .Deserialize(_xmlFile)
            .Books
            .FirstOrDefault(x => x.Id == id);
    }

    public int Create(Book book)
    {
        var database = Database.Deserialize(_xmlFile);
        var books = database.Books;

        book.Id = books.Length > 0
            ? books.Select(b => b.Id).Max() + 1
            : 1;

        database.Books = books.Append(book).ToArray();
        database.Serialize(_xmlFile);
        return book.Id;
    }

    public void Update(Book book)
    {
        var database = Database.Deserialize(_xmlFile);
        var books = database.Books.Where(b => b.Id != book.Id);
        database.Books = books.Append(book).ToArray();
        database.Serialize(_xmlFile);
    }

    public void Delete(int id)
    {
        var database = Database.Deserialize(_xmlFile);
        database.Books = database.Books.Where(b => b.Id != id).ToArray();
        database.Serialize(_xmlFile);
    }
}