using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using is4aspid.Models;
using Microsoft.AspNetCore.Identity;

namespace is4aspid.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>().Property(u => u.UserName).HasMaxLength(128);
            //modelBuilder.Entity<ApplicationUser>().Property(u => u.Email).HasMaxLength(128);
            builder.Entity<IdentityRole>().Property(r => r.Name).HasMaxLength(128);
            builder.Entity<IdentityRole>().Property(r => r.NormalizedName).HasMaxLength(128);

            builder.Entity<ApplicationUser>().Property(r => r.NormalizedUserName).HasMaxLength(128);
        }
    }
}
