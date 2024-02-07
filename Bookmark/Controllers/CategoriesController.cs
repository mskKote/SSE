using System.Web;
using Bookmark.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bookmark.Controllers;

[Route("[controller]")]
public class CategoriesController(
    ICategoryRepository categoryRepository)
    : Controller
{
    // GET categories
    [HttpGet]
    public Category[] Get()
    {
        return categoryRepository.GetAll();
    }

    // GET categories search
    [HttpGet("{search}")]
    public Category[] Search(string search)
    {
        var searchStr = HttpUtility.UrlDecode(search);
        return categoryRepository.SearchBySubstring(searchStr);
    }

    // GET bookmarks by Category
    [HttpGet("{id:int}/bookmarks")]
    public IActionResult GetBookmarks(int id)
    {
        if (id == 0)
            throw new Exception("Missing or mismatched UserID detected");

        // Check if user exists
        var category = categoryRepository.GetById(id);
        if (category == null)
            throw new Exception($"UserID {id} doesn't exist");
        var bookmarks = categoryRepository.GetBookmarksByCategory(category);
        return Json(bookmarks);
    }

    // POST categories
    [HttpPost]
    public IActionResult Post([FromBody] Category category)
    {
        var id = categoryRepository.Create(category);
        return Created(category.CategoryUrl, category);
    }
}