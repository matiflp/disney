using Disney.Models;
using Disney.Models.DTOs;
using Disney.Repositories.Interfaces;
using Disney.Services;
using Disney.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Disney.Controllers
{
    //[Authorize]
    [Route("api/movies")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly ITokenService _tokenSevice;
        private readonly IConfiguration _configuration;
        private readonly ICharacterRepository _characterRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IFileService _fileService;

        public MoviesController(ICharacterRepository characterRepository, ITokenService tokenService,
            IConfiguration configuration, IMovieRepository movieRepository, IFileService fileService)
        {
            _characterRepository = characterRepository;
            _tokenSevice = tokenService;
            _configuration = configuration;
            _movieRepository = movieRepository;
            _fileService = fileService;
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
                            Gernes = movie.Gernes.Select(gerne => new GenreDTO
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
                        if (movies != null)
                        {
                            if (order.ToLower().Equals("asc"))
                                movies = movies.OrderBy(moviess => moviess.CreationDate);
                            else if (order.ToLower().Equals("desc"))
                                movies = movies.OrderByDescending(moviess => moviess.CreationDate);
                        }
                    }
                    else
                        return NotFound("Película no encontrada");

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
        public async Task<IActionResult> Post([FromBody] MovieOrSerieDTO movie)
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
                        return StatusCode(403, "Esta película ya se encuentra en la base de datos");
                    }

                    var movieSerieId = _movieRepository.GetLastId() + 1;

                    var moviee = new MovieOrSerie
                    {
                        Title = movie.Title,
                        Image = !string.IsNullOrEmpty(movie.Image) ? await _fileService.UploadEncodedImageAsync(movie.Image, movie.Title, ApplicationConstants.MoviesSeriesContainer) : "",
                        CreationDate = DateTime.Parse(movie.CreationDate.ToString().Substring(0, 8)),
                        Qualification = movie.Qualification,
                        Gernes = (await Task.WhenAll(movie.Gernes?.Select(async gernes => new Genre
                        {
                            Name = gernes.Name,
                            Image = !string.IsNullOrEmpty(gernes.Image) ? await _fileService.UploadEncodedImageAsync(gernes.Image, gernes.Name, ApplicationConstants.GenresContainer) : ""
                        }))).ToList(),
                        CharacterMovies = (await Task.WhenAll(movie.CharacterMovies?.Select(async characterMovie => new CharacterMovie
                        {
                            MovieSerieId = movieSerieId,
                            CharacterId = _characterRepository.FindByName(characterMovie.Character.Name) != null ? _characterRepository.FindByName(characterMovie.Character.Name).Id : 0,
                            Character = _characterRepository.FindByName(characterMovie.Character.Name) != null ? null : new Character
                            {
                                Name = characterMovie.Character.Name,
                                Age = characterMovie.Character.Age,
                                History = characterMovie.Character.History,
                                Image = !string.IsNullOrEmpty(characterMovie.Character.Image) ? await _fileService.UploadEncodedImageAsync(characterMovie.Character.Image, characterMovie.Character.Name, ApplicationConstants.CharactersContainer) : "",
                                Weight = characterMovie.Character.Weight
                            }
                        }))).ToList()
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
        public async Task<IActionResult> Put(int id, [FromBody] MovieOrSerieDTO movie)
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
                        moviee.Id = id;
                        moviee.Title = movie.Title;
                        moviee.Image = !string.IsNullOrEmpty(movie.Image) ? await _fileService.UploadEncodedImageAsync(movie.Image, movie.Title, ApplicationConstants.MoviesSeriesContainer) : "";
                        moviee.CreationDate = movie.CreationDate;
                        moviee.Qualification = movie.Qualification;
                        moviee.Gernes = (await Task.WhenAll(movie.Gernes?.Select(async gernes => new Genre
                        {
                            Id = gernes.Id,
                            Name = gernes.Name,
                            Image = !string.IsNullOrEmpty(gernes.Image) ? await _fileService.UploadEncodedImageAsync(gernes.Image, gernes.Name, ApplicationConstants.GenresContainer) : ""
                        }))).ToList();
                        moviee.CharacterMovies = (await Task.WhenAll(movie.CharacterMovies?.Select(async characterMovie => new CharacterMovie
                        {
                            Id = characterMovie.Id,
                            Character = _characterRepository.FindByName(characterMovie.Character.Name) != null ? _characterRepository.FindByName(characterMovie.Character.Name) : new Character
                            {
                                Name = characterMovie.Character.Name,
                                Age = characterMovie.Character.Age,
                                History = characterMovie.Character.History,
                                Image = !string.IsNullOrEmpty(characterMovie.Character.Image) ? await _fileService.UploadEncodedImageAsync(characterMovie.Character.Image, characterMovie.Character.Name, ApplicationConstants.CharactersContainer) : "",
                                Weight = characterMovie.Character.Weight
                            }
                        }))).ToList();

                        _movieRepository.Save(moviee);

                        return Ok("Datos Modificados");
                    }
                    else
                    {
                        return BadRequest("No se pudo actualizar. Película no encontrado.");
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

                        return Ok("Película eliminado");
                    }
                    else
                    {
                        return BadRequest("La Película a eliminar no existe");
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
