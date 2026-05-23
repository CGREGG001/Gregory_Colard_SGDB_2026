using AnimalShelter.Core.Enums;
using AnimalShelter.Core.Models;
using Npgsql;

namespace AnimalShelter.DAL.Mappers
{
    public static class ContactMapper
    {
        public static Contact Map(NpgsqlDataReader reader)
        {
            return new Contact
            {
                Id = reader.GetGuid(reader.GetOrdinal("id_person")),
                AddressId = reader.IsDBNull(reader.GetOrdinal("id_address")) ? null : reader.GetGuid(reader.GetOrdinal("id_address")),
                LastName = reader.GetString(reader.GetOrdinal("last_name")),
                FirstName = reader.GetString(reader.GetOrdinal("first_name")),
                NationalRegisterEncrypted = reader.IsDBNull(reader.GetOrdinal("national_register_encrypted")) ? null : (byte[])reader["national_register_encrypted"],
                Gsm = reader.IsDBNull(reader.GetOrdinal("gsm")) ? null : reader.GetString(reader.GetOrdinal("gsm")),
                Phone = reader.IsDBNull(reader.GetOrdinal("phone")) ? null : reader.GetString(reader.GetOrdinal("phone")),
                Email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString(reader.GetOrdinal("email")),
                RoleFlags = (ContactRolesEnum)reader.GetInt16(reader.GetOrdinal("role_flags")),
                RgpdConsentDate = reader.IsDBNull(reader.GetOrdinal("rgpd_consent_date")) ? null : reader.GetDateTime(reader.GetOrdinal("rgpd_consent_date")),
                IsAnonymised = reader.GetBoolean(reader.GetOrdinal("is_anonymised")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
            };
        }
    }
}
