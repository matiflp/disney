using Disney.Models;
using Disney.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Disney.Repositories
{
    public class CharacterMovieRepository : RepositoryBase<CharacterMovie>, ICharacterMovieRepository
    {
        public CharacterMovieRepository(DisneyContext disneyContext): base(disneyContext) { }

        public CharacterMovie FindById(long id)
        {
            return FindByCondition(characterMovie => characterMovie.Id == id).FirstOrDefault();
        }

        public void Save(CharacterMovie characterMovie)
        {
            if (characterMovie.Id == 0)
                Create(characterMovie);
            else
                Update(characterMovie);
            SaveChanges();
        }
    }
}
