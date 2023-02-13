using System.ComponentModel.DataAnnotations;
using _2_PeliculasAPI.Interfaces;

namespace _2_PeliculasAPI.Entities
{
    public class Pelicula  : IId
    {
        public int Id { get; set; }

        [Required]
        [StringLength(300)]
        public string? Titulo { get; set; }
        public bool EnCines { get; set; }
        public DateTime FechaEstreno { get; set; }
        public string? Poster { get; set; }
        public List<PeliculaActor>? PeliculasActores { get; set; }
        public List<PeliculaGenero>? PeliculasGeneros { get; set; }
        public List<PeliculaSalaDeCine>? PeliculasSalasDeCine { get; set; }

    }
}
