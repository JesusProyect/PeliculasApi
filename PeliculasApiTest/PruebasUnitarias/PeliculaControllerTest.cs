using _2_PeliculasAPI.Controllers;
using _2_PeliculasAPI.Dto;
using _2_PeliculasAPI.Entities;
using AutoMapper;
using Castle.Core.Logging;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NetTopologySuite.Algorithm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeliculasApiTest.PruebasUnitarias
{
    public class PeliculaControllerTest : BasePrueba
    {

        [Fact]
        public async Task ObtenerPeliculas_FiltroTitulo_ShouldBeOk()
        {
            //Arrange
            var nombreDb = _CrearDataPrueba();
            var mapper = ConfigurarAutoMapper();
            var contexto = ConstruirContext(nombreDb);

            var controller = new PeliculaController(contexto, mapper, null!, null!); // IalmacenadorArchivo no vamos a usar porque ya lo probamos en Actores, Ilogger luego
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var tituloPelicula = "Pelicula 1";
            var filtroDto = new FiltroPeliculaDto()
            {
                Titulo = tituloPelicula,
                CantidadRegistrosPorPagina = 10
            };

            //Act

            var result = await controller.Filtrar(filtroDto);

            //Assert

            result.Value.Should().BeAssignableTo<List<PeliculaGetDto>>();
            result.Value!.Count.Should().Be(1);
            result.Value![0].Titulo.Should().Be("Pelicula 1");

        }


        [Fact]
        public async Task ObtenerPeliculas_FiltroEnCines_ShouldBeOk()
        {
            //Arrange
            var nombreDb = _CrearDataPrueba();
            var mapper = ConfigurarAutoMapper();
            var contexto = ConstruirContext(nombreDb);

            var controller = new PeliculaController(contexto, mapper, null!, null!); // IalmacenadorArchivo no vamos a usar porque ya lo probamos en Actores, Ilogger luego
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var filtroDto = new FiltroPeliculaDto() { EnCines = true };

            //Act
            var result = await controller.Filtrar(filtroDto);

            //Assert
            result.Value.Should().BeAssignableTo<List<PeliculaGetDto>>();
            result.Value!.Count.Should().Be(1);
            result.Value![0].Titulo.Should().Be("Pelicula En Cines");

        }

        [Fact]
        public async Task ObtenerPeliculas_FiltroProximosEstrenos_ShouldBeOk()
        {
            //Arrange
            var nombreDb = _CrearDataPrueba();
            var mapper = ConfigurarAutoMapper();
            var contexto = ConstruirContext(nombreDb);

            var controller = new PeliculaController(contexto, mapper, null!, null!); // IalmacenadorArchivo no vamos a usar porque ya lo probamos en Actores, Ilogger luego
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var filtroDto = new FiltroPeliculaDto() { ProximosExtrenos = true };

            //Act
            var result = await controller.Filtrar(filtroDto);

            //Assert
            result.Value.Should().BeAssignableTo<List<PeliculaGetDto>>();
            result.Value!.Count.Should().Be(1);
            result.Value![0].Titulo.Should().Be("No Estrenada");

        }

        [Fact]
        public async Task ObtenerPeliculas_FiltroGenero_ShouldBeOk()
        {
            //Arrange
            var nombreDb = _CrearDataPrueba();
            var mapper = ConfigurarAutoMapper();
            var contexto = ConstruirContext(nombreDb);

            var controller = new PeliculaController(contexto, mapper, null!, null!); // IalmacenadorArchivo no vamos a usar porque ya lo probamos en Actores, Ilogger luego
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var generoId = contexto.Generos!.Select(x => x.Id).First();

            var filtroDto = new FiltroPeliculaDto() { GeneroId = generoId };

            //Act

            var result = await controller.Filtrar(filtroDto);

            //Assert
            result.Value.Should().BeAssignableTo<List<PeliculaGetDto>>();
            result.Value!.Count.Should().Be(1);
            result.Value![0].Titulo.Should().Be("Pelicula Con Genero");

        }

        [Fact]
        public async Task ObtenerPeliculas_FiltroTitulo_Ordenar_Ascendente_ShouldBeOk()
        {
            //Arrange
            var nombreDb = _CrearDataPrueba();
            var mapper = ConfigurarAutoMapper();
            var contexto = ConstruirContext(nombreDb);

            var controller = new PeliculaController(contexto, mapper, null!, null!); // IalmacenadorArchivo no vamos a usar porque ya lo probamos en Actores, Ilogger luego
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var filtroDto = new FiltroPeliculaDto() { CampoOrdenar = "titulo", OrdenAscendente = true };

            //Act
            var result = await controller.Filtrar(filtroDto);

            //Assert
            var contexto2 = ConstruirContext(nombreDb);
            var peliculasDb = contexto2.Peliculas!.OrderBy(x => x.Titulo).ToList();

            result.Value!.Count.Should().Be(peliculasDb.Count);
            int contador = 0;
            result.Value!.ForEach((pelicula) =>
            {
                pelicula.Titulo.Should().Be(peliculasDb[contador].Titulo);
                contador++;
            });

        }

        [Fact]
        public async Task ObtenerPeliculas_FiltroTitulo_Ordenar_Descendente_ShouldBeOk()
        {
            //Arrange
            var nombreDb = _CrearDataPrueba();
            var mapper = ConfigurarAutoMapper();
            var contexto = ConstruirContext(nombreDb);

            var controller = new PeliculaController(contexto, mapper, null!, null!); // IalmacenadorArchivo no vamos a usar porque ya lo probamos en Actores, Ilogger luego
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var filtroDto = new FiltroPeliculaDto() { CampoOrdenar = "titulo", OrdenAscendente = false };

            //Act
            var result = await controller.Filtrar(filtroDto);

            //Assert
            var contexto2 = ConstruirContext(nombreDb);
            var peliculasDb = contexto2.Peliculas!.OrderByDescending(x => x.Titulo).ToList();

            result.Value!.Count.Should().Be(peliculasDb.Count);
            int contador = 0;
            result.Value!.ForEach((pelicula) =>
            {
                pelicula.Titulo.Should().Be(peliculasDb[contador].Titulo);
                contador++;
            });

        }

        [Fact]
        public async Task ObtenerPelicula_FiltroCampoNoExiste_ShouldBe_BadRequest()
        {
            //Arrange
            var contexto = ConstruirContext(Guid.NewGuid().ToString());
            var mapper = ConfigurarAutoMapper();
            var loggerMock = new Mock<ILogger<PeliculaController>>();

            var controller = new PeliculaController(contexto, mapper, null!, loggerMock.Object); // IalmacenadorArchivo no vamos a usar porque ya lo probamos en Actores, Ilogger luego
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var filtroDto = new FiltroPeliculaDto() { CampoOrdenar = "NoExiste", OrdenAscendente = false };
            
            //Act
            var result = await controller.Filtrar(filtroDto);
            var badRequest = result.Result as BadRequestObjectResult;  // asi si no no puedo acceder al value 
            
            //Assert
            result.Result.Should().BeAssignableTo<BadRequestObjectResult>();
            badRequest!.Value.Should().Be("El Campo por el que se quiere ordenar no existe");
            loggerMock.Invocations.Count.Should().Be(1);

        }

        //metodo auxiliar que nos mete datos en la bd y nos devuelve el nombre de la bd
        private string _CrearDataPrueba()
       {
            var nameDb = Guid.NewGuid().ToString();
            var context = ConstruirContext(nameDb);
            var genero = new Genero() { Nombre = "Genero1" };

            var peliculas = new List<Pelicula>()
            {
                new(){ Titulo = "Pelicula 1", FechaEstreno = new DateTime(2010,1,1), EnCines = false },
                new(){ Titulo = "No Estrenada", FechaEstreno = DateTime.Today.AddDays(10), EnCines = false },
                new(){ Titulo = "Pelicula En Cines", FechaEstreno = DateTime.Today.AddDays(-1), EnCines = true }

            };

            var peliculaConGenero = new Pelicula()
            {
              Titulo = "Pelicula Con Genero",
              FechaEstreno = new DateTime(2010,1,1),
              EnCines = false
            };

            peliculas.Add(peliculaConGenero);
            context.Add(genero);
            context.AddRange(peliculas);
            context.SaveChanges();

            var peliculaGenero = new PeliculaGenero() { GeneroId = genero.Id, PeliculaId = peliculaConGenero.Id };
            context.Add(peliculaGenero);
            context.SaveChanges(true);

            return nameDb;
       }



    }
}
