using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Disney.Models
{
    public class MovieOrSerie
    {
        public long Id { get; set; }
        public string Image { get; set; }
        public string Title { get; set; }
        public DateTime? CreationDate { get; set; }
        public byte Qualification { get; set; }
        public long CharacterId { get; set; }
        public Character Character { get; set; }
        public ICollection<Gerne> Gernes { get; set; }
    }
}
