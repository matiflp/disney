using Disney.Repositories;
using Disney.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Disney.Controllers
{
    [Route("api/characters")]
    [ApiController]
    //[Authorize]
    public class CharactersController : ControllerBase
    {
        private readonly ICharacterRepository _characterRepository;

        public CharactersController(ICharacterRepository characterRepository)
        {
            _characterRepository = characterRepository;
        }

        // GET: api/<CharactersController>
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var characters = _characterRepository.GetCharacters();

                var charactersView = characters.Select(character => new CharacterDTO
                {
                    Name = character.Name,
                    Image = character.Image
                });

                return Ok(charactersView);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET api/<CharactersController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var characterr = _characterRepository.FindById(id);

            CharacterDTO characterView = new()
            {
                Name = characterr.Name,
                Age = characterr.Age,
                Image = characterr.Image,
                History = characterr.History,
                Weight = characterr.Weight,
                MovieOrSeries = characterr.CharacterMovies.Select(movieSerie => new MovieOrSerieDTO
                {
                    Title = movieSerie.MovieSerie.Title,
                    Image = movieSerie.MovieSerie.Image
                }).ToList()
            };

            return Ok(characterView);
        }

        //GET api/<CharactersController>/name
        [HttpGet("{name}", Name = "Find")]
        public IActionResult FindName(string name, int age, float weight, List<long> idMovieSerie)
        {
            var characters = _characterRepository.GetCharacters();
            Character crt = new();
            if (!String.IsNullOrEmpty(name))
            {
                crt = characters.
                        Where(character => character.Name == name 
                            && character.Age == age
                            && character.Weight == weight
                            && character.CharacterMovies.Select(mos => mos.MovieSerie.Id)
                                .Any(id => idMovieSerie
                                    .Any(idmos => idmos == id)))
                        .FirstOrDefault();
            }

            return Ok(crt);
        }

        // POST api/<CharactersController>
        [HttpPost]
        public void Post([FromBody] CharacterDTO character)
        {
            var charter = new Character
            {
                Name = character.Name,
                Image = character.Image
            };

            _characterRepository.Save(charter);
        }

        // PUT api/<CharactersController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] CharacterDTO character)
        {
            var characterr = _characterRepository.FindById(id);

            characterr.Name = character.Name;
            characterr.Image = character.Image;

            _characterRepository.Save(characterr);

        }

        // DELETE api/<CharactersController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            var characterr = _characterRepository.FindById(id);

            _characterRepository.Delete(characterr);
        }
    }
}
