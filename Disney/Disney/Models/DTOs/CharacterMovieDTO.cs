using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Disney.Models.DTOs
{
    public class CharacterMovieDTO
    {
        public long CharacterId { get; set; }
        public CharacterDTO Character { get; set; }

        public long MovieSerieId { get; set; }
        public MovieOrSerieDTO MovieSerie { get; set; }
    }
}
