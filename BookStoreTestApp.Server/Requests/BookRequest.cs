namespace BookStoreTestApp.Server.Requests;

public class BookRequest
{
	public string Region { get; set; } = "en_US";
	public string Seed { get; set; } = "42";
	public double LikesAvg { get; set; } = 3.5;
	public double ReviewsAvg { get; set; } = 2.0;
	public int Page { get; set; } = 0;
	public int PageSize { get; set; } = 20;
}
