using System.Collections.Generic;
using BackEndPeliculas.Models;


namespace BackEndPeliculas.Interfaces
{
    public interface IPeliculasRepository
    {
        bool DoesItemExist(int id);
        IEnumerable<Peliculas> AllPeliculas { get; }
        Peliculas Find(int id);
        void Insert(Peliculas pelicula);
        void Update(Peliculas pelicula);
        void Delete(int id);
    }
}
