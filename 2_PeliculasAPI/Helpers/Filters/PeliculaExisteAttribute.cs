using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace _2_PeliculasAPI.Helpers.Filters
{
    public class PeliculaExisteAttribute : Attribute, IAsyncResourceFilter
    {
        private readonly ApplicationDbContext _context;

        public PeliculaExisteAttribute( ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            var peliculaIdObject = context.HttpContext.Request.RouteValues["peliculaId"];

            if (peliculaIdObject == null) return;

            var peliculaId = int.Parse(peliculaIdObject.ToString()!);

            var existePelicula = await _context.Peliculas!.AnyAsync(p => p.Id == peliculaId);
            
            if (!existePelicula) context.Result = new NotFoundResult();
            else { await next(); }
        }
    }
}
