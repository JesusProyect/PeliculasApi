using _2_PeliculasAPI.Dto;
using _2_PeliculasAPI.Entities;
using _2_PeliculasAPI.Helpers;
using _2_PeliculasAPI.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _2_PeliculasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomBaseController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CustomBaseController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        protected async Task<List<TDTO>> Get<TEntidad, TDTO>() where TEntidad : class
        {
            var entidades = await _context.Set<TEntidad>().AsNoTracking().ToListAsync();
            var dtos = _mapper.Map<List<TDTO>>(entidades);
            return dtos;
        }

        protected async Task<List<TDTO>> Get<TEntidad, TDTO>(PaginacionDto paginacionDto) where TEntidad : class
        {
            var queryable = _context.Set<TEntidad>().AsQueryable();
            return await Get<TEntidad, TDTO>(paginacionDto, queryable);
           
        }

        protected async Task<List<TDTO>> Get<TEntidad, TDTO>(PaginacionDto paginacionDto, IQueryable<TEntidad> queryable) where TEntidad : class
        {
            await HttpContext.InsertarParametrosPaginacion(queryable, paginacionDto);
            var entidades = await queryable.Paginar(paginacionDto).ToListAsync();
            return _mapper.Map<List<TDTO>>(entidades);
        }

        protected async Task<ActionResult<TDTO>> Get<TEntidad, TDTO>(int id) where  TEntidad : class , IId
        {
            var entidad = await _context.Set<TEntidad>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

            if (entidad is null) return NotFound();

            return _mapper.Map<TDTO>(entidad);
        }

        protected async Task<ActionResult> Post<TCreacion, TEntidad, Tlectura>(TCreacion creacionDto, string nombreRuta) where TEntidad : class, IId
        {
            var entidad = _mapper.Map<TEntidad>(creacionDto);
            _context.Add(entidad);
            await _context.SaveChangesAsync();
            var dtoLectura = _mapper.Map<Tlectura>(entidad);

            return CreatedAtRoute(nombreRuta, new { Id = entidad.Id }, dtoLectura);
        }

        protected async Task<ActionResult> Put<TCreacion, TEntidad>(int id, TCreacion creacionDto) where TEntidad : class, IId
        {
            var entidad = _mapper.Map<TEntidad>(creacionDto);
            entidad.Id = id;
            _context.Entry(entidad).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        protected async Task<ActionResult> Patch<TEntidad, TDTO>(int id , JsonPatchDocument<TDTO> patchDocument) 
            where TDTO : class 
            where TEntidad : class, IId
        {
            if (patchDocument is null) return BadRequest();
            var entidadDb = await _context.Set<TEntidad>().FirstOrDefaultAsync(x => x.Id == id);

            if (entidadDb is null) return NotFound();

            var entidadDto = _mapper.Map<TDTO>(entidadDb);

            patchDocument.ApplyTo(entidadDto, ModelState);

            bool esValido = TryValidateModel(entidadDto);

            if (!esValido) return BadRequest(ModelState);

            _mapper.Map(entidadDto, entidadDb);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        protected async Task<ActionResult> Delete<TEntidad> (int id) where TEntidad : class, IId, new()
        {
            bool existe = await _context.Set<TEntidad>().AnyAsync(x => x.Id == id);
            if (!existe) return NotFound();

            _context.Remove(new TEntidad() { Id = id });
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
