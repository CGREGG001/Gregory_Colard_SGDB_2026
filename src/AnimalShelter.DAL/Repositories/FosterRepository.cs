using AnimalShelter.Core.Interfaces;
using AnimalShelter.Core.Models;
using AnimalShelter.DAL.Infrastructure;
using AnimalShelter.DAL.Queries;
using AnimalShelter.DAL.Mappers;

namespace AnimalShelter.DAL.Repositories;

public class FosterRepository(DbConnectionFactory db) : IFosterRepository
{
    private readonly DbConnectionFactory _db = db;

    public async Task<Guid> AddAsync(FosterStay stay)
    {
        await using var conn = _db.CreateConnection();
        await conn.OpenAsync();

        return await DbHelper.ExecuteScalarAsync<Guid>(conn, FosterQueries.Insert, cmd =>
        {
            cmd.Parameters.AddWithValue("id_animal",   stay.AnimalId);
            cmd.Parameters.AddWithValue("id_person",   stay.ContactId);
            cmd.Parameters.AddWithValue("start_date",  stay.StartDate);
        });
    }

    public async Task<IEnumerable<FosterStay>> GetStaysByAnimalIdAsync(string animalId)
    {
        await using var conn = _db.CreateConnection();
        await conn.OpenAsync();
        return await DbHelper.QueryListAsync(conn, FosterQueries.GetByAnimal,
            cmd => cmd.Parameters.AddWithValue("id_animal", animalId), FosterMapper.Map);
    }

    public async Task<IEnumerable<FosterStay>> GetStaysByContactIdAsync(Guid contactId)
    {
        await using var conn = _db.CreateConnection();
        await conn.OpenAsync();
        return await DbHelper.QueryListAsync(conn, FosterQueries.GetByContact,
            cmd => cmd.Parameters.AddWithValue("id_person", contactId), FosterMapper.Map);
    }

    public async Task<bool> EndStayAsync(Guid stayId, DateTime endDate)
    {
        await using var conn = _db.CreateConnection();
        await conn.OpenAsync();

        var rows = await DbHelper.ExecuteScalarAsync<int>(conn, FosterQueries.EndStay, cmd =>
        {
            cmd.Parameters.AddWithValue("id_foster", stayId);
            cmd.Parameters.AddWithValue("end_date",  endDate);
        });

        return rows > 0;
    }

    public async Task<IEnumerable<FosterStay>> GetHistoryByContactIdAsync(Guid contactId)
    {
        await using var conn = _db.CreateConnection();
        await conn.OpenAsync();

        return await DbHelper.QueryListAsync(conn, FosterQueries.GetHistoryByContact,
            cmd => cmd.Parameters.AddWithValue("id_person", contactId), FosterMapper.Map);
    }
}
