using System;
using System.Collections.Generic;

namespace Disney.Models.DTOs
{
    public class MovieOrSerieDTO
    {
        public string Image { get; set; }
        public string Title { get; set; }
        public DateTime? CreationDate { get; set; }
        public byte Qualification { get; set; }

        public ICollection<CharacterMovieDTO> CharacterMovies { get; set; }
        public ICollection<GenreDTO> Gernes { get; set; }
    }
}
