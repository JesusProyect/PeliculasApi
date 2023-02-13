namespace _2_PeliculasAPI.Services
{
    public class AlmacenadorArchivoLocal : IAlmacenadorArchivos
    {
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContext;

        public AlmacenadorArchivoLocal(IWebHostEnvironment env, IHttpContextAccessor httpContext)
        {
            _env = env;
            _httpContext = httpContext;
        }

        public Task BorrarArchivo(string ruta, string contenedor)
        {
            if (ruta is not null)
            {
                string nombreArchivo = Path.GetFileName(ruta);
                string directorioArchivo = Path.Combine(_env.WebRootPath, contenedor, nombreArchivo); //la direccion donde esta el archivo

                if (File.Exists(directorioArchivo)) File.Delete(directorioArchivo);  //borramos el archivo solo --> directorio/nombrearchivo

            }

            return Task.FromResult(0); // esto es solo para que no se queje no hago nada con el
        }           

        public async Task<string> EditarArchivo(byte[] contenido, string extension, string contenedor,string ruta, string contenType)
        {
            await BorrarArchivo(ruta, contenedor);
            return await GuardarArchivo(contenido, extension, contenedor, contenType);

        }

        public async Task<string> GuardarArchivo(byte[] contenido, string extension, string contenedor, string contenType)
        {
            string nombreArchivo = $"{Guid.NewGuid()}{extension}";
            string folder = Path.Combine(_env.WebRootPath, contenedor);

            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            string ruta = Path.Combine(folder, nombreArchivo);
            await File.WriteAllBytesAsync(ruta,contenido);

            string urlActual = $"{_httpContext.HttpContext!.Request.Scheme}://{_httpContext.HttpContext.Request.Host}";
            string urlBd = Path.Combine(urlActual, contenedor, nombreArchivo).Replace("\\", "/");

            return urlBd;
            
        }

    }
}
