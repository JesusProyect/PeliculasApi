using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace _2_PeliculasAPI.Dto
{
    public class SalaDeCineBaseDto
    {
        [JsonProperty(Order = 10)]
        [Required]
        [StringLength(120)]
        public string? Nombre { get; set; }

        [JsonProperty(Order = 11)]
        [Range(-180,180)]
        public double Longitud{ get; set; }

        [JsonProperty(Order = 12)]
        [Range(-90, 90)]
        public double Latitud { get; set; }
    }
    public class SalaDeCineGetDto: SalaDeCineBaseDto
    {
        [JsonProperty(Order = 1)]
        public int Id { get; set; }
       
    }

    public class SalaDeCinePostDto : SalaDeCineBaseDto
    {

    }

    public class SalaDeCineCercanoFiltroDto
    {
        private int _distanciaEnKms = 10;
        private int _distanciaMaximaEnKms = 50;

        [Range(-180, 180)]
        public double Longitud { get; set; }

        [Range(-90, 90)]
        public double Latitud { get; set; }

        public int DistanciaEnKms
        {
            get { return _distanciaEnKms; }
            set
            {
                _distanciaEnKms = (value > _distanciaMaximaEnKms) ? _distanciaMaximaEnKms : value ;
            }
        }
 
    }

    public class SalaDeCineCercanoDto : SalaDeCineGetDto
    {
        [JsonProperty(Order = 20)]
       public double DistanciaEnMetros { get; set; }

    }
}
