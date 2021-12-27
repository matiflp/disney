using Disney.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Disney.Repositories.Interfaces
{
    public interface IMovieRepository
    {
        public IEnumerable<MovieOrSerie> GetMovies();
        public MovieOrSerie FindById(long id);
        public MovieOrSerie FindByName(string title);
        public long GetLastId();
        public void Save(MovieOrSerie movie);
        public void DeleteMovie(MovieOrSerie movie);
    }
}
