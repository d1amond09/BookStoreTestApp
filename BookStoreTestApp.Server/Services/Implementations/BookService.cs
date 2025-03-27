using BookStoreTestApp.Server.Services.Interfaces;
using BookStoreTestApp.Server.Requests;
using BookStoreTestApp.Server.Models;
using Bogus;
using System.Collections.Generic;

namespace BookStoreTestApp.Server.Services.Implementations;

public class BookService() : IBookService
{
	public List<Book> GenerateBooks(BookRequest request)
	{
		var bookFaker = new Faker<Book>(request.Region)
			.RuleFor(b => b.ISBN, f => f.Random.Bool() ? 
				$"978-{f.Random.Number(10, 99)}-{f.Random.Number(10000, 99999)}-{f.Random.Number(10000, 99999)}-{f.Random.Number(0, 9)}"
				: $"{f.Random.Number(1, 9)}-{f.Random.Number(10000, 99999)}-{f.Random.Number(10000, 99999)}-{f.Random.Number(0, 9)}")
			.RuleFor(b => b.Title, f => f.Lorem.Sentence())
			.RuleFor(b => b.Publisher, f => f.Company.CompanyName())
			.RuleFor(b => b.Authors, f => [.. 
				Enumerable.Range(1, f.Random.Int(1, 3))
				.Select(_ => f.Name.FullName())])
			.RuleFor(b => b.Reviews, f => []);

		int startIndex = request.PageSize * request.Page;

		List<Book> books = [];
		for (int i = 0; i < request.PageSize; i++)
		{
			int index = startIndex + i + 1;
			int seed = $"{request.Seed}-{index}".GetHashCode();
			bookFaker.UseSeed(seed);
			Book book = bookFaker.Generate();
			book.Index = index;
			book.Likes = GenerateLikes(seed, request.LikesAvg);
			book.Reviews = GenerateReviews(seed, request.Region, request.ReviewsAvg);
			books.Add(book);
		}
		return books;
	}

	private static int GenerateLikes(int seed, double avg)
	{
		var random = new Random(seed);

		if (Math.Floor(avg) == avg)
		{
			return (int)Math.Floor(avg);
		}

		var intPart = (int)Math.Floor(avg);
		var fractPart = avg - intPart;
		var likes = intPart;

		if (random.NextDouble() < fractPart)
		{
			likes += 1;
		}

		return likes;
	}


	private List<Review> GenerateReviews(int seed, string region, double averageReviews)
	{
		Random rnd = new(seed);
		var faker = new Faker<Review>()
			.RuleFor(b => b.Text, f => f.Lorem.Sentence(10))
			.RuleFor(b => b.Reviewer, f => f.Name.FullName())
			.UseSeed(seed);

		List<Review> reviews = [];
		int numberOfReviews = (int)Math.Floor(averageReviews); 

		for (int i = 0; i < numberOfReviews; i++)
		{
			reviews.Add(faker.Generate());
		}

		double fractionalPart = averageReviews - numberOfReviews;
		if (fractionalPart > 0 && rnd.NextDouble() < fractionalPart)
		{
			reviews.Add(faker.Generate());
		}

		return reviews;
	}
}