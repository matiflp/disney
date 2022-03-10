namespace Disney.Models.DTOs
{
    public class GenreDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }

        public MovieOrSerieDTO MovieOrSerie { get; set; }
    }
}
