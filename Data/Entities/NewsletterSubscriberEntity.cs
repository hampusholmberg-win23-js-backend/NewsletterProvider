namespace Data.Entities;

public class NewsletterSubscriberEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Email { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
}
