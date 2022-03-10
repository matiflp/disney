using Disney.Models;

namespace Disney.Repositories.Interfaces
{
    public interface ICharacterMovieRepository
    {
        public CharacterMovie FindById(long id);
        public bool GetCharacterId(long characterId);
        public bool GetMovieSerieId(long movieSerieId);
        public void Save(CharacterMovie characterMovie);
    }
}
