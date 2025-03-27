namespace BookStoreTestApp.Server.Models;

public class Book
{
	public int Index { get; set; }
	public string ISBN { get; set; } = string.Empty;
	public string Title { get; set; } = string.Empty;
	public double Likes { get; set; }
	public List<string> Authors { get; set; } = [];
	public string Publisher { get; set; } = string.Empty;
	public List<Review> Reviews { get; set; } = [];
}
