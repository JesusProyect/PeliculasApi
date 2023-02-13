using _2_PeliculasAPI.Dto;

namespace _2_PeliculasAPI.Helpers
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> Paginar<T>(this IQueryable<T> queryable, PaginacionDto paginacionDto)
        {
            return queryable
                .Skip((paginacionDto.Pagina - 1) * paginacionDto.CantidadRegistrosPorPagina)
                .Take(paginacionDto.CantidadRegistrosPorPagina);
        }
    }
}
