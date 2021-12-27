﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Disney.Models
{
    public class MovieOrSerie
    {
        public long Id { get; set; }
        public byte[] Image { get; set; }
        public string Title { get; set; }
        public DateTime? CreationDate { get; set; }
        public byte Qualification { get; set; }

        public ICollection<CharacterMovie> CharacterMovies { get; set; }
        public ICollection<Gerne> Gernes { get; set; }
    }
}
