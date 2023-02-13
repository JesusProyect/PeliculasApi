using _2_PeliculasAPI.Helpers.Validations;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace _2_PeliculasAPI.Dto
{
    public class ActorBaseDto
    {
        [JsonProperty(Order = 10)]
        [Required]
        [StringLength(120)]
        public string? Nombre { get; set; }

        [JsonProperty(Order = 11)]
        public DateTime FechaNacimiento { get; set; }
    }
    public class ActorGetDto : ActorBaseDto
    {
        [JsonProperty(Order = 1)]
        public int Id { get; set; }

        [JsonProperty(Order = 20)]
        public string? Foto { get; set; }
    }

    public class ActorPostDto : ActorBaseDto
    {
        [JsonProperty(Order = 20)]
        [PesoImagenValidation(pesoMaximoMb:  2 )]
        [TipoArchivoValidation(grupoTipoArchivo: GrupoTipoArchivo.Imagen)]
        public IFormFile? Foto { get; set; }
    }

    public class ActorPatchDto : ActorBaseDto{}

    public class ActorPeliculaDetalleDto
    {
        public int ActorId { get; set; }
        public string? Personaje { get; set; }
        public string? NombrePersona { get; set; }
    }
}
