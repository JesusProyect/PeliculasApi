using _2_PeliculasAPI;
using _2_PeliculasAPI.Helpers.AutoMapperProfiles;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PeliculasApiTest
{
    public  class BasePrueba
    {
        protected string usuarioPorDefectoId = "9722b56a-77ea-4e41-941d-e319b6eb3712";
        protected string usuarioPorDefectoEmail = "text@gmail.com";

         protected static ApplicationDbContext ConstruirContext(string nombreDb)
         {
            var opciones = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(nombreDb).Options;

            var dbContext = new ApplicationDbContext(opciones);
            return dbContext;
         } 

        protected IMapper ConfigurarAutoMapper()
        {
            var config = new MapperConfiguration(options =>
            {
                var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                options.AddProfile(new AutoMapperProfiles(geometryFactory));
            });

            return config.CreateMapper();
        }

        protected ControllerContext ConstruirControllerContext()
        {
            var usuario = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, usuarioPorDefectoEmail),
                new Claim(ClaimTypes.Email, usuarioPorDefectoEmail),
                new Claim(ClaimTypes.NameIdentifier, usuarioPorDefectoId)

            }));

            return new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = usuario }
            };
        }

        protected WebApplicationFactory<Program> ConstruirWebAplicationFactory(string nombreBd, bool ignorarSeguridad = true)
        {
            var factory = new WebApplicationFactory<Program>();
            factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var descriptorContext = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                    if(descriptorContext != null) services.Remove(descriptorContext);

                    services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase(nombreBd));

                    if (ignorarSeguridad)
                    {
                        services.AddSingleton<IAuthorizationHandler, AllowAnonimusHandler>();
                        services.AddControllers(options => options.Filters.Add(new UsuarioFalsoFiltro()));
                    }

                });
            });

            return factory;
        }


    }
}
