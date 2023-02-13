using _2_PeliculasAPI.Dto;
using _2_PeliculasAPI.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace _2_PeliculasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalaDeCineController : CustomBaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly GeometryFactory _geometryFactory;

        public SalaDeCineController(ApplicationDbContext context, IMapper mapper, GeometryFactory geometryFactory) : base(context, mapper)
        {
            _context = context;
            _geometryFactory = geometryFactory;
        }

        [HttpGet]
        public async Task<ActionResult<List<SalaDeCineGetDto>>> Get()
        {
            return await Get<SalaDeCine, SalaDeCineGetDto>();
        }

        [HttpGet("{Id:int}", Name ="obtenerSalaCine")]
        public async Task<ActionResult<SalaDeCineGetDto>> Get(int id)
        {
            return await Get<SalaDeCine, SalaDeCineGetDto>(id);
        }

        [HttpGet("cercanos")]
        public async Task<ActionResult<List<SalaDeCineCercanoDto>>> Cercanos([FromQuery] SalaDeCineCercanoFiltroDto salaDeCineCercanoFiltroDto)
        {
            var ubicacionUsuario = _geometryFactory.CreatePoint(new Coordinate(salaDeCineCercanoFiltroDto.Longitud, salaDeCineCercanoFiltroDto.Latitud));

            var salasDeCine = await _context.SalasDeCines!
                .OrderBy(sdc => sdc.Ubicacion!.Distance(ubicacionUsuario))
                .Where(sdc => sdc.Ubicacion!.IsWithinDistance(ubicacionUsuario, salaDeCineCercanoFiltroDto.DistanciaEnKms * 1000))
                .Select(x => new SalaDeCineCercanoDto
                {
                    Id  = x.Id,
                    Nombre = x.Nombre,
                    Longitud = x.Ubicacion!.Y,
                    Latitud = x.Ubicacion!.X,
                    DistanciaEnMetros = Math.Round(x.Ubicacion.Distance(ubicacionUsuario))

                })
                .ToListAsync();

            return salasDeCine;
        }                                           

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] SalaDeCinePostDto salaDeCinePostDto)
        {
            return await Post<SalaDeCinePostDto, SalaDeCine, SalaDeCineGetDto>(salaDeCinePostDto, "obtenerSalaCine");
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody]SalaDeCinePostDto salaDeCinePostDto)
        {
            return await Put<SalaDeCinePostDto, SalaDeCine>(id, salaDeCinePostDto);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            return await Delete<SalaDeCine>(id);
        }

    }
}
