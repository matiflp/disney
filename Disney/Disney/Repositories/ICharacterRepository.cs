using Disney.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Disney.Repositories
{
    public interface ICharacterRepository : IRepositoryBase<Character>
    {
        public IEnumerable<Character> GetCharacters();
        public Character FindById(long id);
        public void Save(Character character);
    }
}
