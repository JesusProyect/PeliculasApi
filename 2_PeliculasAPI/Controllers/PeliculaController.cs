using _2_PeliculasAPI.Dto;
using _2_PeliculasAPI.Entities;
using _2_PeliculasAPI.Helpers;
using _2_PeliculasAPI.Services;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace _2_PeliculasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeliculaController : CustomBaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAlmacenadorArchivos _almacenadorArchivos;
        private readonly ILogger<PeliculaController> _logger;
        private readonly string _contenedor = "peliculas";

        public PeliculaController(ApplicationDbContext context, IMapper mapper, IAlmacenadorArchivos almacenadorArchivos, ILogger<PeliculaController> logger) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
            _almacenadorArchivos = almacenadorArchivos;
            _logger = logger;
        }


        #region GET

        [HttpGet]  //esto es el primer get index para cuadno abres la pagina te muestra resultados
        public async Task<ActionResult<PeliculaIndexDto>> Get()
        {
            var top = 5;
            var hoy = DateTime.Today;

            var proximosExtrenos = await _context.Peliculas!.
                Where(x => x.FechaEstreno > hoy)
                .OrderBy(p => p.FechaEstreno)
                .Take(top)
                .ToListAsync();

            var enCines = await _context.Peliculas!
                .Where(p => p.EnCines)
                .Take(top)
                .ToListAsync();

            PeliculaIndexDto resultado = new();
            resultado.FuturosEstrenos = _mapper.Map<List<PeliculaGetDto>>(proximosExtrenos);
            resultado.EnCines = _mapper.Map<List<PeliculaGetDto>>(enCines);

            return Ok(resultado);

            // var peliculas = await _context.Peliculas!.ToListAsync();
            //return Ok(_mapper.Map<List<PeliculaGetDto>>(peliculas));
        }

        [HttpGet("filtro")]
        public async Task<ActionResult<List<PeliculaGetDto>>> Filtrar([FromQuery] FiltroPeliculaDto filtroPeliculaDto)
        {
            var peliculasQueryable = _context.Peliculas!.AsQueryable();

            if (!string.IsNullOrEmpty(filtroPeliculaDto.Titulo)) peliculasQueryable = peliculasQueryable.Where(p => p.Titulo!.Contains(filtroPeliculaDto.Titulo));
            if (filtroPeliculaDto.EnCines) peliculasQueryable = peliculasQueryable.Where(p => p.EnCines);
            if (filtroPeliculaDto.ProximosExtrenos) 
            {
                var hoy = DateTime.Today;
                 peliculasQueryable = peliculasQueryable.Where(p => p.FechaEstreno > hoy);

            }
            if (filtroPeliculaDto.GeneroId != 0) peliculasQueryable = peliculasQueryable.Where(p => p.PeliculasGeneros!
                                                                                .Select(genero => genero.GeneroId)
                                                                                .Contains(filtroPeliculaDto.GeneroId));

            if (!string.IsNullOrEmpty(filtroPeliculaDto.CampoOrdenar))
            {
                var tipoOrden = filtroPeliculaDto.OrdenAscendente ? "ascending" : "descending";

                try
                {
                    peliculasQueryable = peliculasQueryable.OrderBy($"{filtroPeliculaDto.CampoOrdenar} {tipoOrden}");
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex.Message, ex);
                    return BadRequest("El Campo por el que se quiere ordenar no existe");
                }
            }
            await HttpContext.InsertarParametrosPaginacion(peliculasQueryable, filtroPeliculaDto.Paginacion! );

            var peliculas = await peliculasQueryable.Paginar(filtroPeliculaDto.Paginacion!).ToListAsync();

            return _mapper.Map<List<PeliculaGetDto>>(peliculas);

        }


        [HttpGet("{id:int}", Name = "obtenerPeliculaById")]
        public async Task<ActionResult<PeliculaDetalleDto>> Get(int id)
        {
            Pelicula? pelicula = await _context.Peliculas!
                .Include(p => p.PeliculasActores!).ThenInclude(pa => pa.Actor)
                .Include(p => p.PeliculasGeneros!).ThenInclude(pg => pg.Genero)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (pelicula is null) return NotFound();
            pelicula.PeliculasActores = pelicula.PeliculasActores!.OrderBy(pa => pa.Orden).ToList();
           
            return Ok(_mapper.Map<PeliculaDetalleDto>(pelicula));

        }

        #endregion

        #region POST

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] PeliculaPostDto peliculaPostDto)
        {
            Pelicula pelicula = _mapper.Map<Pelicula>(peliculaPostDto);

            if (peliculaPostDto.Poster is not null)
            {
                using MemoryStream memoryStream = new();
                await peliculaPostDto.Poster.CopyToAsync(memoryStream);
                byte[] contenido = memoryStream.ToArray();
                string extension = Path.GetExtension(peliculaPostDto.Poster.FileName);

                pelicula.Poster = await _almacenadorArchivos.GuardarArchivo(contenido, extension, _contenedor, peliculaPostDto.Poster.ContentType);
            }

            _context.Add(pelicula);
            await _context.SaveChangesAsync();

            PeliculaGetDto peliculaGetDto = _mapper.Map<PeliculaGetDto>(pelicula);

            return CreatedAtRoute("obtenerPeliculaById", new { peliculaGetDto.Id }, peliculaGetDto);
        }

        #endregion

        #region PUT

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromForm] PeliculaPostDto peliculaPostDto)
        {
            Pelicula? pelicula = await _context.Peliculas!
                .Include(x => x.PeliculasGeneros)
                .Include(x => x.PeliculasActores)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (pelicula is null) return NotFound();

            pelicula = _mapper.Map(peliculaPostDto, pelicula); //modifica los campos distintos lo actualiza 

            if (peliculaPostDto.Poster is not null)
            {
                using MemoryStream memoryStream = new();
                await peliculaPostDto.Poster.CopyToAsync(memoryStream);
                byte[] contenido = memoryStream.ToArray();
                string extension = Path.GetExtension(peliculaPostDto.Poster.FileName);

                pelicula.Poster= await _almacenadorArchivos.EditarArchivo(contenido, extension, _contenedor, pelicula.Poster!, peliculaPostDto.Poster.ContentType);
            }

            await _context.SaveChangesAsync(); // guarda los campos modificados despues del mapeo no la entidad entera

            return NoContent();
        }
        #endregion

        #region PATCH

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<PeliculaPatchDto> patchDocument)
        {
           return await Patch<Pelicula, PeliculaPatchDto>(id, patchDocument);

        }
        #endregion

        #region DELETE
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
           return await Delete<Pelicula>(id);


        }
        #endregion


    }
}
