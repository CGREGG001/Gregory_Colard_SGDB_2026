namespace AnimalShelter.DAL.Queries
{
    public class ContactQueries
    {
        public const string Insert = @"
        INSERT INTO contacts 
            (id_address, last_name, first_name, national_register_encrypted, gsm, phone, email, role_flags, rgpd_consent_date)
        VALUES 
            (@id_address, @last_name, @first_name, @nr_encrypted, @gsm, @phone, @email, @role_flags, @rgpd_date)
        RETURNING id_person;";

        public const string GetById = @"
            SELECT c.*, a.* 
            FROM contacts c 
            LEFT JOIN addresses a ON c.id_address = a.id_address 
            WHERE c.id_person = @id AND c.deleted_at IS NULL;";

        public const string GetAll = @"
            SELECT *
            FROM contacts
            WHERE deleted_at IS NULL ORDER BY last_name, first_name;";

        public const string Update = @"
            UPDATE contacts SET 
                last_name = @last_name, first_name = @first_name, gsm = @gsm, 
                phone = @phone, email = @email, role_flags = @role_flags
            WHERE id_person = @id;";

        public const string SoftDelete = @"
            UPDATE contacts SET
            deleted_at = CURRENT_TIMESTAMP
            WHERE id_person = @id;";
    }
}
