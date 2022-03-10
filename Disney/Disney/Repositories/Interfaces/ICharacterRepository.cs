using Disney.Models;
using System.Collections.Generic;

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
