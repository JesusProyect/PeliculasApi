using System.ComponentModel.DataAnnotations;
using _2_PeliculasAPI.Interfaces;

namespace _2_PeliculasAPI.Entities
{
    public class Actor : IId
    {
        public int Id{ get; set; }

        [Required]
        [StringLength(120)]
        public string? Nombre { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string? Foto { get; set; }
        public List<PeliculaActor>? PeliculasActores{ get; set; }
    }
}
