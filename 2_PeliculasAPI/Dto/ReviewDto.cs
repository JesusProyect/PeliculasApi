using System.ComponentModel.DataAnnotations;

namespace _2_PeliculasAPI.Dto
{
    public class ReviewGetDto
    {
        public int Id { get; set; }
        public string? Comentario { get; set; }
        public int Puntuacion { get; set; }
        public int PeliculaId { get; set; }
        public string? UsuarioId { get; set; }
        public string? NombreUsuario { get; set; }

    }

    public class ReviewPostDto
    {
        public string? Comentario { get; set; }
        [Range(1,5)]
        public int Puntuacion { get; set; }
    }
}
