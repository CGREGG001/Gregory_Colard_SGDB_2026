using AnimalShelter.Core.Models;
using Npgsql;

namespace AnimalShelter.DAL.Mappers;

public static class FosterMapper
{
    public static FosterStay Map(NpgsqlDataReader reader)
    {
        FosterStay stay = new FosterStay
        {
            Id = reader.GetGuid(reader.GetOrdinal("id_foster")),
            AnimalId = reader.GetString(reader.GetOrdinal("id_animal")),
            ContactId = reader.GetGuid(reader.GetOrdinal("id_person")),
            StartDate = reader.GetDateTime(reader.GetOrdinal("start_date")),
            EndDate = reader.IsDBNull(reader.GetOrdinal("end_date")) ? null : reader.GetDateTime(reader.GetOrdinal("end_date"))
        };

        // Mapping des colonnes de jointure (AnimalName)
        int animalNameOrdinal = reader.GetOrdinal("animal_name");

        if (!reader.IsDBNull(animalNameOrdinal))
        {
            stay.AnimalName = reader.GetString(animalNameOrdinal);
        }

        // Mapping des colonnes de jointure (ContactName : Prénom + Nom)
        int firstNameOrdinal = reader.GetOrdinal("first_name");
        int lastNameOrdinal = reader.GetOrdinal("last_name");
        
        if (!reader.IsDBNull(firstNameOrdinal) && !reader.IsDBNull(lastNameOrdinal))
        {
            stay.ContactName = $"{reader.GetString(firstNameOrdinal)} {reader.GetString(lastNameOrdinal)}";
        }

        return stay;
    }
}
