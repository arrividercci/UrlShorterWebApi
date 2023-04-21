using Microsoft.EntityFrameworkCore;
using UrlShorterServiceWebApi.Entities;

namespace UrlShorterServiceWebApi.Data
{
    public class UrlShorterContext : DbContext
    {
        public UrlShorterContext(DbContextOptions<UrlShorterContext> options) : base(options) { }
        
        public DbSet<Url> Urls { get; set; }
        public DbSet<UserUrls> UserUrls { get; set; }
    }
}
