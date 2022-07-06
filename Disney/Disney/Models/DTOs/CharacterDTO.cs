using System.Collections.Generic;

namespace Disney.Models.DTOs
{
    public class CharacterDTO
    {
        public long Id { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public float Weight { get; set; }
        public string History { get; set; }
        public ICollection<CharacterMovieDTO> CharacterMovies { get; set; }
    }
}
