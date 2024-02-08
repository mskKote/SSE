namespace task_1_books_service.Models;

public interface IBookRepository
{
    Book[] GetAll();
    Book? GetById(int id);
    int Create(Book book);
    void Update(Book book);
    void Delete(int id);
}