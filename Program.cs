using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using BackEndPeliculas.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace BackEndPeliculas
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<MoviesContext>();
                    context.Database.EnsureCreated();
                    context.Database.Migrate();
                    List<Movie> movies = context.Movie.ToList();
                    if (movies.Count() == 0)
                    {
                        context.Movie.AddRange(
                            new Movie { Title = "Los caza fantasmas", Genre = "Comedia", ReleaseDate = DateTime.Parse("1889 - 12 - 12"), Price = 20 },
                            new Movie { Title = "Los caza fantasmas 2", Genre = "Comedia", ReleaseDate = DateTime.Parse("1891 - 12 - 12"), Price = 10 },
                            new Movie { Title = "Volver al futuro", Genre = "Ciencia Ficción", ReleaseDate = DateTime.Parse("1887 - 1 - 1"), Price = 50 },
                            new Movie { Title = "Volver al futuro 2", Genre = "Ciencia Ficción", ReleaseDate = DateTime.Parse("1889 - 3 - 12"), Price = 45 },
                            new Movie { Title = "Rapido y furioso", Genre = "Acción", ReleaseDate = DateTime.Parse("1998 - 12 - 2"), Price = 5 }
                            );
                        context.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Error al momento de inicializar la base de datos, revise el metodo");
                }
            }
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel()
                    //.UseUrls("https://*:44320")
                    .UseIISIntegration()
                    .UseStartup<Startup>();
                });
    }
}
