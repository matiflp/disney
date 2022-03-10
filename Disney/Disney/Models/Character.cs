using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Disney.Models
{
    public class Character
    {
        [Key]
        public long Id { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public float Weight { get; set; }
        [DataType(DataType.MultilineText)]
        public string History { get; set; }

        public ICollection<CharacterMovie> CharacterMovies { get; set; }
    }
}
