using System.ComponentModel.DataAnnotations;

namespace _2_PeliculasAPI.Dto
{
    public class UsuarioGetDto //GET CREO
    {
        public string? Id { get; set; }
        public string? Email{ get; set; }
    }

    public class EditarRolDto
    {
        public string? UsuarioId { get; set; }
        public string? NombreRol { get; set; }
    }
    public class UserToken
    {
        public string? Token { get; set; }
        public DateTime Expiracion { get; set; }
    }

    public class UserInfo
    {
        [Required]
        public string? Email { get; set; }
        
        [Required]
        public string? Password { get; set; }
    }

    
}
