using Disney.Models;
using Disney.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Disney.Repositories
{
    public class MovieRepository : RepositoryBase<MovieOrSerie>, IMovieRepository
    {
        public MovieRepository(DisneyContext disneyContext) : base(disneyContext) { }

        public MovieOrSerie FindById(long id)
        {
            return FindByCondition(movie => movie.Id == id)
                .Include(movie => movie.Gernes)
                .Include(movie => movie.CharacterMovies)
                    .ThenInclude(charMovies => charMovies.Character)
                .FirstOrDefault();
        }

        public MovieOrSerie FindByName(string title)
        {
            return FindByCondition(movie => movie.Title.Equals(title)).FirstOrDefault();
        }

        public IEnumerable<MovieOrSerie> GetMovies()
        {
            return FindAll().OrderBy(movies => movies.Title);
        }

        public long GetLastId()
        {
            return FindAll().OrderByDescending(movie => movie.Id).FirstOrDefault() == null ? 0 : FindAll().OrderByDescending(movie => movie.Id).FirstOrDefault().Id;
        }

        public void Save(MovieOrSerie movie)
        {
            if (movie.Id == 0)
                Create(movie);
            else
                Update(movie);
            SaveChanges();
        }

        public void DeleteMovie(MovieOrSerie movie)
        {
            Delete(movie);
        }
    }
}
