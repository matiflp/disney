using Microsoft.EntityFrameworkCore;
using Disney.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Disney.Repositories.Interfaces;

namespace Disney.Repositories
{
    public class CharacterRepository : RepositoryBase<Character>, ICharacterRepository
    {
        public CharacterRepository(DisneyContext disneyContext) : base(disneyContext) { }

        public Character FindById(long id)
        {
            return FindByCondition(character => character.Id == id)
                .Include(character => character.CharacterMovies)
                    .ThenInclude(charMovies => charMovies.MovieSerie)
                .FirstOrDefault();
        }

        public Character FindByName(string name)
        {
            return FindByCondition(character => character.Name.Equals(name)).FirstOrDefault();
        }

        public long GetLastId()
        {
            return FindAll().OrderByDescending(character => character.Id).FirstOrDefault() == null ? 0 : FindAll().OrderByDescending(character => character.Id).FirstOrDefault().Id;
        }

        public IEnumerable<Character> GetCharacters()
        {
            return FindAll()
                .OrderBy(character => character.Name);
        }

        public void Save(Character character)
        {
            if (character.Id == 0)
                Create(character);
            else
                Update(character);
            SaveChanges();
        }

        public void DeleteCharacter(Character character)
        {
            Delete(character);
            SaveChanges();
        }
    }
}
