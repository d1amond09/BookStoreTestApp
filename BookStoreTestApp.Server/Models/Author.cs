namespace BookStoreTestApp.Server.Models;

public class Author
{
	public string FirstName { get; set; } = string.Empty;
	public string SecondName { get; set; } = string.Empty;

	public override string ToString() => $"{FirstName} {SecondName}";
}
