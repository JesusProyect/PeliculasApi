using System.ComponentModel.DataAnnotations;

namespace _2_PeliculasAPI.Dto
{
    //este vale para el get y el get byID
    public class GeneroGetDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(40)]
        public string? Nombre { get; set; }
    }

    //este sirve para post y put
    public class GeneroPostDto
    {
        [Required]
        [StringLength(40)]
        public string? Nombre { get; set; }

    }


}
