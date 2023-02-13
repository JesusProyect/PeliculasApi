namespace _2_PeliculasAPI.Services
{
    public interface IAlmacenadorArchivos
    {
        Task<string> GuardarArchivo(byte[] contenido, string extension, string contenedor, string contenType);
        Task<string> EditarArchivo(byte[] contenido, string extension, string contenedor, string ruta, string contenType);
        Task BorrarArchivo(string ruta, string contenedor);


    }
}
