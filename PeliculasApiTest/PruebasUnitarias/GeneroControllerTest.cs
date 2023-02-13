using _2_PeliculasAPI.Controllers;
using _2_PeliculasAPI.Dto;
using _2_PeliculasAPI.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeliculasApiTest.PruebasUnitarias
{
    public  class GeneroControllerTest : BasePrueba
    {
        [Fact]
        public async Task GetGeneroShoulBe_List_GeneroGetDto()
        {
            //Arrange
            var nombreBd = Guid.NewGuid().ToString(); // creamos un nombre de bd unico para que cada prueba tenga su propia bd
            var contexto = ConstruirContext(nombreBd);
            var mapper = ConfigurarAutoMapper();

            contexto.Generos!.AddRange(new List<Genero>()
            {
                new(){ Nombre = "Genero1"},
                new(){ Nombre = "Genero2"}
            });

            await contexto.SaveChangesAsync();

            var contexto2 = ConstruirContext(nombreBd); //creamos otro para que si los cargamos en el mismo ya estan en memoria, no sabemos si estamos yendo a la base de datos o los cogemos de la memmoria

            //Act

            var controller = new GeneroController(contexto2, mapper);
            var respuesta = await controller.Get();

            //Assert

            respuesta.Value.Should().NotBeNull();
            respuesta.Value!.Count.Should().Be(2);
        }
        [Fact]
        public async Task GetnGeneroById_ShouldBe_NotFound()
        {
            //Arrange
            var nombreBd = Guid.NewGuid().ToString(); 
            var contexto = ConstruirContext(nombreBd);
            var mapper = ConfigurarAutoMapper();

            //Act

            var controller = new GeneroController(contexto, mapper);
            var respuesta = await controller.Get(1);

            //Assert

            respuesta.Result.Should().BeAssignableTo<NotFoundResult>();
           
        }
        [Fact]
        public async Task ObtenerGeneroPorId_ShouldBe_GeneroGetDto()
        {
            //Arrange
            var nombreBd = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBd);
            var mapper = ConfigurarAutoMapper();

            contexto.Generos!.Add(new() { Nombre = "Genero1" });  // id es identity the first one always going to be 1
            await contexto.SaveChangesAsync();

           var contexto2 = ConstruirContext(nombreBd);
          
            //Act

            var controller = new GeneroController(contexto2, mapper);
            var respuesta = await controller.Get(1);  //id

            //Assert

            respuesta.Value!.Nombre.Should().Be("Genero1");
        }
        [Fact]
        public async Task CrearGenero_ShouldBe_CreatedAtRoute()
        {
            //Arrange
            var nombreBd = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBd);
            var mapper = ConfigurarAutoMapper();

            var nuevoGenero = new GeneroPostDto()
            {
                Nombre = "NuevoGenero"
            };
            await contexto.SaveChangesAsync();

            //Act
            var controller = new GeneroController(contexto, mapper);
            var respuesta = await controller.Post(nuevoGenero);  //id

            //Assert

            respuesta.Should().BeAssignableTo<CreatedAtRouteResult>();

            var contexto2 = ConstruirContext(nombreBd);
            var cantidad = await contexto2.Generos!.CountAsync();
            cantidad.Should().Be(1);

        }
        [Fact]
        public async Task ActualizarGenero_ShouldBe_NoContent()
        {
            //Arrange
            var nombreBd = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBd);
            var mapper = ConfigurarAutoMapper();

            contexto.Generos!.Add(new() { Nombre = "GeneroSinActualizar" });
            await contexto.SaveChangesAsync();

            var contexto2 = ConstruirContext(nombreBd);

            //Act
            var controller = new GeneroController(contexto2, mapper);
            var generoPostDto = new GeneroPostDto() { Nombre = "GeneroActualizado" };
            var respuesta = await controller.Put(1, generoPostDto);

            //Assert

            respuesta.Should().BeAssignableTo<NoContentResult>();

            var contexto3 = ConstruirContext(nombreBd);
            var genero = contexto3.Generos!.First(x => x.Id == 1);
            genero.Should().NotBeNull();
            genero.Nombre.Should().Be("GeneroActualizado");
        }
        [Fact]
        public async Task DeleteGenero_ShouldBe_NotFound()
        {
            //Arrange
            var nombreBd = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBd);
            var mapper = ConfigurarAutoMapper();

            //Act

            var controller = new GeneroController(contexto, mapper);
            var resultado = await controller.Delete(1); // id

            //Assert

            resultado.Should().BeAssignableTo<NotFoundResult>();
        }
        [Fact]
        public async Task DeleteGenero_ShouldBe_NoContent()
        {
            //Arrange
            var nombreBd = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBd);
            var mapper = ConfigurarAutoMapper();

            contexto.Generos!.Add(new() { Nombre = "Genero1" });
            await contexto.SaveChangesAsync();

            var contexto2 = ConstruirContext(nombreBd);

            //Act
            var controller = new GeneroController(contexto2, mapper);
            var resultado = await controller.Delete(1); // id

            //Assert

            resultado.Should().BeAssignableTo<NoContentResult>();
            var contexto3 = ConstruirContext(nombreBd);
            var existe = await contexto3.Generos!.AnyAsync(x => x.Id == 1);

            existe.Should().BeFalse();

        }

    }
}
