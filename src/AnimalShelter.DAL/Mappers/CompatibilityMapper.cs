using AnimalShelter.Core.Models;
using AnimalShelter.Core.Enums;
using Npgsql;

namespace AnimalShelter.DAL.Mappers;

public static class CompatibilityMapper
{
    public static Compatibility Map(NpgsqlDataReader reader)
    {
        return new Compatibility
        {
            Id = reader.GetGuid(reader.GetOrdinal("id_compatibility")),
            AnimalId = reader.GetString(reader.GetOrdinal("id_animal")),
            TargetType = reader.GetFieldValue<CompatibilityTypeEnum>(reader.GetOrdinal("target_type")),
            ValueEnum = reader.GetFieldValue<CompatibilityValueEnum>(reader.GetOrdinal("value")),
            Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description"))
        };
    }
}
