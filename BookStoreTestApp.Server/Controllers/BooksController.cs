using System.Security.Cryptography;
using BookStoreTestApp.Server.Models;
using BookStoreTestApp.Server.Requests;
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
	public ActionResult<List<Book>> GetBooks([FromQuery] BookRequest request)
	{
		var books = _bookService.GenerateBooks(request);
		return Ok(books);
	}

	[HttpGet("random-seed")]
	public ActionResult<int> GetRandomSeed()
	{
		var random = new Random();
		int seed = random.Next(1, int.MaxValue);
		return Ok(seed);
	}
}
