using Bookmark.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bookmark.Controllers;

[Route("[controller]")]
public class UsersController(
    IUserRepository userRepository,
    IBookmarkRepository bookmarkRepository)
    : Controller
{
    // GET users
    [HttpGet]
    public User[] Get()
    {
        return userRepository.GetAll();
    }

    // GET users/5
    [HttpGet("{id:int}")]
    public IActionResult Get(int id)
    {
        var user = userRepository.GetById(id);
        if (user == null) return NotFound();

        return Json(user);
    }

    // POST users
    [HttpPost]
    public IActionResult Post([FromBody] User user)
    {
        var id = userRepository.Create(user);
        return Created("http://localhost:5000/users/" + id, user);
    }

    // PUT users/5
    [HttpPut("{id:int}")]
    public void Put(int id, [FromBody] User user)
    {
        // id is also expected in the body
        if (user.Id == 0 || user.Id != id)
            throw new Exception("Missing or mismatched ID detected");
        userRepository.Update(user);
    }

    // DELETE users/5
    [HttpDelete("{id:int}")]
    public void Delete(int id)
    {
        userRepository.Delete(id);
    }

    // GET users/5/bookmarks
    [HttpGet("{id:int}/bookmarks")]
    public IActionResult GetBookmarks(int id)
    {
        if (id == 0)
            throw new Exception("Missing or mismatched UserID detected");

        // Check if user exists
        if (userRepository.GetById(id) == null)
            throw new Exception($"UserID {id} doesn't exist");

        var bookmarks = bookmarkRepository.GetByUser(id);
        return Json(bookmarks);
    }

    // POST users/5/bookmarks
    [HttpPost("{id:int}/bookmarks")]
    public IActionResult Post(int id, [FromBody] Models.Bookmark bookmark)
    {
        if (id == 0 || bookmark.UserId == 0 || bookmark.UserId != id)
            throw new Exception("Missing or mismatched UserID detected");

        // Check if user exists
        if (userRepository.GetById(id) == null)
            throw new Exception($"UserID {id} doesn't exist");

        var newId = bookmarkRepository.Create(bookmark);
        return Created($"http://localhost:5000/{id}/bookmarks/${newId}", bookmark);
    }
}