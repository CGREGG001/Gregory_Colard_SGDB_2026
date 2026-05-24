using AnimalShelter.Core.Models;
using AnimalShelter.Core.Enums;
using Npgsql;

namespace AnimalShelter.DAL.Mappers;

public static class AdoptionMapper
{
    public static AdoptionFile Map(NpgsqlDataReader reader)
    {
        var file = new AdoptionFile
        {
            Id = reader.GetGuid(reader.GetOrdinal("id_adoption")),
            AnimalId = reader.GetString(reader.GetOrdinal("id_animal")),
            ContactId = reader.GetGuid(reader.GetOrdinal("id_person")),
            RequestDate = reader.GetDateTime(reader.GetOrdinal("request_date")),
            Status = reader.GetFieldValue<AdoptionStatusEnum>(reader.GetOrdinal("status")),
            AnimalName = reader.GetString(reader.GetOrdinal("animal_name")),
            ContactName = $"{reader.GetString(reader.GetOrdinal("first_name"))} {reader.GetString(reader.GetOrdinal("last_name"))}"
        };
        return file;
    }
}
