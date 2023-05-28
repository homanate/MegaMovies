using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MegaMovies.Data;
using MegaMovies.Models;
using MegaMovies.ViewModels;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing.Constraints;
using System.Data;

namespace MegaMovies.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private const string DefaultImageFilePath = "Camera.png";

        public MoviesController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
              return _context.Movies != null ? 
                          View(await _context.Movies.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Movies'  is null.");
        }

        // GET: Movies/Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Movies == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id);

            var movieViewModel = new MovieViewModel()
            {
                Id = movie.Id,
                MovieTitle = movie.MovieTitle,
                Director = movie.Director,
                ReleaseDate = movie.ReleaseDate,
                ExistingImage = movie.MoviePoster
            };

            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MovieTitle,Director,ReleaseDate,PosterFile")] MovieModel movieModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    movieModel.MoviePoster = ProcessUploadedFile(movieModel);

                    //Insert record
                    _context.Add(movieModel);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "Movie added successfully";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DataException dex)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(movieModel);
        }

        // GET: Movies/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Movies == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies.FindAsync(id);
            var MovieModel = new MovieModel()
            {
                Id = movie.Id,
                MovieTitle = movie.MovieTitle,
                Director = movie.Director,
                ReleaseDate = movie.ReleaseDate,
                MoviePoster = movie.MoviePoster
            };

            if (movie == null)
            {
                return NotFound();
            }
            return View(MovieModel);
        }

        // POST: Movies/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MovieModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var movie = await _context.Movies.FindAsync(model.Id);
                    movie.MovieTitle = model.MovieTitle;
                    movie.Director = model.Director;
                    movie.ReleaseDate = model.ReleaseDate;

                    if (movie.MoviePoster != null && movie.MoviePoster != "Camera.png")
                    {
                        string deleteFileFromFolder = Path.Combine(_environment.WebRootPath, "Uploads");
                        var CurrentImage = Path.Combine(Directory.GetCurrentDirectory(), deleteFileFromFolder, movie.MoviePoster);

                        if (System.IO.File.Exists(CurrentImage))
                        {
                            System.IO.File.Delete(CurrentImage);
                        }

                        movie.MoviePoster = ProcessUploadedFile(model);

                    }
                    else
                    {
                        if (movie != null)
                        {
                            movie.MoviePoster = ProcessUploadedFile(model);                           
                        }
                    }
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "Movie updated successfully";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        // GET: Movies/Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Movies == null)
            {
                return NotFound();
            }

            var movieModel = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movieModel == null)
            {
                return NotFound();
            }

            return View(movieModel);
        }

        // POST: Movies/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Movies == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Movies'  is null.");
            }

            var movie = await _context.Movies.FindAsync(id);

            if (movie.MoviePoster != null && movie.MoviePoster != "Camera.png")
            {
                string deleteFileFromFolder = Path.Combine(_environment.WebRootPath, "Uploads");
                var CurrentImage = Path.Combine(Directory.GetCurrentDirectory(), deleteFileFromFolder, movie.MoviePoster);
                _context.Movies.Remove(movie);
                if (System.IO.File.Exists(CurrentImage))
                {
                    System.IO.File.Delete(CurrentImage);
                }
            }
            else
            {
                if (movie != null)
                {
                    _context.Movies.Remove(movie);
                }
            }

            await _context.SaveChangesAsync();
            TempData["success"] = "Movie deleted successfully";
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
          return (_context.Movies?.Any(e => e.Id == id)).GetValueOrDefault();
        }


        private string ProcessUploadedFile(MovieModel model)
        {
            string uniqueFileName = null;
            string path = Path.Combine(_environment.WebRootPath, "Uploads");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (model.PosterFile != null)
            {

                //Save image to wwwroot/ Uploads
                string wwwRootPath = Path.Combine(_environment.WebRootPath, "Uploads");
                uniqueFileName = Path.GetFileNameWithoutExtension(model.PosterFile.FileName);
                string extension = Path.GetExtension(model.PosterFile.FileName);
                model.MoviePoster = uniqueFileName = uniqueFileName + DateTime.Now.ToString("yymmssfff") + extension;
                string filePath = Path.Combine(wwwRootPath, model.MoviePoster);
                using (var filestream = new FileStream(filePath, FileMode.Create))
                {
                    model.PosterFile.CopyTo(filestream);
                }
            }
            else
            {
                model.MoviePoster = uniqueFileName = DefaultImageFilePath;
            }

            return uniqueFileName;
        }
    }
}

