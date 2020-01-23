using System;
using System.Collections.Generic;
using System.Linq;
using BackEndPeliculas.Interfaces;
using BackEndPeliculas.Models;

namespace BackEndPeliculas.Services
{
    public class PeliculasRepository : IPeliculasRepository
    {
        private readonly MoviesContext _listaPeliculas;

        public PeliculasRepository()
        {
        }
        public IEnumerable<Peliculas> AllPeliculas
        {
            
            get { Console.WriteLine("Entre a todas las peliculas"); return _listaPeliculas.Peliculas.ToList(); }
        }

        public void Delete(int id)
        {
            _listaPeliculas.Remove(this.Find(id));
            _listaPeliculas.SaveChanges();        }

        public bool DoesItemExist(int id)
        {
            return _listaPeliculas.Peliculas.Any(item => item.ID == id);
        }

        public Peliculas Find(int id)
        {
            return _listaPeliculas.Peliculas.FirstOrDefault(item => item.ID == id);
        }

        public void Insert(Peliculas pelicula)
        {
            _listaPeliculas.Peliculas.Add(pelicula);
            _listaPeliculas.SaveChanges();
        }

        public void Update(Peliculas pelicula)
        {
            _listaPeliculas.Update(pelicula);
            _listaPeliculas.SaveChanges();
        }

    }
}
