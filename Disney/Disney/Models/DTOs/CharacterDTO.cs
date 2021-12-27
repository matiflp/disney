using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Disney.Models.DTOs
{
    public class CharacterDTO
    {
        public byte[] Image { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public float Weight { get; set; }
        public string History { get; set; }
        public ICollection<CharacterMovieDTO> CharacterMovies { get; set; }
    }
}
