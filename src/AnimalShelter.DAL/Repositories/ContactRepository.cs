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

            // sp_contact_register gère l'insertion adresse + contact atomiquement
            var contactId = await DbHelper.ExecuteScalarAsync<Guid>(
                connection,
                ContactQueries.Register,
                cmd =>
                {
                    cmd.Parameters.AddWithValue("street",      (object?)contact.Address?.Street    ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("number",      (object?)contact.Address?.Number    ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("box",         (object?)contact.Address?.Box       ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("post_code",   (object?)contact.Address?.PostCode  ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("city",        (object?)contact.Address?.City      ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("country",     (object?)contact.Address?.Country   ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("last_name",   contact.LastName);
                    cmd.Parameters.AddWithValue("first_name",  contact.FirstName);
                    cmd.Parameters.AddWithValue("nr_encrypted",(object?)contact.NationalRegisterEncrypted ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("nr_hash",     (object?)contact.NationalRegisterHash      ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("gsm",         (object?)contact.Gsm           ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("phone",       (object?)contact.Phone         ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("email",       (object?)contact.Email         ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("role_flags",  (short)contact.RoleFlags);
                    cmd.Parameters.AddWithValue("rgpd_date",   (object?)contact.RgpdConsentDate ?? DBNull.Value);
                }
            );

            return contactId;
        }

        public async Task<Contact?> GetByIdAsync(Guid id)
        {
            const string query = ContactQueries.GetById;

            await using var connection = await GetOpenConnectionAsync();
            
            var contact = await DbHelper.QuerySingleAsync(
                connection,
                query,
                cmd => cmd.Parameters.AddWithValue("id", id),
                ContactMapper.Map
            );

            if (contact != null && contact.AddressId.HasValue)
            {
                contact.Address = await DbHelper.QuerySingleAsync(
                    connection,
                    AddressQueries.GetById,
                    cmd => cmd.Parameters.AddWithValue("id", contact.AddressId.Value),
                    AddressMapper.Map
                );
            }

            return contact;
        }

        public async Task<IEnumerable<Contact>> GetAllAsync()
        {
            List<Contact> contacts = [];
            await using var connection = await GetOpenConnectionAsync();

            return await DbHelper.QueryListAsync(
                connection,
                ContactQueries.GetAll,
                bind: null,
                ContactMapper.Map
            );
        }

        public async Task<bool> UpdateAsync(Contact contact)
        {
            await using var connection = await GetOpenConnectionAsync();

            // sp_contact_update_full gère la mise à jour adresse + contact atomiquement
            var rows = await DbHelper.ExecuteScalarAsync<int>(
                connection,
                ContactQueries.Updatefull,
                cmd =>
                {
                    cmd.Parameters.AddWithValue("contact_id", contact.Id);
                    cmd.Parameters.AddWithValue("last_name",  contact.LastName);
                    cmd.Parameters.AddWithValue("first_name", contact.FirstName);
                    cmd.Parameters.AddWithValue("gsm",        (object?)contact.Gsm ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("phone",      (object?)contact.Phone ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("email",      (object?)contact.Email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("role_flags", (short)contact.RoleFlags);
                    cmd.Parameters.AddWithValue("id_address", (object?)contact.Address?.Id ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("street",     (object?)contact.Address?.Street ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("number",     (object?)contact.Address?.Number ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("box",        (object?)contact.Address?.Box ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("post_code",  (object?)contact.Address?.PostCode ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("city",       (object?)contact.Address?.City ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("country",    (object?)contact.Address?.Country ?? DBNull.Value);
                }
            );

            return rows > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            await using var connection = await GetOpenConnectionAsync();

            var rows = await DbHelper.ExecuteScalarAsync<int>(
                connection,
                ContactQueries.SoftDelete,
                cmd => cmd.Parameters.AddWithValue("id", id)
            );

            return rows > 0;
        }

        private async Task<NpgsqlConnection> GetOpenConnectionAsync()
        {
            var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();
            return connection;
        }
    }
}
