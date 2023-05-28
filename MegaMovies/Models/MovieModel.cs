using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MegaMovies.Models
{
    public class MovieModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DisplayName("Movie Title")]
        public string MovieTitle { get; set; }

        [Required]
        public string Director { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayName("Release Date")]
        public DateTime ReleaseDate { get; set; }


        [DisplayName("Movie Poster")]
        public string? MoviePoster { get; set; }

        [NotMapped]
        [DisplayName("Upload File")]
        public IFormFile? PosterFile { get; set; }
    }
}
