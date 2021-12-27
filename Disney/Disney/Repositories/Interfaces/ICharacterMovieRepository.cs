using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Disney.Models;

namespace Disney.Repositories.Interfaces
{
    public interface ICharacterMovieRepository
    {
        public CharacterMovie FindById(long id);
        public void Save(CharacterMovie characterMovie);
    }
}
