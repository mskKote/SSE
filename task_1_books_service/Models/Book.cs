namespace task_1_books_service.Models;

public sealed class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string Genre { get; set; }

    public string Url => "/books/" + Id;

    public string BookUrl => "/books/" + Id + "/book";
}