using AnimalShelter.Core.Interfaces;
using AnimalShelter.Core.Models;
using AnimalShelter.Core.Enums;
using AnimalShelter.DAL.Infrastructure;
using AnimalShelter.DAL.Queries;
using AnimalShelter.DAL.Mappers;

namespace AnimalShelter.DAL.Repositories;

public class AdoptionRepository(DbConnectionFactory connectionFactory) : IAdoptionRepository
{
    private readonly DbConnectionFactory _connectionFactory = connectionFactory;

    public async Task<Guid> AddAsync(AdoptionFile file)
    {
        await using var conn = _connectionFactory.CreateConnection();

        await conn.OpenAsync();

        return await DbHelper.ExecuteScalarAsync<Guid>(
            conn,
            AdoptionQueries.Insert,
            cmd =>
            {
                cmd.Parameters.AddWithValue("id_animal", file.AnimalId);
                cmd.Parameters.AddWithValue("id_person", file.ContactId);
                cmd.Parameters.AddWithValue("status", file.Status);
            }
        );
    }

    public async Task<IEnumerable<AdoptionFile>> GetAllAsync()
    {
        await using var connection = _connectionFactory.CreateConnection();

        await connection.OpenAsync();

        return await DbHelper.QueryListAsync(connection, AdoptionQueries.GetAll, null, AdoptionMapper.Map);
    }

    public async Task<AdoptionFile?> GetByIdAsync(Guid id)
    {
        await using var connection = _connectionFactory.CreateConnection();

        await connection.OpenAsync();

        return await DbHelper.QuerySingleAsync(connection, AdoptionQueries.GetById,
            cmd => cmd.Parameters.AddWithValue("id", id), AdoptionMapper.Map);
    }

    public async Task<bool> UpdateStatusAsync(Guid id, AdoptionStatusEnum status)
    {
        await using var connection = _connectionFactory.CreateConnection();

        await connection.OpenAsync();

        return await DbHelper.ExecuteNonQueryAsync(connection, AdoptionQueries.UpdateStatus, cmd =>
        {
            cmd.Parameters.AddWithValue("id", id);
            cmd.Parameters.AddWithValue("status", status);
        }) > 0;
    }
}
