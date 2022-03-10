using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Disney.Models
{
    public class MovieOrSerie
    {
        [Key]
        public long Id { get; set; }
        public string Image { get; set; }
        public string Title { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? CreationDate { get; set; }
        public byte Qualification { get; set; }

        public ICollection<CharacterMovie> CharacterMovies { get; set; }
        public ICollection<Genre> Gernes { get; set; }
    }
}
