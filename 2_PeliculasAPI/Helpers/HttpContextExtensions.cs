using _2_PeliculasAPI.Dto;
using Microsoft.EntityFrameworkCore;

namespace _2_PeliculasAPI.Helpers
{
    public static class HttpContextExtensions
    {
        public async static Task InsertarParametrosPaginacion<T>(this HttpContext httpContext, IQueryable<T> queryable, PaginacionDto paginacionDto)
        {
            double cantidad = await queryable.CountAsync();
            double cantidadPaginas = Math.Ceiling(cantidad / paginacionDto.CantidadRegistrosPorPagina);
            httpContext.Response.Headers.Add("totalResultados", cantidad.ToString());
            httpContext.Response.Headers.Add("resultadosPorPagina", paginacionDto.CantidadRegistrosPorPagina.ToString());
            httpContext.Response.Headers.Add("paginaActual", paginacionDto.Pagina.ToString());
            httpContext.Response.Headers.Add("cantidadPaginas", cantidadPaginas.ToString());

        }
    }
}
