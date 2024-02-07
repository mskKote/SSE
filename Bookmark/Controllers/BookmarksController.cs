using System.Web;
using Bookmark.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bookmark.Controllers;

[Route("[controller]")]
public class BookmarksController(
    IUserRepository userRepository,
    IBookmarkRepository bookmarkRepository)
    : Controller
{
    // DELETE bookmarks/5
    [HttpDelete("{id:int}")]
    public void Delete(int id)
    {
        bookmarkRepository.Delete(id);
    }

    // GET bookmarks
    [HttpGet]
    public Models.Bookmark[] Get()
    {
        return bookmarkRepository.GetAll();
    }

    // GET bookmarks search
    [HttpGet("{search}")]
    public Models.Bookmark[] Get(string search)
    {
        var searchStr = HttpUtility.UrlDecode(search);
        return bookmarkRepository.SearchBySubstring(searchStr);
    }

    // PUT bookmarks/5
    [HttpPut("{id:int}")]
    public void Put(int id, [FromBody] Models.Bookmark bookmark)
    {
        // id is also expected in the body
        if (bookmark.Id == 0 || bookmark.Id != id)
            throw new Exception("Missing or mismatched ID detected");
        bookmarkRepository.Update(bookmark);
    }
}