using AnimalShelter.Core.Enums;
using AnimalShelter.Core.Models;
using Npgsql;

namespace AnimalShelter.DAL.Mappers
{
    public class AnimalMapper
    {
        public static Animal Map(NpgsqlDataReader reader)
        {
            return new Animal
            {
                Id = reader.GetString(reader.GetOrdinal("id_animal")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                Species = reader.GetFieldValue<SpeciesEnum>(reader.GetOrdinal("species")),
                Sex = reader.GetFieldValue<SexEnum>(reader.GetOrdinal("sex")),
                Colors = reader.IsDBNull(reader.GetOrdinal("colors")) ? null :
                    reader.GetString(reader.GetOrdinal("colors")),
                IsSterilised = reader.GetBoolean(reader.GetOrdinal("is_sterilised")),
                SterilisationDate = reader.IsDBNull(reader.GetOrdinal("sterilisation_date")) ? null :
                    reader.GetDateTime(reader.GetOrdinal("sterilisation_date")),
                BirthDate = reader.IsDBNull(reader.GetOrdinal("birth_date")) ? null :
                    reader.GetDateTime(reader.GetOrdinal("birth_date")),
                Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null :
                    reader.GetString(reader.GetOrdinal("description")),
                Particularities = reader.IsDBNull(reader.GetOrdinal("particularities")) ? null :
                    reader.GetString(reader.GetOrdinal("particularities")),
                CurrentStatus = reader.GetFieldValue<AnimalStatusEnum>(reader.GetOrdinal("current_status")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
            };
        }
    }
}
