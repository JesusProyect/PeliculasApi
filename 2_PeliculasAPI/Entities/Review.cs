using _2_PeliculasAPI.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace _2_PeliculasAPI.Entities
{
    public class Review : IId
    {
        public int Id { get; set; }
        public string? Comentario { get; set; }
        public int PeliculaId { get; set; }
        public Pelicula? Pelicula { get; set; }
        public string? UsuarioId { get; set; }
        public IdentityUser? Usuario { get; set; }

        [Range(1, 5)]
        public int Puntuacion { get; set; }
    }
}
