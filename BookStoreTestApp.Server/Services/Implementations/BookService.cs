using BookStoreTestApp.Server.Services.Interfaces;
using BookStoreTestApp.Server.Models;
using Bogus;

namespace BookStoreTestApp.Server.Services.Implementations;

public class BookService : IBookService
{
	private readonly Random _random = new();
	private readonly Faker<Book> _bookFaker;

	public BookService()
	{
		_bookFaker = new Faker<Book>()
			.RuleFor(b => b.ISBN, f => f.Random.AlphaNumeric(10))
			.RuleFor(b => b.Title, f => f.Lorem.Sentence())
			.RuleFor(b => b.Authors, f => GenerateAuthors())
			.RuleFor(b => b.Publisher, f => f.Company.CompanyName())
			.RuleFor(b => b.Reviews, f => []);
	}


	private List<string> GenerateAuthors()
	{
		var faker = new Faker();
		int numberOfAuthors = _random.Next(1, 4);
		var authors = new List<string>();

		for (int i = 0; i < numberOfAuthors; i++)
		{
			authors.Add(faker.Name.FullName());
		}

		return authors;
	}

	public List<Book> GenerateBooks(int count, double averageLikes, double averageReviews) =>
		[.. _bookFaker.Generate(count).Select((book, index) =>
			{
				book.Index = index + 1;
				book.Reviews = GenerateReviews(averageLikes, averageReviews);
				return book;
			})
		];


	private List<Review> GenerateReviews(double averageLikes, double averageReviews)
	{
		var reviews = new List<Review>();
		int numberOfReviews = (int)Math.Floor(averageReviews); 

		for (int i = 0; i < numberOfReviews; i++)
		{
			reviews.Add(CreateRandomReview());
		}

		double fractionalPart = averageReviews - numberOfReviews;
		if (fractionalPart > 0 && _random.NextDouble() < fractionalPart)
		{
			reviews.Add(CreateRandomReview());
		}

		return reviews;
	}

	private Review CreateRandomReview()
	{
		return new Review
		{
			Text = new Faker().Lorem.Sentence(10),
			Reviewer = new Faker().Name.FullName()
		};
	}
}