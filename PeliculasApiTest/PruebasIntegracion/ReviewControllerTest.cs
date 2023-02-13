using _2_PeliculasAPI.Dto;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.WebSockets;

namespace PeliculasApiTest.PruebasIntegracion
{
    public class ReviewControllerTest  : BasePrueba
    {

        private static readonly string url = "/api/pelicula/1/review";

                 [Fact]
        public async Task ObtenerReview_PeliculaInexistente_ShouldBe_NotFOund()
        {
            //Arrange
            var nombreBd = Guid.NewGuid().ToString();
            var factory = ConstruirWebAplicationFactory(nombreBd);

            var cliente = factory.CreateClient();

            //Act
            var respuesta = await cliente.GetAsync(url);

            //Assert
            respuesta.ReasonPhrase.Should().Be("Not Found");
        }

        [Fact]
        public async Task ObtenerReview_PeliculaInexistente_ShouldBe_EmptyList()
        {
            //Arrange
            var nombreBd = Guid.NewGuid().ToString();
            var factory = ConstruirWebAplicationFactory(nombreBd);
            var context = ConstruirContext(nombreBd);
            context.Peliculas!.Add(new() { Titulo = " Pelicula 1"});
            await context.SaveChangesAsync();

            var cliente = factory.CreateClient();

            //Act
            var respuesta = await cliente.GetAsync(url);

            //Assert
            respuesta.EnsureSuccessStatusCode();
            var reviews = JsonConvert.DeserializeObject<List<ReviewGetDto>>(await respuesta.Content.ReadAsStringAsync());
            reviews.Should().BeEmpty();
        }

    }
}
