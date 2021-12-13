using Microsoft.EntityFrameworkCore;
using Disney.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Disney.Repositories
{
    public class CharacterRepository : RepositoryBase<Character>, ICharacterRepository
    {
        public CharacterRepository(DisneyContext disneyContext) : base(disneyContext) { }

        public Character FindById(long id)
        {
            return FindByCondition(character => character.Id == id).FirstOrDefault();
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
    }
}
