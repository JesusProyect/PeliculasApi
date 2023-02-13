using _2_PeliculasAPI.Dto;
using _2_PeliculasAPI.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace _2_PeliculasAPI.Helpers.AutoMapperProfiles
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles(GeometryFactory geometryFactory)
        {
            //Genero
            CreateMap<Genero, GeneroGetDto>().ReverseMap();
            CreateMap<GeneroPostDto, Genero>();

            //Actor
            CreateMap<Actor, ActorGetDto>().ReverseMap();
            CreateMap<ActorPostDto, Actor>()
                .ForMember(dest => dest.Foto, opt => opt.Ignore());
            CreateMap<ActorPatchDto, Actor>().ReverseMap();

            //Pelicula
            CreateMap<Pelicula, PeliculaGetDto>().ReverseMap();
            CreateMap<PeliculaPostDto, Pelicula>()
                .ForMember(dest => dest.Poster, opt => opt.Ignore())
                .ForMember(dest => dest.PeliculasGeneros, opt => opt.MapFrom(MapPeliculasGeneros))
                .ForMember(dest => dest.PeliculasActores, opt => opt.MapFrom(MapPeliculasActores));
            CreateMap<PeliculaPatchDto, Pelicula>().ReverseMap();
            CreateMap<Pelicula, PeliculaDetalleDto>()
                .ForMember(dest => dest.Generos, opt => opt.MapFrom(MapPeliculasGeneros))
                .ForMember(dest => dest.Actores, opt => opt.MapFrom(MapPeliculasActores));

            //SalaDeCine
            CreateMap<SalaDeCine, SalaDeCineGetDto>()
               .ForMember(dest => dest.Latitud, opt => opt.MapFrom(y => y.Ubicacion!.Y)) // coordenada Y
               .ForMember(dest => dest.Longitud, opt => opt.MapFrom(x => x.Ubicacion!.X));

            CreateMap<SalaDeCineGetDto, SalaDeCine> ()
                .ForMember(dest => dest.Ubicacion, opt => opt.MapFrom( y => geometryFactory.CreatePoint(new Coordinate(y.Longitud, y.Latitud))));

            CreateMap<SalaDeCinePostDto, SalaDeCine>()
                .ForMember(dest => dest.Ubicacion, opt => opt.MapFrom(y => geometryFactory.CreatePoint(new Coordinate(y.Longitud, y.Latitud))));

            //Usuario
            CreateMap<IdentityUser, UsuarioGetDto>();

            //Reviews
            CreateMap<Review, ReviewGetDto>()
                .ForMember(dest => dest.NombreUsuario, opt => opt.MapFrom( r => r.Usuario!.UserName));

            CreateMap<ReviewGetDto, Review>();
            CreateMap<ReviewPostDto, Review>();



        }

        private List<PeliculaGenero> MapPeliculasGeneros(PeliculaPostDto peliculaPostDto, Pelicula pelicula)
        {
            List<PeliculaGenero> resultado = new();
            if (peliculaPostDto.GenerosIds == null || peliculaPostDto.GenerosIds.Count == 0) return resultado;

            peliculaPostDto.GenerosIds.ForEach(generoId => resultado.Add(new() { GeneroId = generoId }));

            return resultado;
        }
        private List<ActorPeliculaDetalleDto> MapPeliculasActores(Pelicula pelicula, PeliculaDetalleDto peliculaDetalleDto)
        {
            List<ActorPeliculaDetalleDto> resultado = new();
            if (pelicula.PeliculasActores is null) return resultado;

            pelicula.PeliculasActores.ForEach(actorPelicula => resultado.Add(new()
            {
                 ActorId = actorPelicula.ActorId,
                 Personaje = actorPelicula.Personaje,
                 NombrePersona = actorPelicula.Actor!.Nombre
            }));

            return resultado;
        }
        private List<PeliculaActor> MapPeliculasActores(PeliculaPostDto peliculaPostDto, Pelicula pelicula)
        {
            List<PeliculaActor> resultado = new();
            if (peliculaPostDto.Actores == null || peliculaPostDto.Actores.Count == 0) return resultado;

            int orden = 0;
            peliculaPostDto.Actores.ForEach(actor => resultado.Add(new() {
                ActorId = actor.ActorId,
                Personaje = actor.Personaje,
                Orden = ++orden
            }));

            return resultado;
        }
        private List<GeneroGetDto> MapPeliculasGeneros (Pelicula pelicula, PeliculaDetalleDto PeliculaDetalleDto)
        {
            List<GeneroGetDto> resultado = new();
            if (pelicula.PeliculasGeneros is null) return resultado;

            pelicula.PeliculasGeneros.ForEach(generoPelicula => resultado.Add(new()
            {
                Id = generoPelicula.GeneroId,
                Nombre = generoPelicula.Genero!.Nombre
            }));

            return resultado;
        }

    }
}
