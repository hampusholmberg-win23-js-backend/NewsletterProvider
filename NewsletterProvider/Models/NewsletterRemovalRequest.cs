namespace NewsletterProvider.Models;

public class NewsletterRemovalRequest
{
    public string Email { get; set; } = null!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}