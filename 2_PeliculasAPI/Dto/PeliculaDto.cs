using _2_PeliculasAPI.Helpers;
using _2_PeliculasAPI.Helpers.Validations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace _2_PeliculasAPI.Dto
{
    public class PeliculaBaseDto
    {
        [JsonProperty(Order = 10)]
        [Required]
        [StringLength(300)]
        public string? Titulo { get; set; }

        [JsonProperty(Order = 11)]
        public bool EnCines { get; set; }

        [JsonProperty(Order = 12)]
        public DateTime FechaEstreno { get; set; }
       
    }

    public class PeliculaGetDto : PeliculaBaseDto
    {
        [JsonProperty(Order = 1)]
        public int Id { get; set; }

        [JsonProperty(Order = 20)]
        public string? Poster { get; set; }
    }

    public class PeliculaPostDto : PeliculaBaseDto
    {
        [JsonProperty(Order = 20)]
        [ModelBinder(BinderType = typeof(TypeBinder<List<int>>))]
        public List<int>? GenerosIds { get; set; }

        [JsonProperty(Order = 21)]
        [ModelBinder(BinderType = typeof(TypeBinder<List<ActorPeliculaPostDto>>))]
        public List<ActorPeliculaPostDto>? Actores { get; set; }

        [JsonProperty(Order = 22)]
        [PesoImagenValidation(pesoMaximoMb: 2)]
        [TipoArchivoValidation(GrupoTipoArchivo.Imagen)]
        public IFormFile? Poster { get; set; }

    }

    public class PeliculaPatchDto : PeliculaBaseDto
    {

    }

    public class PeliculaIndexDto
    {
        public List<PeliculaGetDto>? FuturosEstrenos { get; set; }
        public List<PeliculaGetDto>? EnCines { get; set; }

    }

    public class FiltroPeliculaDto
    {
        public int Pagina { get; set; }
        public int CantidadRegistrosPorPagina { get; set; }
        public PaginacionDto? Paginacion 
        {
            get 
            { 
                return new PaginacionDto() { Pagina = Pagina, CantidadRegistrosPorPagina = CantidadRegistrosPorPagina };
            } 
        }
        public string? Titulo { get; set; }
        public int GeneroId { get; set; }
        public bool EnCines { get; set; }
        public bool ProximosExtrenos { get; set; }
        public string? CampoOrdenar { get; set; }
        public bool OrdenAscendente { get; set; } = true;

    }

    public class PeliculaDetalleDto : PeliculaGetDto
    {
        [JsonProperty(Order = 50)]
        public List<GeneroGetDto>? Generos { get; set; }

        [JsonProperty(Order = 51)]
        public List<ActorPeliculaDetalleDto>? Actores { get; set; }
    }
    
}
