using AnimalShelter.Core.Interfaces;
using AnimalShelter.Core.Models;
using AnimalShelter.DAL.Infrastructure;

namespace AnimalShelter.DAL.Repositories;

public class AnimalRepository : IAnimalRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public AnimalRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public Task<string> AddAsync(Animal animal)
    {
        throw new NotImplementedException();
    }

    public Task<Animal?> GetByIdAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Animal>> GetAllActiveAsync()
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAsync(Animal animal)
    {
        throw new NotImplementedException();
    }
    public Task<bool> DeleteAsync(string id)
    {
        throw new NotImplementedException();
    }
}
