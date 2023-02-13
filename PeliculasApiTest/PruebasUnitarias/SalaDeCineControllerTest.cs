using _2_PeliculasAPI.Controllers;
using _2_PeliculasAPI.Dto;
using AutoMapper;
using FluentAssertions;
using NetTopologySuite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeliculasApiTest.PruebasUnitarias
{
    public class SalaDeCineControllerTest : BasePrueba
    {


        [Fact]
        public async Task ObtenerSalasDeCine_A5Km_oMenos_ShouldBeOk()
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            var filtro = new SalaDeCineCercanoFiltroDto() { DistanciaEnKms = 5, Latitud = 18.481139, Longitud = -69.938950 };

            var contextInitializer = ConstruirContext("PruebasDeIntegracion");
            LocalDbInitializer.Initialize(contextInitializer);

            using var context = LocalDbInitializer.GetDbContextLocalDb(false); //lo edite para que funcione
            var mapper = ConfigurarAutoMapper();
            var controller = new SalaDeCineController(context, mapper, geometryFactory);
            var respuesta = await controller.Cercanos(filtro);

            LocalDbInitializer.End();

            var valor = respuesta.Value;
            valor!.Count.Should().Be(2);

        }

    }
}
