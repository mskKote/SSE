using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using task_1_books_service.Models;

namespace task_1_books_service.Controllers;

[Route("v2")]
public sealed class BooksControllerV2(
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
        string? genre,
        // TASK 1.3 Pagination
        int skip = 1,
        int take = 10)
    {
        var books = bookRepository.GetAll();
        var totalBooks = books.Length;

        // TASK 1.1 Search
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

        // TASK 1.3 Pagination
        books = books
            .Skip(skip)
            .Take(take)
            .ToArray();

        // TASK 1.5
        // Extend the books API to support both XML and JSON output.
        // Both variants, i.e. using the corresponding HTTP-Header and using either .xml or .json, should be available,
        // default to XML if nothing is specified. [But IRL json much easier...((]
        // For the JSON output, obey to the Javascript naming conventions (camel case) while preserving the original pascal case names in your C# source.

        var acceptHeader = Request.Headers.Accept.FirstOrDefault();
        if (acceptHeader != null && acceptHeader.Contains("application/json"))
        {
            // For JSON output, use custom naming policy to convert PascalCase to camelCase
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                ReferenceHandler = ReferenceHandler.Preserve
            };
            var jsonResponse = JsonSerializer.Serialize(new
            {
                TotalItems = totalBooks,
                Items = books
            }, jsonOptions);

            return Content(jsonResponse, "application/json");
        }

        // XML output
        var xmlResponse = new XElement("booksResponse",
            new XElement("totalItems", totalBooks),
            new XElement("books",
                books.Select(book => new XElement("book",
                    new XElement("Title", book.Title),
                    new XElement("Author", book.Author),
                    new XElement("Genre", book.Genre)
                )))
        );

        return Content(xmlResponse.ToString(), "application/xml");
    }

    /**
     * TASK 1.4
     *
     * In order to help users calculating how many days are left till some date,
     * e.g. book return date,
     * implement a method which returns the number of days remaining from today till a specified date.
     * The return of the method should either be an XML like
     * <daysUntil endDate="2015-10-21">42</daysUntil>
     * or an empty HTTP Response with a suitable status code to indicate that the input date could not be successfully parsed.
     */
    [HttpGet("daysUntil")]
    public IActionResult DaysUntil(string endDate)
    {
        if (!DateTime.TryParse(endDate, out var parsedEndDate))
            return BadRequest("Invalid date format. Please provide the date in the format yyyy-MM-dd.");

        var remainingTime = parsedEndDate.Date - DateTime.Today;
        var daysRemaining = remainingTime.Days;

        if (daysRemaining < 0)
            return BadRequest("End date cannot be in the past.");

        var xmlResponse = new XElement("daysUntil",
            new XAttribute("endDate", parsedEndDate.ToString("yyyy-MM-dd")),
            daysRemaining);

        return Content(xmlResponse.ToString(), "application/xml");
    }
}