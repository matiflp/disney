using Disney.Repositories.Interfaces;
using Disney.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Disney.Services;
using Microsoft.Extensions.Configuration;
using Disney.Models.DTOs;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Disney.Controllers
{
    [Authorize]
    [Route("api/characters")]
    [ApiController]
    public class CharactersController : ControllerBase
    {
        private readonly ICharacterRepository _characterRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly ITokenService _tokenSevice;
        private readonly IConfiguration _configuration;

        public CharactersController(ICharacterRepository characterRepository, ITokenService tokenService, IConfiguration configuration, IMovieRepository movieRepository)
        {
            _characterRepository = characterRepository;
            _tokenSevice = tokenService;
            _configuration = configuration;
            _movieRepository = movieRepository;
        }

        // GET: api/<CharactersController>
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                string token = HttpContext.Session.GetString("Token");
                if (string.IsNullOrEmpty(token))
                    return Forbid();

                if (_tokenSevice.IsTokenValid(_configuration["Jwt:Key"].ToString(), _configuration["Jwt:Issuer"].ToString(), token))
                {
                    var characters = _characterRepository.GetCharacters();

                    var charactersView = characters.Select(character => new CharacterDTO
                    {
                        Name = character.Name,
                        Image = character.Image,
                    });

                    return Ok(charactersView);
                }

                return Forbid();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET api/<CharactersController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                string token = HttpContext.Session.GetString("Token");
                if (string.IsNullOrEmpty(token))
                    return Forbid();

                if (_tokenSevice.IsTokenValid(_configuration["JWt:Key"].ToString(), _configuration["Jwt:Issuer"].ToString(), token))
                {
                    var characterr = _characterRepository.FindById(id);
                    if (characterr != null)
                    {
                        CharacterDTO characterView = new()
                        {
                            Name = characterr.Name,
                            Age = characterr.Age,
                            Image = characterr.Image,
                            History = characterr.History,
                            Weight = characterr.Weight,
                            CharacterMovies = characterr.CharacterMovies?.Select(characterMovie => new CharacterMovieDTO
                            {
                                MovieSerie = new MovieOrSerieDTO
                                {
                                    Title = characterMovie.MovieSerie.Title,
                                    Image = characterMovie.MovieSerie.Image
                                }
                            }).ToList()
                        };

                        return Ok(characterView);
                    }
                    else
                    {
                        return BadRequest("El personaje no existe");
                    }
                }

                return Forbid();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //GET api/<CharactersController>/
        [HttpGet("search")]
        public IActionResult Search(string name, int age = 0, double weight = 0.0, long idMovie = 0)
        {
            try
            {
                string token = HttpContext.Session.GetString("Token");
                if (string.IsNullOrEmpty(token))
                    return Forbid();

                if (_tokenSevice.IsTokenValid(_configuration["Jwt:Key"].ToString(), _configuration["Jwt:Issuer"], token))
                {
                    var characters = _characterRepository.GetCharacters();
                    if (!string.IsNullOrEmpty(name))
                    {
                        characters = characters.Where(x => x.Name.Contains(name) ||
                                                          x.Age.ToString().Contains(age.ToString()) ||
                                                          x.Weight.ToString().Contains(weight.ToString()) ||
                                                          x.CharacterMovies.Any(charMovie => charMovie.MovieSerie.Id.ToString().Equals(idMovie.ToString())))
                                                          .ToList();
                    } else
                        return NotFound("Personaje No encontrado");

                    return Ok(characters);
                }

                return Forbid();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        
        // POST api/<CharactersController>
        [HttpPost]
        public IActionResult Post([FromBody] CharacterDTO character)
        {
            try
            {
                string token = HttpContext.Session.GetString("Token");
                if (string.IsNullOrEmpty(token))
                    return Forbid();

                if (_tokenSevice.IsTokenValid(_configuration["Jwt:Key"], _configuration["Jwt:Issuer"], token))
                {

                    if (_characterRepository.FindByName(character.Name) != null)
                    {
                        return StatusCode(403, "Este personaje ya se encuentra en la base de datos");
                    }

                    var characterId = _characterRepository.GetLastId()+1;
                    var movieSerieId = _movieRepository.GetLastId();

                    var charter = new Character
                    {
                        Image = character.Image,
                        Name = character.Name,
                        Age = character.Age,
                        Weight = character.Weight,
                        History = character.History,
                        CharacterMovies = character.CharacterMovies?.Select(charMovie => new CharacterMovie
                        {
                            CharacterId =  characterId,
                            MovieSerieId = _movieRepository.FindByName(charMovie.MovieSerie.Title) != null ? _movieRepository.FindByName(charMovie.MovieSerie.Title).Id : movieSerieId++,
                            MovieSerie = _movieRepository.FindByName(charMovie.MovieSerie.Title) != null ? null : new MovieOrSerie
                            {
                                CreationDate = charMovie.MovieSerie.CreationDate,
                                Image = charMovie.MovieSerie.Image,
                                Title = charMovie.MovieSerie.Title,
                                Qualification = charMovie.MovieSerie.Qualification,
                                Gernes = charMovie.MovieSerie.Gernes?.Select(gerne => new Gerne
                                {
                                    Name = gerne.Name,
                                    Image = gerne.Image,
                                    MovieOrSerieId = movieSerieId
                                }).ToList()
                            }
                        }).ToList()
                    };

                    _characterRepository.Save(charter);

                    return Ok("Personaje añadido correctamente");
                }

                return Forbid("Usuario no autorizado");
                    
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            
        }

        // PUT api/<CharactersController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] CharacterDTO character)
        {
            try
            {
                string token = HttpContext.Session.GetString("Token");
                if (string.IsNullOrEmpty(token))
                    return Forbid();

                if (_tokenSevice.IsTokenValid(_configuration["Jwt:Key"], _configuration["Jwt:Issuer"], token))
                {
                    var characterr = _characterRepository.FindById(id);

                    if (characterr != null)
                    {
                        var movieSerieId = _movieRepository.GetLastId();
                        _characterRepository.Save(new Character
                        {
                            Id = id,
                            Name = character.Name,
                            Age = character.Age,
                            History = character.History,
                            Image = character.Image,
                            Weight = character.Weight,
                            CharacterMovies = character.CharacterMovies?.Select(charMovie => new CharacterMovie
                            {
                                CharacterId = id,
                                MovieSerieId = _movieRepository.FindByName(charMovie.MovieSerie.Title) != null ? _movieRepository.FindByName(charMovie.MovieSerie.Title).Id : movieSerieId++,
                                MovieSerie = _movieRepository.FindByName(charMovie.MovieSerie.Title) != null ? null : new MovieOrSerie
                                {
                                    CreationDate = charMovie.MovieSerie.CreationDate,
                                    Image = charMovie.MovieSerie.Image,
                                    Title = charMovie.MovieSerie.Title,
                                    Qualification = charMovie.MovieSerie.Qualification,
                                    Gernes = charMovie.MovieSerie.Gernes?.Select(gerne => new Gerne
                                    {
                                        Name = gerne.Name,
                                        Image = gerne.Image,
                                        MovieOrSerieId = movieSerieId
                                    }).ToList()
                                }
                            }).ToList()
                        });

                        return Ok("Personaje modificado");
                    }
                    else
                    {
                        return BadRequest("No se pudo actualizar. Personaje no encontrado.");
                    }
                }

                return Forbid();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE api/<CharactersController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                string token = HttpContext.Session.GetString("Token");
                if (string.IsNullOrEmpty(token))
                    return Forbid();

                if (_tokenSevice.IsTokenValid(_configuration["Jwt:Key"], _configuration["Jwt:Issuer"], token))
                {
                    var characterr = _characterRepository.FindById(id);
                    if (characterr != null)
                    {
                        _characterRepository.DeleteCharacter(characterr);

                        return Ok("Personaje eliminado");
                    }
                    else
                    {
                        return BadRequest("El personaje a eliminar no existe");
                    }
                }

                return Forbid();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            
        }
    }
}
