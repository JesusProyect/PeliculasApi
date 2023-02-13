using _2_PeliculasAPI.Dto;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Policy;

namespace PeliculasApiTest.PruebasIntegracion
{
    public class GeneroControllerTest : BasePrueba
    {
        private static readonly string _url =  "/api/genero";

        [Fact]
        public async Task ObtenerTodosLosGeneros_ShouldBe_ListadoVacio()
        {
            //Arrange
            var nombreBd = Guid.NewGuid().ToString();
            var factory = ConstruirWebAplicationFactory(nombreBd);
            var client = factory.CreateClient();

            //Act
            var respuesta = await client.GetAsync(_url);

            //Assert

            ((int)respuesta.StatusCode).Should().Be(StatusCodes.Status200OK);
            var generos = JsonConvert.DeserializeObject<List<GeneroGetDto>>(await respuesta.Content.ReadAsStringAsync());

            generos!.Count.Should().Be(0);


        }

        [Fact]
        public async Task ObtenerTodosLosGeneros_ShouldBe_ListadoDeGeneros()
        {
            //Arrange
            var nombreBd = Guid.NewGuid().ToString();
            var factory = ConstruirWebAplicationFactory(nombreBd);

            var contexto = ConstruirContext(nombreBd);
            contexto.Generos!.Add(new() { Nombre = "Genero 1" });
            contexto.Generos!.Add(new() { Nombre = "Genero 2" });

            await contexto.SaveChangesAsync();


            var client = factory.CreateClient();

            //Act
            var respuesta = await client.GetAsync(_url);

            //Assert

            respuesta.EnsureSuccessStatusCode();
            var generos = JsonConvert.DeserializeObject<List<GeneroGetDto>>(await respuesta.Content.ReadAsStringAsync());

            generos!.Count.Should().Be(2);


        }

        [Fact]
        public async Task BorrarGenero_ShouldBe_Ok()
        {
            //Arange
            var nombreBd = Guid.NewGuid().ToString();
            var factory = ConstruirWebAplicationFactory(nombreBd);

            var contexto = ConstruirContext(nombreBd);
            contexto.Generos!.Add(new() { Nombre = "Genero 1" });
            await contexto.SaveChangesAsync();

            var cliente = factory.CreateClient();

            //Act
            var respuesta = await cliente.DeleteAsync($"{_url}/1");

            //Assert
            respuesta.EnsureSuccessStatusCode();

            var contexto2 = ConstruirContext(nombreBd);
            var existe = await contexto2.Generos!.AnyAsync();

            existe.Should().BeFalse();

        }

        [Fact]
        public async Task BorrarGenero_ShouldBe_Unautorize()
        {
            //Arange
            var nombreBd = Guid.NewGuid().ToString();
            var factory = ConstruirWebAplicationFactory(nombreBd, ignorarSeguridad: false);

            var cliente = factory.CreateClient();

            //Act
            var respuesta = await cliente.DeleteAsync($"{_url}/1");

            //Assert
            respuesta.ReasonPhrase.Should().Be("Unauthorized");

        }




    }
}
