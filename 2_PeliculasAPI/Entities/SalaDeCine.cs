using _2_PeliculasAPI.Interfaces;
using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;

namespace _2_PeliculasAPI.Entities
{
    public class SalaDeCine  : IId
    {
        public int Id { get; set; }

        [Required]
        [StringLength(120)]
        public string? Nombre { get; set; }
        public Point? Ubicacion { get; set; }
        public List<PeliculaSalaDeCine>? PeliculasSalasDeCine{ get; set; }
    }
}
