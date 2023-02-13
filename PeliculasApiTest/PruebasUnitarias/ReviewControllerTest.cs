using _2_PeliculasAPI.Controllers;
using _2_PeliculasAPI.Dto;
using _2_PeliculasAPI.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeliculasApiTest.PruebasUnitarias
{
    public class ReviewControllerTest : BasePrueba
    {

        [Fact]
        public async Task UsuarioNoPuedeCrear_2Reviews_ParaLaMismaPelicula()
        {
            //Arrange
            var nombreBd = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBd);//este
            CrearPeliculas(nombreBd);//es para que sean contextos distintos

            var peliculaId = contexto.Peliculas!.Select(x => x.Id).First();

            var review1 = new Review()
            {
               PeliculaId = peliculaId,
               UsuarioId = usuarioPorDefectoId,
               Puntuacion = 5
            };
            contexto.Add(review1);
            await contexto.SaveChangesAsync();

            var contexto2 = ConstruirContext(nombreBd);
            var mapper = ConfigurarAutoMapper();
            var controller = new ReviewController(contexto2, mapper)
            {
                ControllerContext = ConstruirControllerContext()
            };

            var reviewDto = new ReviewPostDto()
            {
              Puntuacion = 5
            };

            //Act
            var respuesta = await controller.Post(peliculaId, reviewDto);
            var badRequest = respuesta as BadRequestObjectResult;

            //Assert
            respuesta.Should().BeAssignableTo<BadRequestObjectResult>();
            badRequest!.Value.Should().Be("El usuario ya ha escrito un review de la pelicula");

        }

        [Fact]
        public async Task CrearReview_ShouldBe_Ok()
        {
            //Arrange
            var nombreBd = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBd);//este
            CrearPeliculas(nombreBd);//es para que sean contextos distintos

            var peliculaId = contexto.Peliculas!.Select(x => x.Id).First();
            var contexto2 = ConstruirContext(nombreBd);
            var mapper = ConfigurarAutoMapper();
            var controller = new ReviewController(contexto2, mapper)
            {
                ControllerContext = ConstruirControllerContext()
            };
            var reviewDto = new ReviewPostDto() { Puntuacion = 5, Comentario = "test" };

            //Act
            var respuesta = await controller.Post(peliculaId, reviewDto);
            var OkResult = respuesta as NoContentResult;

            //Assert
            OkResult.Should().BeAssignableTo<NoContentResult>();

            var contexto3 = ConstruirContext(nombreBd);
            var reviewDb = contexto3.Reviews!.First();

            reviewDb.UsuarioId.Should().Be(usuarioPorDefectoId);

        }


        //metodo auxiliar pues los comentarios necesitan de una pelicula para existir
        private static void CrearPeliculas(string nombreBd)
        {
            var contexto = ConstruirContext(nombreBd);//de este

            contexto.Peliculas!.Add(new() { Titulo = "pelicula 1" });
            contexto.SaveChanges();

        }

    }
}
