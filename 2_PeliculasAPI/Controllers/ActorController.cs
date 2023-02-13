using _2_PeliculasAPI.Dto;
using _2_PeliculasAPI.Entities;
using _2_PeliculasAPI.Services;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch.Adapters;
using _2_PeliculasAPI.Helpers;

namespace _2_PeliculasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActorController : CustomBaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAlmacenadorArchivos _almacenadorArchivos;
        private readonly string _contenedor = "actores";

        public ActorController(ApplicationDbContext context, IMapper mapper, IAlmacenadorArchivos almacenadorArchivos) : base(context , mapper)
        {
            _context = context;
            _mapper = mapper;
            _almacenadorArchivos = almacenadorArchivos;
        }

        #region GET
        [HttpGet]
        public async Task<ActionResult<List<ActorGetDto>>> Get([FromQuery] PaginacionDto paginacionDto)
        {
           return await Get<Actor, ActorGetDto>(paginacionDto);
        }

        [HttpGet("{id:int}", Name = "obtenerActorById")]
        public async Task<ActionResult<ActorGetDto>> Get(int id)
        {
            return await Get<Actor,ActorGetDto>(id);
        }
        #endregion

        #region POST

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] ActorPostDto actorPostDto)
        {
            Actor actor = _mapper.Map<Actor>(actorPostDto);

            if(actorPostDto.Foto is not null)
            {
                using MemoryStream memoryStream = new();
                await actorPostDto.Foto.CopyToAsync(memoryStream);
                byte[] contenido = memoryStream.ToArray();
                string extension = Path.GetExtension(actorPostDto.Foto.FileName);

                actor.Foto = await _almacenadorArchivos.GuardarArchivo(contenido, extension, _contenedor, actorPostDto.Foto.ContentType);
            }

            _context.Add(actor);
            await _context.SaveChangesAsync();

            ActorGetDto actorDto = _mapper.Map<ActorGetDto>(actor);
            
            return CreatedAtRoute("obtenerActorById", new { actorDto.Id }, actorDto);
        }

        #endregion

        #region PUT

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put (int id, [FromForm] ActorPostDto actorPostDto)
        {
            Actor? actor = await _context.Actores!.FirstOrDefaultAsync(x => x.Id == id);

            if (actor is null) return NotFound();

            actor = _mapper.Map(actorPostDto, actor); //modifica los campos distintos lo actualiza 

            if (actorPostDto.Foto is not null)
            {
                using MemoryStream memoryStream = new();
                await actorPostDto.Foto.CopyToAsync(memoryStream);
                byte[] contenido = memoryStream.ToArray();
                string extension = Path.GetExtension(actorPostDto.Foto.FileName);

                actor.Foto = await _almacenadorArchivos.EditarArchivo(contenido, extension, _contenedor, actor.Foto!,  actorPostDto.Foto.ContentType);
            }

            await _context.SaveChangesAsync(); // guarda lso campos modificados despues del mapeo no la entidad entera

            return NoContent();
        }

        #endregion

        #region PATCH

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<ActorPatchDto> patchDocument)
        {
            return await Patch<Actor, ActorPatchDto>(id, patchDocument);

        }

        #endregion

        #region DELETE

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
           return   await Delete<Actor>(id);

        }

        #endregion
    }
}
