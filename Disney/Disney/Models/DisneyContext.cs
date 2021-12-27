using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Disney.Models
{
    public class DisneyContext : IdentityDbContext
    {
        public DisneyContext(DbContextOptions<DisneyContext> options) : base(options)
        {

        }

        public DbSet<Character> Characters { get; set; }
        public DbSet<MovieOrSerie> MovieOrSeries { get; set; }
        public DbSet<CharacterMovie> CharacterMovies { get; set; }
        public DbSet<Gerne> Genres { get; set; }
    }
}
