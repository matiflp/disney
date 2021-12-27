using Disney.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Disney.Repositories.Interfaces
{
    public interface ICharacterRepository
    {
        public IEnumerable<Character> GetCharacters();
        public Character FindById(long id);
        public Character FindByName(string name);
        public long GetLastId();
        public void Save(Character character);
        public void DeleteCharacter(Character character);
    }
}
