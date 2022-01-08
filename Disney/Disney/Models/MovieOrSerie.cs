using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Disney.Models
{
    public class MovieOrSerie
    {
        [Key]
        public long Id { get; set; }
        public byte[] Image { get; set; }
        public string Title { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? CreationDate { get; set; }
        public byte Qualification { get; set; }

        public ICollection<CharacterMovie> CharacterMovies { get; set; }
        public ICollection<Gerne> Gernes { get; set; }
    }
}
