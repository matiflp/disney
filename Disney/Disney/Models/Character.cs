using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Disney.Models
{
    public class Character
    {
        [Key]
        public long Id { get; set; }
        public byte[] Image { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public float Weight { get; set; }
        [DataType(DataType.MultilineText)]
        public string History { get; set; }

        public ICollection<CharacterMovie> CharacterMovies { get; set; }
    }
}
