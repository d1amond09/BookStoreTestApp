using BookStoreTestApp.Server.Models;
using BookStoreTestApp.Server.Requests;

namespace BookStoreTestApp.Server.Services.Interfaces;

public interface IBookService
{
	List<Book> GenerateBooks(BookRequest bookRequest);
}
