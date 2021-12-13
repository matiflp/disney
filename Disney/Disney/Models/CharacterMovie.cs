using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Disney.Models
{
    public class CharacterMovie
    {
        public long Id { get; set; }

        public long CharacterId { get; set; }
        public Character Character { get; set; }

        public long MovieSerieId { get; set; }
        public MovieOrSerie MovieSerie { get; set; }

    }
}
