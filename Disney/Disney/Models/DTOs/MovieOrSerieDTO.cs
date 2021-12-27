using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Disney.Models.DTOs
{
    public class MovieOrSerieDTO
    {
        public byte[] Image { get; set; }
        public string Title { get; set; }
        public DateTime? CreationDate { get; set; }
        public byte Qualification { get; set; }

        public ICollection<CharacterMovieDTO> CharacterMovies { get; set; }
        public ICollection<Gerne> Gernes { get; set; }
    }
}
