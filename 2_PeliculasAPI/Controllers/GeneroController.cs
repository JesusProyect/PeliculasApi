using _2_PeliculasAPI.Dto;
using _2_PeliculasAPI.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace _2_PeliculasAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GeneroController : CustomBaseController
    {
        public GeneroController( ApplicationDbContext context, IMapper mapper) : base(context, mapper){}

        [HttpGet]
        public async Task<ActionResult<List<GeneroGetDto>>> Get()
        {
            return await Get<Genero,GeneroGetDto>();
        }

        [HttpGet("{id:int}", Name = "obtenerGenero")]
        public async Task<ActionResult<GeneroGetDto>> Get(int id)
        {
           return await Get<Genero,GeneroGetDto>(id);
        }

        [HttpPost]
        public async Task<ActionResult> Post ([FromBody] GeneroPostDto generoPostDto )
        {
            return await Post<GeneroPostDto, Genero, GeneroGetDto>(generoPostDto, "obtenerGenero");
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put (int id, [FromBody] GeneroPostDto generoPostDto) 
        {
            return await Put<GeneroPostDto, Genero >(id, generoPostDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete (int id)
        {
            return await Delete<Genero>(id);
        }
    }
}
