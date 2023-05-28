using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MegaMovies.ViewModels
{
    public class UploadImageViewModel
    {
        [Required]
        [Display(Name = "Upload File")]
        public IFormFile? PosterFile { get; set; }
    }
}
