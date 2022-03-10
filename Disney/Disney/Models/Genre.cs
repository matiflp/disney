using System.ComponentModel.DataAnnotations;

namespace Disney.Models
{
    public class Genre
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public long MovieOrSerieId { get; set; }
        public MovieOrSerie MovieOrSerie { get; set; }

    }
}
