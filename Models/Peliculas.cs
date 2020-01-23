using System;
using System.ComponentModel.DataAnnotations;

namespace BackEndPeliculas.Models
{
    public class Peliculas
    {
        [Required]
        public int ID { get; set; }
        [Required]
        public string Title { get; set; }
        [DataType(DataType.Date)]
        [Required]
        public DateTime ReleaseDate { get; set; }
        [Required]
        public string Genre { get; set; }
        [Required]
        public decimal Price { get; set; }
    }
}
