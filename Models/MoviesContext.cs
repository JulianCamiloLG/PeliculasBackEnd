using Microsoft.EntityFrameworkCore;

namespace BackEndPeliculas.Models
{
    public class MoviesContext : DbContext
    {
        public MoviesContext(DbContextOptions<MoviesContext> options) : base(options)
        {
        }
        public DbSet<Movie> Movie { get; set; }
        public DbSet<Peliculas> Peliculas { get; set; }
    }
}
