using System.ComponentModel.DataAnnotations;

namespace Disney.Models
{
    public class CharacterMovie
    {
        [Key]
        public long Id { get; set; }

        public long CharacterId { get; set; }
        public Character Character { get; set; }

        public long MovieSerieId { get; set; }
        public MovieOrSerie MovieSerie { get; set; }

    }
}
