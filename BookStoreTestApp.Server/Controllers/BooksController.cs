using System.Formats.Asn1;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using BookStoreTestApp.Server.Models;
using BookStoreTestApp.Server.Requests;
using BookStoreTestApp.Server.Services.Implementations;
using BookStoreTestApp.Server.Services.Interfaces;
using CsvHelper;
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

	[HttpGet("export-csv")]
	public IActionResult ExportToCsv([FromQuery] BookRequest request)
	{
		request.PageSize = request.PageSize * request.Page;
		request.Page = 0;
		List<Book> books = _bookService.GenerateBooks(request); 

		using var writer = new StringWriter();
		using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
		csv.WriteRecords(books);
		var csvData = writer.ToString();

		var bytes = Encoding.UTF8.GetBytes(csvData);
		return File(bytes, "text/csv", "books.csv");
	}
}
