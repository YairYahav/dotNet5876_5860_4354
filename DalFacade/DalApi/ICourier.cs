﻿//// <summary> Interface for managing Courier entities in the Data Access Layer (DAL). </summary>
namespace DalApi;
using DO;
internal interface ICourier
{
    void Create(Courier item); //Creates new entity object in DAL
    Courier? Read(int id); //Reads entity object by its ID 
    List<Courier> ReadAll(); //stage 1 only, Reads all entity objects
    void Update(Courier item); //Updates entity object
    void Delete(int id); //Deletes an object by its Id
    void DeleteAll(); //Delete all entity objects
}
