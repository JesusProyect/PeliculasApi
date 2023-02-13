using _2_PeliculasAPI.Controllers;
using _2_PeliculasAPI.Dto;
using _2_PeliculasAPI.Entities;
using _2_PeliculasAPI.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeliculasApiTest.PruebasUnitarias
{
    public class ActoresControllerTest : BasePrueba
    {
        [Fact]
        public async Task ObtenerPersonasPaginadas()
        {
            //Arrange
            var nombreBd = Guid.NewGuid().ToString(); // creamos un nombre de bd unico para que cada prueba tenga su propia bd
            var contexto = ConstruirContext(nombreBd);
            var mapper = ConfigurarAutoMapper();

            contexto.Actores!.AddRange(new List<Actor>()
            {
                new(){ Nombre = "Actor1"},
                new(){ Nombre = "Actor2"},
                new(){ Nombre = "Actor3"}

            });

            await contexto.SaveChangesAsync();

            var contexto2 = ConstruirContext(nombreBd); //creamos otro para que si los cargamos en el mismo ya estan en memoria, no sabemos si estamos yendo a la base de datos o los cogemos de la memmoria

            //Act

            var controller = new ActorController(contexto2, mapper, null!); //el null esto es cuando vayamos a crear un archivo con foto, como no es este el caso pasamos null
            controller.ControllerContext.HttpContext = new DefaultHttpContext();  // nos daba nulo porque no estabamos usando una peticion http con esta linea lo solucionamos

            //Assert 
            var pagina1 = await controller.Get(new PaginacionDto() { Pagina = 1, CantidadRegistrosPorPagina = 2});
            pagina1.Value!.Count.Should().Be(2);


            controller.ControllerContext.HttpContext = new DefaultHttpContext();  // nos daba nulo porque no estabamos usando una peticion http con esta linea lo solucionamos
            var pagina2 = await controller.Get(new PaginacionDto() { Pagina = 2, CantidadRegistrosPorPagina = 2 }); // no se pueden agregar 2 cabeceras con el mismo nombre por eso me da error
            pagina2.Value!.Count.Should().Be(1);


            controller.ControllerContext.HttpContext = new DefaultHttpContext();  // nos daba nulo porque no estabamos usando una peticion http con esta linea lo solucionamos
            var pagina3 = await controller.Get(new PaginacionDto() { Pagina = 3, CantidadRegistrosPorPagina = 2 }); //solucion resetear el httpcontext para que no de el error de samekey value
            pagina3.Value!.Count.Should().Be(0);



        }

        [Fact]
        public async Task CrearActorSinImagen()
        {
            //Arrange
            var nombreBd = Guid.NewGuid().ToString(); // creamos un nombre de bd unico para que cada prueba tenga su propia bd
            var contexto = ConstruirContext(nombreBd);
            var mapper = ConfigurarAutoMapper();

            var actor = new ActorPostDto() { Nombre = "jesus", FechaNacimiento = DateTime.Now };
            var mock = new Mock<IAlmacenadorArchivos>();
            mock.Setup(x => x.GuardarArchivo(null!, null!, null!, null!)).Returns(Task.FromResult("url"));

            //Act
            var controller = new ActorController(contexto, mapper, mock.Object);
            var respuesta = await controller.Post(actor);

            //Assert

            respuesta.Should().BeAssignableTo<CreatedAtRouteResult>();

            var contexto2 = ConstruirContext(nombreBd);
            var actorBd = await contexto.Actores!.FirstOrDefaultAsync(x => x.Nombre == "jesus");

            actorBd.Should().NotBeNull();
            actorBd!.Foto.Should().BeNull();

            mock.Invocations.Count.Should().Be(0);
        }

        [Fact]
        public async Task CrearActorConFoto()
        {
            //Arrange
            var nombreBd = Guid.NewGuid().ToString(); // creamos un nombre de bd unico para que cada prueba tenga su propia bd
            var contexto = ConstruirContext(nombreBd);
            var mapper = ConfigurarAutoMapper();
            var content = Encoding.UTF8.GetBytes("Imagen de prueba");
            var archivo = new FormFile(new MemoryStream(content), 0, content.Length, "Data", "imagen.jpg");
            archivo.Headers = new HeaderDictionary();

            archivo.ContentType = "image/jpg";

            var actor = new ActorPostDto()
            {
                Nombre = "jesus",
                FechaNacimiento = DateTime.Now,
                Foto = archivo
            };

            var mock = new Mock<IAlmacenadorArchivos>();
            mock.Setup(x => x.GuardarArchivo(content, ".jpg", "actores", archivo.ContentType)).Returns(Task.FromResult("url"));
            mock.Setup(x => x.GuardarArchivo(It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult("url"));

            //Act
            var controller = new ActorController(contexto, mapper, mock.Object);
            var respuesta = await controller.Post(actor);

            //Assert
            respuesta.Should().BeAssignableTo<CreatedAtRouteResult>();

            var contexto2 = ConstruirContext(nombreBd);
            var actorBd = contexto2.Actores!.FirstOrDefault(x => x.Nombre == "jesus");
            actorBd.Should().NotBeNull();
            actorBd!.Foto.Should().Be("url");
            mock.Invocations.Count.Should().Be(1); //porque aqui devuelve 1 y en el anterior 0

             
        }

        [Fact]
        public async Task ActorPatch_ShouldBe_NotFound()
        {
            //Arrange
            var nombreBd = Guid.NewGuid().ToString(); // creamos un nombre de bd unico para que cada prueba tenga su propia bd
            var contexto = ConstruirContext(nombreBd);
            var mapper = ConfigurarAutoMapper();

            //Act

            var controller = new ActorController(contexto, mapper, null!);
            var patchDoc = new JsonPatchDocument<ActorPatchDto>();
            var respuesta = await controller.Patch(1, patchDoc);

            //Assert

            respuesta.Should().BeAssignableTo<NotFoundResult>();
        }

        [Fact]
        public async Task Patch_ShouldBe_Update1Field_andReturn_NoContent()
        {
            //Arrange
            var nombreBd = Guid.NewGuid().ToString(); // creamos un nombre de bd unico para que cada prueba tenga su propia bd
            var contexto = ConstruirContext(nombreBd);
            var mapper = ConfigurarAutoMapper();

            var fechaNacimiento = DateTime.Now;
            var actor = new Actor() { Nombre = "jesus", FechaNacimiento = fechaNacimiento };
            contexto.Add(actor);

            await contexto.SaveChangesAsync();

            var contexto2 = ConstruirContext(nombreBd);
            var mockValidator = new Mock<IObjectModelValidator>();
            mockValidator.Setup(x => x.Validate(It.IsAny<ActionContext>(), It.IsAny<ValidationStateDictionary>(), It.IsAny<string>(), It.IsAny<object>())); // no usamos return porque es un void
            
            var controller = new ActorController(contexto, mapper, null!);
            controller.ObjectValidator = mockValidator.Object;
            var patchDoc = new JsonPatchDocument<ActorPatchDto>();
            patchDoc.Operations.Add(new Operation<ActorPatchDto>("replace", "/nombre", null!, "lizbeth"));

            //Act
            var respuesta = await controller.Patch(1, patchDoc);

            //Assert
             respuesta.Should().BeAssignableTo<NoContentResult>();

            var contexto3 = ConstruirContext(nombreBd);
            var actorDb = await contexto3.Actores!.FirstOrDefaultAsync(x => x.Nombre == "lizbeth");
            actorDb.Should().NotBeNull();
            actorDb!.FechaNacimiento.Should().Be(fechaNacimiento);


        }
    }
}
