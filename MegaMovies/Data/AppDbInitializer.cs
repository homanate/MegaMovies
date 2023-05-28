using MegaMovies.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MegaMovies.Data
{
    public class AppDbInitializer
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                context.Database.EnsureCreated();

                if (!context.Movies.Any())
                {
                    context.Movies.AddRange(new List<MovieModel>()
                    {
                        new MovieModel()
                        {
                            //Id = 1,
                            MovieTitle = "Aliens",
                            Director = "James Cameron",
                            ReleaseDate = new DateTime(1986,07,18),
                            MoviePoster = "Aliens.jpg"
                        },
                        new MovieModel()
                        {
                            //Id = 3,
                            MovieTitle = "Gladiator",
                            Director = "Ridley Scott",
                            ReleaseDate = new DateTime(2000,05,05),
                            MoviePoster = "Gladiator.jpg"
                        },
                        new MovieModel()
                        {
                            //Id = 4,
                            MovieTitle = "Dumb and Dumber",
                            Director = "Peter Farrelly",
                            ReleaseDate = new DateTime(1994,12,16),
                            MoviePoster = "DumbAndDumber.jpg"
                        }
                    });
                    context.SaveChanges();
                }
            }
        }
    }
}
