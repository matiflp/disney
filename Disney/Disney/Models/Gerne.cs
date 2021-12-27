using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Disney.Models
{
    public class Gerne
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public byte[] Image { get; set; }
        public long MovieOrSerieId { get; set; }
        public MovieOrSerie MovieOrSerie { get; set; }

    }
}
