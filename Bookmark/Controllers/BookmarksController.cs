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
}