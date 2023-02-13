using System.ComponentModel.DataAnnotations;
using _2_PeliculasAPI.Interfaces;

namespace _2_PeliculasAPI.Entities
{
    public class Genero : IId
    {
        public int Id{ get; set; }

        [Required]
        [StringLength(40)]
        public string? Nombre{ get; set; }
        public List<PeliculaGenero>? PeliculasGeneros { get; set; }
    }
}
