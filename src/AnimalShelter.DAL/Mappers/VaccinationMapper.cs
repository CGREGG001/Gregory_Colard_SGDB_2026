using AnimalShelter.Core.Models;
using Npgsql;

namespace AnimalShelter.DAL.Mappers;

public static class VaccinationMapper
{
    public static Vaccination Map(NpgsqlDataReader reader)
    {
        return new Vaccination
        {
            Id = reader.GetGuid(reader.GetOrdinal("id_vaccin")),
            AnimalId = reader.GetString(reader.GetOrdinal("id_animal")),
            VaccineName = reader.GetString(reader.GetOrdinal("vaccine_name")),
            VaccineDate = reader.GetDateTime(reader.GetOrdinal("vaccine_date")),
            IsDone = reader.GetBoolean(reader.GetOrdinal("is_done")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
        };
    }
}
