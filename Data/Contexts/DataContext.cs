
using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Contexts;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }
    public DbSet<NewsletterSubscriberEntity> NewsletterSubscribers { get; set; }
}
