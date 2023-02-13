using _2_PeliculasAPI.Dto;
using _2_PeliculasAPI.Entities;
using _2_PeliculasAPI.Helpers.Filters;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace _2_PeliculasAPI.Controllers
{
    [Route("api/pelicula/{peliculaId:int}/[controller]")]
    [ServiceFilter(typeof(PeliculaExisteAttribute))]
    [ApiController]
    public class ReviewController : CustomBaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ReviewController(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<ActionResult<List<ReviewGetDto>>> Get(int peliculaId, [FromQuery] PaginacionDto paginacionDto)
        {
            var queryable = _context.Reviews!.Include(x => x.Usuario).AsQueryable();
            queryable = queryable.Where(x => x.PeliculaId == peliculaId);
            return await Get<Review, ReviewGetDto>(paginacionDto, queryable);
        }

        [Authorize] //saco el id del token
        [HttpPost]
        public async Task<ActionResult> Post(int peliculaId, [FromBody] ReviewPostDto reviewPostDto)
        {
           
            var usuarioId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value;

            var reviewExiste = await _context.Reviews!.AnyAsync(x => x.PeliculaId == peliculaId && x.UsuarioId == usuarioId);
            if (reviewExiste) return BadRequest("El usuario ya ha escrito un review de la pelicula");

            var review = _mapper.Map<Review>(reviewPostDto);
            review.PeliculaId = peliculaId;
            review.UsuarioId = usuarioId;

            _context.Add(review);
            await _context.SaveChangesAsync();

            return NoContent();

        }

        [Authorize]
        [HttpPut("{reviewId:int}")]
        public async Task<ActionResult> Put(int peliculaId, int reviewId, [FromBody]ReviewPostDto reviewPostDto)
        {

            var reviewDb = await _context.Reviews!.FirstOrDefaultAsync(r => r.Id == reviewId);
            if (reviewDb is null) return NotFound();

            var usuarioId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value;
            if (reviewDb.UsuarioId != usuarioId) return BadRequest("No tiene permisos de editar este review"); //solo el usuario que creo el comentario es el que puede actualizarlo, si es diferente retornamos un prohibido

            reviewDb = _mapper.Map(reviewPostDto, reviewDb);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{reviewId:int}")]
        public async Task<ActionResult> Delete(int reviewId)
        {
            var reviewDb = await _context.Reviews!.FirstOrDefaultAsync(r => r.Id == reviewId);
            if (reviewDb is null) return NotFound();

            var usuarioId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value;
            if (reviewDb.UsuarioId != usuarioId) return Forbid();

            _context.Remove(reviewDb);
            await _context.SaveChangesAsync();


            return NoContent();
        }

    }
}
