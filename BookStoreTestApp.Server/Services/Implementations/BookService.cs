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
			.RuleFor(b => b.Title, f => GenerateTitle(f, request.Region))
			.RuleFor(b => b.Publisher, f => $"{f.Company.CompanyName()} - {f.Random.Number(1980, 2025)}")
			.RuleFor(b => b.Authors, f => [..
				Enumerable.Range(1, f.Random.Int(1, 3))
				.Select(_ => f.Name.FullName())])
			.RuleFor(b => b.Reviews, f => []);

		int startIndex = request.PageSize * request.Page;

		List<Book> books = [];
		for (int i = 0; i < request.PageSize; i++)
		{
			int index = startIndex + i + 1;
			int seed = $"{request.Seed}-{request.Region}-{index}".GetHashCode();
			bookFaker.UseSeed(seed);
			Book book = bookFaker.Generate();
			book.Index = index;
			book.ImageUrl = GenerateRandomImageUrl(seed, book.Title);
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
		var faker = new Faker<Review>(region)
			.RuleFor(b => b.Text, f => GenerateReviewText(f, region))
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

	private string GenerateReviewText(Faker f, string region)
	{
		List<string> sentences = region switch
		{
			"de" => [
					$"Ich fand dieses Buch {f.Commerce.ProductAdjective()} und {f.Commerce.ProductAdjective()}.",
					$"Die Handlung war {f.Commerce.ProductAdjective()}, und die Charaktere waren {f.Commerce.ProductAdjective()}.",
					$"Ich empfehle dieses Buch jedem, der {f.Commerce.ProductAdjective()} Geschichten mag.",
					$"Dieses Buch ist ein großartiges Beispiel für {f.Commerce.ProductAdjective()} Schreiben.",
					$"Ich war beeindruckt vom {f.Commerce.ProductAdjective()} Ende."
			],
			"fr" => [
					$"J'ai trouvé ce livre {f.Commerce.ProductAdjective()} et {f.Commerce.ProductAdjective()}.",
					$"L'intrigue était {f.Commerce.ProductAdjective()}, et les personnages étaient {f.Commerce.ProductAdjective()}.",
					$"Je recommande vivement ce livre à tous ceux qui aiment les histoires {f.Commerce.ProductAdjective()}.",
					$"Ce livre est un excellent exemple d'écriture {f.Commerce.ProductAdjective()}.",
					$"J'ai été impressionné par la fin {f.Commerce.ProductAdjective()}."
			],
			_ => [
					$"I found this book to be {f.Commerce.ProductAdjective()} and {f.Commerce.ProductAdjective()}.",
					$"The storyline was {f.Commerce.ProductAdjective()}, and the characters were {f.Commerce.ProductAdjective()}.",
					$"I highly recommend this book for anyone who enjoys {f.Commerce.ProductAdjective()} stories.",
					$"This book is a great example of {f.Commerce.ProductAdjective()} writing.",
					$"I was impressed by the {f.Commerce.ProductAdjective()} ending."
			],
		};

		return string.Join(" ", sentences.OrderBy(_ => f.Random.Int(0, sentences.Count - 1)).Take(2));
	}


	private string GenerateTitle(Faker f, string region)
	{
		List<string> titleFormats = region switch
		{
			"fr" =>
			[
				"{Adjective} {Noun}",
				"{Noun} de {Noun}",
				"{Verb} le {Noun}",
				"{Noun} dans {Place}",
				"Le {Adjective} {Noun}"
			],
			"de" =>
			[
				"{Adjective} {Noun}",
				"{Noun} von {Noun}",
				"{Verb} das {Noun}",
				"{Noun} in {Place}",
				"Der {Adjective} {Noun}"
			],
			_ =>
			[
				"{Adjective} {Noun}",
				"{Noun} of {Noun}",
				"{Verb} the {Noun}",
				"{Noun} in {Place}",
				"The {Adjective} {Noun}"
			],
		};

		var selectedFormat = f.PickRandom(titleFormats);

		return selectedFormat
			.Replace("{Adjective}", f.Commerce.ProductAdjective())
			.Replace("{Noun}", f.Commerce.ProductAdjective())
			.Replace("{Verb}", f.Hacker.Verb())
			.Replace("{Place}", f.Address.City());
	}

	public string GenerateRandomImageUrl(int seed, string title)
	{
		var r = new Random(seed).Next(9);
		var g = new Random(seed).Next(9);
		var b = new Random(seed).Next(9);
		string rgb = $"{r}{g}{b}";

		return $"https://dummyimage.com/verticalrectangle/{rgb}/fff&text={title}";
	}
}