using BookStoreTestApp.Server.Models;

namespace BookStoreTestApp.Server.Services.Interfaces;

public interface IBookService
{
	List<Book> GenerateBooks(int count, double averageLikes, double averageReviews);
}
