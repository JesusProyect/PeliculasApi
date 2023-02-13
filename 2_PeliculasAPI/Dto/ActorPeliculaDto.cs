namespace _2_PeliculasAPI.Dto
{
    public class ActorPeliculaBaseDto
    {
    }

    public class ActorPeliculaPostDto : ActorPeliculaBaseDto
    {
        public int ActorId { get; set; }
        public string? Personaje { get; set; }
    }
}
