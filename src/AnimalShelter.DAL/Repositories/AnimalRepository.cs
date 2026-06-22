using AnimalShelter.Core.Interfaces;
using AnimalShelter.Core.Models;
using AnimalShelter.DAL.Infrastructure;
using AnimalShelter.DAL.Mappers;
using AnimalShelter.DAL.Queries;
using Npgsql;

namespace AnimalShelter.DAL.Repositories;

public class AnimalRepository(DbConnectionFactory connectionFactory) : IAnimalRepository
{
    private readonly DbConnectionFactory _connectionFactory = connectionFactory;

    public async Task<string> AddAsync(Animal animal)
    {
        const string query = AnimalQueries.Insert;
        await using var connection = await GetOpenConnectionAsync();

        var result = await DbHelper.ExecuteScalarAsync<string>(
            connection,
            query,
            cmd =>
            {
                cmd.Parameters.AddWithValue("name", animal.Name);
                cmd.Parameters.AddWithValue("species", animal.Species);
                cmd.Parameters.AddWithValue("sex", animal.Sex);
                cmd.Parameters.AddWithValue("colors", DbHelper.DbValue(animal.Colors));
                cmd.Parameters.AddWithValue("is_sterilised", animal.IsSterilised);
                cmd.Parameters.AddWithValue("sterilisation_date", DbHelper.DbValue(animal.SterilisationDate));
                cmd.Parameters.AddWithValue("birth_date", DbHelper.DbValue(animal.BirthDate));
                cmd.Parameters.AddWithValue("description", DbHelper.DbValue(animal.Description));
                cmd.Parameters.AddWithValue("particularities", DbHelper.DbValue(animal.Particularities));
            }
        );

        return result?.ToString() ?? throw new Exception("Error: Could not retrieve generated Animal ID.");
    }

    public async Task<Animal?> GetByIdAsync(string id)
    {
        const string query = AnimalQueries.GetById;
        await using var connection = await GetOpenConnectionAsync();

        return await DbHelper.QuerySingleAsync(
            connection,
            query,
            cmd => cmd.Parameters.AddWithValue("id", id),
            AnimalMapper.Map
        );
    }

    public async Task<IEnumerable<Animal>> GetAllActiveAsync()
    {
        const string query = AnimalQueries.GetAllActive;
        await using var connection = await GetOpenConnectionAsync();

        return await DbHelper.QueryListAsync(
            connection,
            query,
            bind: null,
            AnimalMapper.Map
        );
    }

    public async Task<bool> UpdateAsync(Animal animal)
    {
        const string query = AnimalQueries.Update;
        await using var connection = await GetOpenConnectionAsync();

        var rows = await DbHelper.ExecuteScalarAsync<int>(
            connection,
            query,
            cmd =>
            {
                cmd.Parameters.AddWithValue("id", animal.Id);
                cmd.Parameters.AddWithValue("name", animal.Name);
                cmd.Parameters.AddWithValue("colors", DbHelper.DbValue(animal.Colors));
                cmd.Parameters.AddWithValue("description", DbHelper.DbValue(animal.Description));
                cmd.Parameters.AddWithValue("particularities", DbHelper.DbValue(animal.Particularities));
                cmd.Parameters.AddWithValue("status", animal.CurrentStatus);
            }
        );

        return rows > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        const string query = AnimalQueries.SoftDelete;
        await using var connection = await GetOpenConnectionAsync();

        var rows = await DbHelper.ExecuteScalarAsync<int>(
            connection,
            query,
            cmd => cmd.Parameters.AddWithValue("id", id)
        );

        return rows > 0;
    }

    private async Task<NpgsqlConnection> GetOpenConnectionAsync()
    {
        var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        return connection;
    }
}
