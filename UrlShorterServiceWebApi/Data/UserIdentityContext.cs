using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UrlShorterServiceWebApi.Entities;

namespace UrlShorterServiceWebApi.Data
{
    public class UserIdentityContext : IdentityDbContext<User>
    {
        public UserIdentityContext(DbContextOptions<UserIdentityContext> options) : base(options) {}
    }
}
