using BookStoreTestApp.Server.Models;
using BookStoreTestApp.Server.Services.Implementations;
using BookStoreTestApp.Server.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreTestApp.Server.Controllers;

[Route("api/books")]
[ApiController]
public class BooksController(IBookService bookService) : ControllerBase
{
	private readonly IBookService _bookService = bookService;

	[HttpGet]
	public ActionResult<List<Book>> GetBooks(int page, int likes, int reviews)
	{
		var books = _bookService.GenerateBooks(20, likes, reviews);
		return Ok(books);
	}
}
