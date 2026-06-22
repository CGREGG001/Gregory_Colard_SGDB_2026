using Npgsql;
using AnimalShelter.Core.Interfaces;
using AnimalShelter.Core.Models;
using AnimalShelter.DAL.Infrastructure;
using AnimalShelter.DAL.Queries;
using AnimalShelter.DAL.Mappers;

namespace AnimalShelter.DAL.Repositories;

public class VaccinationRepository(DbConnectionFactory connectionFactory) : IVaccinationRepository
{
    private readonly DbConnectionFactory _connectionFactory = connectionFactory;

    private async Task<NpgsqlConnection> GetOpenConnectionAsync()
    {
        var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        return connection;
    }

    public async Task<Guid> AddAsync(Vaccination v)
    {
        await using var connection = await GetOpenConnectionAsync();

        return await DbHelper.ExecuteScalarAsync<Guid>(
            connection,
            VaccinationQueries.Insert,
            cmd =>
        {
            cmd.Parameters.AddWithValue("id_animal", v.AnimalId);
            cmd.Parameters.AddWithValue("name", v.VaccineName);
            cmd.Parameters.AddWithValue("date", v.VaccineDate);
            cmd.Parameters.AddWithValue("is_done", v.IsDone);
        });
    }

    public async Task<IEnumerable<Vaccination>> GetByAnimalIdAsync(string animalId)
    {
        await using var connection = await GetOpenConnectionAsync();

        return await DbHelper.QueryListAsync(
            connection,
            VaccinationQueries.GetByAnimal,
            cmd => cmd.Parameters.AddWithValue("id_animal", animalId),
            VaccinationMapper.Map
        );
    }

    public async Task<bool> UpdateAsync(Vaccination v)
    {
        await using var connection = await GetOpenConnectionAsync();

        var rows = await DbHelper.ExecuteScalarAsync<int>(
            connection,
            VaccinationQueries.Update,
            cmd =>
            {
                cmd.Parameters.AddWithValue("id",      v.Id);
                cmd.Parameters.AddWithValue("name",    v.VaccineName);
                cmd.Parameters.AddWithValue("date",    v.VaccineDate);
                cmd.Parameters.AddWithValue("is_done", v.IsDone);
            });

        return rows > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        await using var connection = await GetOpenConnectionAsync();

        var rows = await DbHelper.ExecuteScalarAsync<int>(
            connection,
            VaccinationQueries.SoftDelete,
            cmd => cmd.Parameters.AddWithValue("id", id));

        return rows > 0;
    }
}
