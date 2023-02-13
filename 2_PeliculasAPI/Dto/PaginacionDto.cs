namespace _2_PeliculasAPI.Dto
{
    public class PaginacionDto
    {
        private int _pagina = 1;
        private readonly int _cantidadMaximaRegistrosPorPagina = 50;
        private int _cantidadRegistrosPorPagina = 10;


        public int Pagina {
            get => _pagina;
            set 
            {
                _pagina = (value <= 0 ? _pagina : value); 
            }
        }
        public int CantidadRegistrosPorPagina
        {
            get => _cantidadRegistrosPorPagina; 
            set
            {
                _cantidadRegistrosPorPagina = (value <= 0 ? _cantidadRegistrosPorPagina : (value > _cantidadMaximaRegistrosPorPagina) ? _cantidadMaximaRegistrosPorPagina : value);
            }  
        }


    }
}
