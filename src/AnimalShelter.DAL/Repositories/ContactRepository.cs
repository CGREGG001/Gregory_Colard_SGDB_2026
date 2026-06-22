using AnimalShelter.Core.Interfaces;
using AnimalShelter.Core.Models;
using AnimalShelter.DAL.Infrastructure;
using AnimalShelter.DAL.Mappers;
using AnimalShelter.DAL.Queries;
using Npgsql;

namespace AnimalShelter.DAL.Repositories
{
    public class ContactRepository(DbConnectionFactory connectionFactory) : IContactRepository
    {
        private readonly DbConnectionFactory _connectionFactory = connectionFactory;

        public async Task<Guid> AddAsync(Contact contact)
        {
            await using var connection = await GetOpenConnectionAsync();

            // Utilisation d'une transaction pour garantir l'intégrité (Adresse + Contact)
            await using var transaction = await connection.BeginTransactionAsync();

            try
            {
                // 1. Si une adresse est fournie, on l'insère d'abord
                if (contact.Address != null)
                {
                    await using var addrCmd = new NpgsqlCommand(AddressQueries.Insert, connection, transaction);
                    addrCmd.Parameters.AddWithValue("street", contact.Address.Street);
                    addrCmd.Parameters.AddWithValue("number", contact.Address.Number);
                    addrCmd.Parameters.AddWithValue("box", (object?)contact.Address.Box ?? DBNull.Value);
                    addrCmd.Parameters.AddWithValue("post_code", contact.Address.PostCode);
                    addrCmd.Parameters.AddWithValue("city", contact.Address.City);
                    addrCmd.Parameters.AddWithValue("country", contact.Address.Country);

                    var addrId = await addrCmd.ExecuteScalarAsync();
                    contact.AddressId = (Guid)addrId!;
                }

                // 2. Insertion du contact
                await using var cmd = new NpgsqlCommand(ContactQueries.Insert, connection, transaction);
                cmd.Parameters.AddWithValue("id_address", (object?)contact.AddressId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("last_name", contact.LastName);
                cmd.Parameters.AddWithValue("first_name", contact.FirstName);
                cmd.Parameters.AddWithValue("nr_encrypted", (object?)contact.NationalRegisterEncrypted ?? DBNull.Value);
                cmd.Parameters.AddWithValue("nr_hash", (object?)contact.NationalRegisterHash ?? DBNull.Value);
                cmd.Parameters.AddWithValue("gsm", (object?)contact.Gsm ?? DBNull.Value);
                cmd.Parameters.AddWithValue("phone", (object?)contact.Phone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("email", (object?)contact.Email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("role_flags", (short)contact.RoleFlags);
                cmd.Parameters.AddWithValue("rgpd_date", (object?)contact.RgpdConsentDate ?? DBNull.Value);

                var contactId = await cmd.ExecuteScalarAsync();

                await transaction.CommitAsync();
                return (Guid)contactId!;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw; // L'exception sera rattrapée par la BLL
            }
        }

        public async Task<Contact?> GetByIdAsync(Guid id)
        {
            const string query = ContactQueries.GetById;

            await using var connection = await GetOpenConnectionAsync();

            await using var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("id", id);

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                Contact contact = ContactMapper.Map(reader);

                // Si une adresse existe dans la jointure, on la mappe aussi
                if (!reader.IsDBNull(reader.GetOrdinal("id_address")))
                {
                    contact.Address = AddressMapper.Map(reader);
                }

                return contact;
            }
            return null;
        }

        public async Task<IEnumerable<Contact>> GetAllAsync()
        {
            List<Contact> contacts = [];
            await using var connection = await GetOpenConnectionAsync();

            await using var cmd = new NpgsqlCommand(ContactQueries.GetAll, connection);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                contacts.Add(ContactMapper.Map(reader));
            }
            return contacts;
        }

        public async Task<bool> UpdateAsync(Contact contact)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            await using var transaction = await connection.BeginTransactionAsync();

            try
            {
                // Update Adresse si présente
                if (contact.Address != null)
                {
                    await using var addrCmd = new NpgsqlCommand(AddressQueries.Update, connection, transaction);
                    addrCmd.Parameters.AddWithValue("id", contact.Address.Id);
                    addrCmd.Parameters.AddWithValue("street", contact.Address.Street);
                    addrCmd.Parameters.AddWithValue("number", contact.Address.Number);
                    addrCmd.Parameters.AddWithValue("box", (object?)contact.Address.Box ?? DBNull.Value);
                    addrCmd.Parameters.AddWithValue("post_code", contact.Address.PostCode);
                    addrCmd.Parameters.AddWithValue("city", contact.Address.City);
                    addrCmd.Parameters.AddWithValue("country", contact.Address.Country);
                    await addrCmd.ExecuteNonQueryAsync();
                }

                // Update Contact
                await using var cmd = new NpgsqlCommand(ContactQueries.Update, connection, transaction);
                cmd.Parameters.AddWithValue("id", contact.Id);
                cmd.Parameters.AddWithValue("last_name", contact.LastName);
                cmd.Parameters.AddWithValue("first_name", contact.FirstName);
                cmd.Parameters.AddWithValue("gsm", (object?)contact.Gsm ?? DBNull.Value);
                cmd.Parameters.AddWithValue("phone", (object?)contact.Phone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("email", (object?)contact.Email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("role_flags", (short)contact.RoleFlags);

                var rows = await cmd.ExecuteNonQueryAsync();
                await transaction.CommitAsync();
                return rows > 0;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            await using var cmd = new NpgsqlCommand(ContactQueries.SoftDelete, connection);
            cmd.Parameters.AddWithValue("id", id);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        private async Task<NpgsqlConnection> GetOpenConnectionAsync()
        {
            var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();
            return connection;
        }
    }
}
