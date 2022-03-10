namespace Disney.Models.DTOs
{
    public class CharacterMovieDTO
    {
        public long Id { get; set; }
        public long CharacterId { get; set; }
        public CharacterDTO Character { get; set; }

        public long MovieSerieId { get; set; }
        public MovieOrSerieDTO MovieSerie { get; set; }
    }
}
