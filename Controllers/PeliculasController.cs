using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;

using BackEndPeliculas.Models;

namespace BackEndPeliculas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PeliculasController : Controller
    {
        private readonly MoviesContext _peliculasRepository;
        private string _remoteUrl;
        private string _remotePort;
        private string _path;
        private string _method;
        private string _time;
        private readonly string _folder;
        public PeliculasController(MoviesContext peliculasRepository)
        {
            _peliculasRepository = peliculasRepository;
            _folder = Environment.CurrentDirectory + "\\Logs\\";
        }

        [HttpGet]
        public IActionResult List()
        {
            using StreamWriter log = CreateLogs();
            List<Movie> peliculas;
            try
            {
                peliculas = _peliculasRepository.Movie.ToList();
            }
            catch (Exception ex)
            {
                RegistrarErrores(log, ex);
                log.Dispose();
                return NotFound();
            }
            log.WriteLine("Películas encontradas: " + peliculas.Count());
            log.WriteLine("Solicitud completada con éxito");
            log.Dispose();
            return Ok(peliculas);
        }

        [HttpGet("{id}")]
        public IActionResult GetUno(int id)
        {
            using StreamWriter log = CreateLogs();
            Movie pelicula;
            try
            {
                pelicula = _peliculasRepository.Movie.FirstOrDefault(x => x.ID == id);
                if (pelicula == null)
                {
                    log.WriteLine("Película no encontrada");
                    log.Dispose();
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                RegistrarErrores(log, ex);
                log.Dispose();
                return NotFound();
            }
            log.WriteLine("Solicitud completada con éxito");
            log.Dispose();
            Response.Headers.Add("Content-Type", "charset=utf-16");
            return Ok(pelicula);
        }

        [HttpGet("title/{titulo}")]
        public IActionResult BuscarPorTitulo(string titulo)
        {
            StreamWriter log = CreateLogs();
            IQueryable<Movie> peliculas;
            try
            {
                peliculas = from m in _peliculasRepository.Movie select m;
                peliculas = peliculas.Where(x => x.Title.Contains(titulo));
                log.WriteLine("Películas encontradas: " + peliculas.Count());
            }
            catch (Exception ex)
            {
                RegistrarErrores(log, ex);
                log.Dispose();
                return NotFound();
            }
            log.WriteLine("Solicitud completada con éxito");
            log.Dispose();
            return Ok(peliculas.ToList());
        }

        [HttpGet("genre")]
        public IActionResult Generos()
        {
            StreamWriter log = CreateLogs();
            IQueryable<string> generos;
            List<string> genres;
            try
            {
                generos = from m in _peliculasRepository.Movie orderby m.Genre select m.Genre;
                genres = generos.Distinct().ToList();
                log.WriteLine("Géneros encontrados:");
                log.WriteLine(genres.Count());
            }
            catch (Exception ex)
            {
                RegistrarErrores(log, ex);
                log.Dispose();
                return NotFound();
            }
            log.WriteLine("Solicitud completada con éxito");
            log.Dispose();
            return Ok(genres);
        }

        [HttpGet("genero/{genero}")]
        public IActionResult BuscaRPorGenero(string genero)
        {
            StreamWriter log = CreateLogs();
            IQueryable<Movie> peliculas;
            try
            {
                peliculas = from m in _peliculasRepository.Movie select m;
                peliculas = peliculas.Where(m => m.Genre == genero);
                log.WriteLine("Películas con el género encontrado: " + peliculas.Count());
            }
            catch (Exception ex)
            {
                RegistrarErrores(log, ex);
                log.Dispose();
                return NotFound();
            }
            log.Dispose();
            return Ok(peliculas.ToList());
        }

        [HttpPost]
        public IActionResult Crear(Movie pelicula)
        {
            StreamWriter log = CreateLogs();
            try
            {
                _peliculasRepository.Movie.Add(pelicula);
                _peliculasRepository.SaveChanges();
                log.WriteLine("Película creada con ID: " + pelicula.ID);
                log.Dispose();
            }
            catch (Exception ex)
            {
                RegistrarErrores(log, ex);
                log.Dispose();
                return NotFound();
            }
            return CreatedAtRoute(new { id = pelicula.ID.ToString() }, pelicula);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Movie pelicula)
        {
            StreamWriter log = CreateLogs();
            Movie peliculaAux;
            try
            {
                peliculaAux = _peliculasRepository.Movie.FirstOrDefault(x => x.ID == id);
                if (peliculaAux == null)
                {
                    log.WriteLine("Película con el id {0} no encontrada", id);
                    return NotFound();
                }
                log.WriteLine("Antes del cambio: ");
                log.WriteLine("Title: " + pelicula.Title);
                log.WriteLine("Genre: " + pelicula.Genre);
                log.WriteLine("ReleaseDate: " + pelicula.ReleaseDate);
                log.WriteLine("Price: " + pelicula.Price);

                peliculaAux.Title = pelicula.Title;
                peliculaAux.Price = pelicula.Price;
                peliculaAux.Genre = pelicula.Genre;
                peliculaAux.ReleaseDate = pelicula.ReleaseDate;

                _peliculasRepository.Movie.Update(peliculaAux);
                _peliculasRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                RegistrarErrores(log, ex);
                log.Dispose();
                return NotFound();
            }
            log.Dispose();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            StreamWriter log = CreateLogs();
            Movie pelicula;
            try
            {
                pelicula = _peliculasRepository.Movie.FirstOrDefault(x => x.ID == id);
                if (pelicula == null)
                {
                    log.WriteLine("Película con el id {0} no encontrada", id);
                    log.Dispose();
                    return NotFound();
                }
                _peliculasRepository.Movie.Remove(pelicula);
                _peliculasRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                RegistrarErrores(log, ex);
                log.Dispose();
                return NotFound();
            }
            log.WriteLine("Película eliminada:");
            log.WriteLine("ID: " + pelicula.ID);
            log.WriteLine("Title: " + pelicula.Title);
            log.WriteLine("Genre: " + pelicula.Genre);
            log.WriteLine("ReleaseDate: " + pelicula.ReleaseDate);
            log.WriteLine("Price: " + pelicula.Price);
            log.Dispose();
            return NoContent();
        }

        private StreamWriter CreateLogs()
        {
            _remoteUrl = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            _remotePort = Request.HttpContext.Connection.RemotePort.ToString();
            _path = Request.Path;
            _method = Request.Method;
            _time = DateTime.Now.ToString("dd-MM-yyyy__HH_mm_ss");
            string rutalog = "log_" + _time + ".txt";
            string[] datos = { "Solicitud: " + _method, "Desde: " + _remoteUrl + ":" + _remotePort, "Digirida hacia: " + _path, "Realizada a las: " + _time, "Guardando logs en: " + _folder + rutalog };
            StreamWriter log = new StreamWriter(Path.Combine(_folder, rutalog));
            foreach (string line in datos)
            {
                log.WriteLine(line);
            }
            return log;
        }

        private void RegistrarErrores(StreamWriter log, Exception ex)
        {
            log.WriteLine("Ocurrio la excepción:");
            log.WriteLine(ex.Message);
            log.WriteLine("Más info:");
            log.WriteLine(ex.StackTrace);
            log.WriteLine("Emisor: " + ex.Source);
            log.WriteLine("Excepción interna:" + ex.InnerException);
            log.WriteLine("Error completo:");
            log.WriteLine(ex);
        }
    }
}
