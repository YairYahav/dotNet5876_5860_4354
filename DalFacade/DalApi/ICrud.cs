using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalApi
{
    public interface ICrud<T> where T : class
    {
        // Create a new entity 
        // May cause some problems and to throw exception id an ID already exists.
        void Create(T item);


        // Read entity by its ID
        // Won't throw exception but return null if not found
        T? Read(int id);


        // Read entity object by predicate
        T? Read(Func<T, bool> filter); // Stage 2


        // Read all the entities
        // Stage 1: IEnumerable<T> ReadAll();
        IEnumerable<T> ReadAll(Func<T, bool>? filter = null); // stage 2



        // Update an existing entity by ID
        void Update(T item);


        // Dekete an entity by ID
        void Delete(int id);


        // Delete all entities 
        void DeleteAll();
    }
}

