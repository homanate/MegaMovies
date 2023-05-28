using MegaMovies.Models;
using Microsoft.EntityFrameworkCore;

namespace MegaMovies.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<MovieModel> Movies { get; set; }
    }
}
