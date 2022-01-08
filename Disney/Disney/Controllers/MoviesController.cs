using Disney.Models;
using Disney.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Disney.Repositories.Interfaces;
using Disney.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Disney.Controllers
{
    [Authorize]
    [Route("api/movies")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly ICharacterRepository _characterRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly ITokenService _tokenSevice;
        private readonly IConfiguration _configuration;

        public MoviesController(ICharacterRepository characterRepository, ITokenService tokenService, 
            IConfiguration configuration, IMovieRepository movieRepository)
        {
            _characterRepository = characterRepository;
            _tokenSevice = tokenService;
            _configuration = configuration;
            _movieRepository = movieRepository;
        }

        // GET: api/<MoviesController>
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                string token = HttpContext.Session.GetString("Token");
                if (string.IsNullOrEmpty(token))
                    return Forbid();

                if (_tokenSevice.IsTokenValid(_configuration["Jwt:Key"].ToString(), 
                    _configuration["Jwt:Issuer"].ToString(), token))
                {
                    var movies = _movieRepository.GetMovies();

                    var moviesView = movies.Select(movie => new MovieOrSerieDTO
                    {
                        Title = movie.Title,
                        Image = movie.Image,
                        CreationDate = movie.CreationDate
                    });

                    return Ok(moviesView);
                }

                return Forbid();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET api/<MoviesController>/5
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
                    var movie = _movieRepository.FindById(id);
                    if (movie != null)
                    {
                        MovieOrSerieDTO movieView = new()
                        {
                            Title = movie.Title,
                            Image = movie.Image,
                            CreationDate = movie.CreationDate,
                            Qualification = movie.Qualification,
                            Gernes = movie.Gernes.Select(gerne => new Gerne
                            {
                                Name = gerne.Name
                            }).ToList(),
                            CharacterMovies = movie.CharacterMovies?.Select(characterMovie => new CharacterMovieDTO
                            {
                                Character = new CharacterDTO
                                {
                                    Name = characterMovie.Character.Name
                                }
                            }).ToList()
                        };

                        return Ok(movieView);
                    }
                    else
                    {
                        return BadRequest("La película no existe");
                    }
                }

                return Forbid();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //GET api/<MoviesController>/
        [HttpGet("search")]
        public IActionResult Search(string title, string gerne, string order)
        {
            try
            {
                string token = HttpContext.Session.GetString("Token");
                if (string.IsNullOrEmpty(token))
                    return Forbid();

                if (_tokenSevice.IsTokenValid(_configuration["Jwt:Key"].ToString(), _configuration["Jwt:Issuer"], token))
                {
                    var movies = _movieRepository.GetMovies();
                    if (!string.IsNullOrEmpty(title))
                    {
                        movies = movies.Where(x => x.Title.Contains(title) ||
                                                   x.Gernes.Any(gernes => gernes.Name.Equals(gerne)))
                                                          .ToList();
                        if(movies != null)
                        {
                            if (order.ToLower().Equals("asc"))
                                movies = movies.OrderBy(moviess => moviess.CreationDate);
                            else if (order.ToLower().Equals("desc"))
                                movies = movies.OrderByDescending(moviess => moviess.CreationDate);
                        }
                    }
                    else
                        return NotFound("Película no encontrado");

                    return Ok(movies);
                }

                return Forbid();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST api/<MoviesController>
        [HttpPost]
        public IActionResult Post([FromBody] MovieOrSerieDTO movie)
        {
            try
            {
                string token = HttpContext.Session.GetString("Token");
                if (string.IsNullOrEmpty(token))
                    return Forbid();

                if (_tokenSevice.IsTokenValid(_configuration["Jwt:Key"], _configuration["Jwt:Issuer"], token))
                {

                    if (_movieRepository.FindByName(movie.Title) != null)
                    {
                        return StatusCode(403, "Esta pelicula ya se encuentra en la base de datos");
                    }

                    var characterId = _characterRepository.GetLastId();
                    var movieSerieId = _movieRepository.GetLastId() + 1;

                    var moviee = new MovieOrSerie
                    {
                        Title = movie.Title,
                        Image = movie.Image,
                        CreationDate = movie.CreationDate,
                        Qualification = movie.Qualification,
                        Gernes = movie.Gernes?.Select(gernes => new Gerne
                        {
                            Name = gernes.Name,
                            Image = gernes.Image
                        }).ToList(),
                        CharacterMovies = movie.CharacterMovies?.Select(characterMovie => new CharacterMovie
                        {
                            MovieSerieId = movieSerieId,
                            CharacterId = _characterRepository.FindByName(characterMovie.Character.Name) != null ? _characterRepository.FindByName(characterMovie.Character.Name).Id : characterId++,
                            Character = _characterRepository.FindByName(characterMovie.Character.Name) != null ? null : new Character
                            {
                                Name = characterMovie.Character.Name,
                                Age = characterMovie.Character.Age,
                                History = characterMovie.Character.History,
                                Image = characterMovie.Character.Image,
                                Weight = characterMovie.Character.Weight
                            }
                        }).ToList()
                    };

                    _movieRepository.Save(moviee);

                    return Ok("Película añadido correctamente");
                }

                return Forbid();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        // PUT api/<MoviesController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] MovieOrSerieDTO movie)
        {
            try
            {
                string token = HttpContext.Session.GetString("Token");
                if (string.IsNullOrEmpty(token))
                    return Forbid();

                if (_tokenSevice.IsTokenValid(_configuration["Jwt:Key"], _configuration["Jwt:Issuer"], token))
                {
                    var moviee = _movieRepository.FindById(id);

                    if (moviee != null)
                    {

                        var characterId = _characterRepository.GetLastId();

                        _movieRepository.Save(new MovieOrSerie
                        {
                            Title = movie.Title,
                            Image = movie.Image,
                            CreationDate = movie.CreationDate,
                            Qualification = movie.Qualification,
                            Gernes = movie.Gernes?.Select(gernes => new Gerne
                            {
                                Name = gernes.Name,
                                Image = gernes.Image
                            }).ToList(),
                            CharacterMovies = movie.CharacterMovies?.Select(characterMovie => new CharacterMovie
                            {
                                MovieSerieId = id,
                                CharacterId = _characterRepository.FindByName(characterMovie.Character.Name) != null ? _characterRepository.FindByName(characterMovie.Character.Name).Id : characterId + 1,
                                Character = _characterRepository.FindByName(characterMovie.Character.Name) != null ? null : new Character
                                {
                                    Name = characterMovie.Character.Name,
                                    Age = characterMovie.Character.Age,
                                    History = characterMovie.Character.History,
                                    Image = characterMovie.Character.Image,
                                    Weight = characterMovie.Character.Weight
                                }
                            }).ToList()
                        });

                        return Ok("Datos Modificados");
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

        // DELETE api/<MoviesController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            try
            {
                string token = HttpContext.Session.GetString("Token");
                if (string.IsNullOrEmpty(token))
                    return Forbid();

                if (_tokenSevice.IsTokenValid(_configuration["Jwt:Key"], _configuration["Jwt:Issuer"], token))
                {
                    var movie = _movieRepository.FindById(id);
                    if (movie != null)
                    {
                        _movieRepository.DeleteMovie(movie);

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
