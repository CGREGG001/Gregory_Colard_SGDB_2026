using AnimalShelter.Core.Models;
using Npgsql;

namespace AnimalShelter.DAL.Mappers
{
    public static class AddressMapper
    {
        public static Address Map(NpgsqlDataReader reader)
        {
            return new Address
            {
                Id = reader.GetGuid(reader.GetOrdinal("id_address")),
                Street = reader.GetString(reader.GetOrdinal("street")),
                Number = reader.GetString(reader.GetOrdinal("number")),
                Box = reader.IsDBNull(reader.GetOrdinal("box")) ? null : reader.GetString(reader.GetOrdinal("box")),
                PostCode = reader.GetString(reader.GetOrdinal("post_code")),
                City = reader.GetString(reader.GetOrdinal("city")),
                Country = reader.GetString(reader.GetOrdinal("country")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
            };
        }
    }
}
