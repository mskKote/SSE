using System.Xml;
using Microsoft.AspNetCore.Mvc;
using task_1_books_service.Models;

namespace task_1_books_service.Controllers;

// TASK 1.2
//
// Having fixed the API, one can now concentrate on adding new functionality.
// In order to still provide some kind of legacy support, we introduce API versions.
// Implement Versioning for the Library API.
// Version 1 should provide functionality as of 1.1,
// the features added in 1.3 – 1.5 should be interfaced in API Version 2 only.
// All unchanged functionality from version 1 should be available in version 2 as well.

[Route("v1")]
public sealed class BooksControllerV1(
    IBookRepository bookRepository)
    : Controller
{
    private const string Atom = """
                                <service xmlns:atom="http://www.w3.org/2005/Atom" xmlns="http://localhost:5000">
                                                                           <atom:link rel="books" href="/books" />
                                                                       </service>
                                """;

    // GET /
    [HttpGet("")]
    [Produces("application/xml")]
    public IActionResult Index()
    {
        var doc = new XmlDocument();
        doc.LoadXml(Atom);
        return Ok(doc);
    }

    // GET /books
    [HttpGet("books")]
    public IActionResult GetBooks(
        // TASK 1.1 Search
        string? author,
        string? title,
        string? genre)
    {
        var books = bookRepository.GetAll();

        if (!string.IsNullOrEmpty(author)
            || !string.IsNullOrEmpty(title)
            || !string.IsNullOrEmpty(genre))
            books = books
                .Where(book =>
                {
                    if (!string.IsNullOrEmpty(author) &&
                        !book.Author.Contains(author, StringComparison.InvariantCultureIgnoreCase))
                        return false;

                    if (!string.IsNullOrEmpty(title) &&
                        !book.Title.Contains(title, StringComparison.InvariantCultureIgnoreCase))
                        return false;

                    if (!string.IsNullOrEmpty(genre) &&
                        !book.Genre.Contains(genre, StringComparison.InvariantCultureIgnoreCase))
                        return false;

                    return true;
                })
                .ToArray();

        var totalBooks = books.Length;

        var response = new
        {
            TotalItems = totalBooks,
            Items = books
        };

        return Ok(response);
    }


    // TASK 1.1 code below replaced with books/search

    // GET /books/ofAuthor/.../withTitle/...
    [Obsolete("This method is deprecated. Use the new BooksControllerV2 instead.")]
    [HttpGet("books/ofAuthor/{author}/withTitle/{title}")]
    public Book[] GetBooksByAuthorAndTitle(string author, string title)
    {
        var books = bookRepository.GetAll().Where(book =>
            book.Author.Contains(author, StringComparison.InvariantCultureIgnoreCase) &&
            book.Title.Contains(title, StringComparison.InvariantCultureIgnoreCase)
        ).ToArray();

        return books;
    }

    // GET /books/ofAuthor/.../hasGenre/...
    [Obsolete("This method is deprecated. Use the new BooksControllerV2 instead.")]
    [HttpGet("books/ofAuthor/{author}/hasGenre/{genre}")]
    public Book[] GetBooksByAuthorAndGenre(string author, string genre)
    {
        var books = bookRepository.GetAll().Where(book =>
            book.Author.Contains(author, StringComparison.InvariantCultureIgnoreCase) &&
            book.Genre.Contains(genre, StringComparison.InvariantCultureIgnoreCase)
        ).ToArray();

        return books;
    }

    // GET /books/ofAuthor/.../withTitle/...
    [Obsolete("This method is deprecated. Use the new BooksControllerV2 instead.")]
    [HttpGet("books/withTitle/{title}/hasGenre/{genre}")]
    public Book[] GetBooksByTitleAndGenre(string title, string genre)
    {
        var books = bookRepository.GetAll().Where(book =>
            book.Title.Contains(title, StringComparison.InvariantCultureIgnoreCase) &&
            book.Genre.Contains(genre, StringComparison.InvariantCultureIgnoreCase)
        ).ToArray();

        return books;
    }
}