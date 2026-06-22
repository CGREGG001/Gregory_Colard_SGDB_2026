using AnimalShelter.Core.Interfaces;
using AnimalShelter.Core.Models;
using AnimalShelter.Core.Enums;
using AnimalShelter.DAL.Infrastructure;
using AnimalShelter.DAL.Queries;
using AnimalShelter.DAL.Mappers;
using System.Data;

namespace AnimalShelter.DAL.Repositories;

public class CompatibilityRepository(DbConnectionFactory factory) : ICompatibilityRepository
{
    private readonly DbConnectionFactory _connectionFactory = factory;

    public async Task SaveAsync(Compatibility c)
    {
        await using var connection = _connectionFactory.CreateConnection();

        await connection.OpenAsync();

        await DbHelper.ExecuteScalarAsync<int>(connection, CompatibilityQueries.Upsert, cmd =>
        {
            cmd.Parameters.AddWithValue("id_animal", c.AnimalId);
            cmd.Parameters.AddWithValue("type", c.TargetType);
            cmd.Parameters.AddWithValue("value", c.ValueEnum);
            cmd.Parameters.AddWithValue("desc", DbHelper.DbValue(c.Description));
        });
    }

    public async Task<IEnumerable<Compatibility>> GetByAnimalIdAsync(string animalId)
    {
        await using var connection = _connectionFactory.CreateConnection();

        await connection.OpenAsync();

        return await DbHelper.QueryListAsync(connection, CompatibilityQueries.GetByAnimal,
            cmd => cmd.Parameters.AddWithValue("id_animal", animalId),
            CompatibilityMapper.Map);
    }

    public async Task<bool> DeleteAsync(string animalId, CompatibilityTypeEnum type)
    {
        await using var connection = _connectionFactory.CreateConnection();

        await connection.OpenAsync();

        var rows = await DbHelper.ExecuteScalarAsync<int>(connection, CompatibilityQueries.SoftDelete, cmd =>
        {
            cmd.Parameters.AddWithValue("id_animal", animalId);
            cmd.Parameters.AddWithValue("type", type);
        });

        return rows > 0;
    }
}
