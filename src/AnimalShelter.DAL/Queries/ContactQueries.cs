namespace AnimalShelter.DAL.Queries
{
    public class ContactQueries
    {
        public const string Insert = @"
        INSERT INTO contact_persons 
            (id_address, last_name, first_name, national_register_encrypted, gsm, phone, email, role_flags, rgpd_consent_date)
        VALUES 
            (@id_address, @last_name, @first_name, @nr_encrypted, @gsm, @phone, @email, @role_flags, @rgpd_date)
        RETURNING id_person;";

        public const string GetById = @"
            SELECT *
            FROM contact_persons
            WHERE id_person = @id AND deleted_at IS NULL;";

        public const string GetAll = @"
            SELECT *
            FROM contact_persons
            WHERE deleted_at IS NULL ORDER BY last_name, first_name;";

        public const string Update = @"
            UPDATE contact_persons SET 
                last_name = @last_name, first_name = @first_name, gsm = @gsm, 
                phone = @phone, email = @email, role_flags = @role_flags
            WHERE id_person = @id;";

        public const string SoftDelete = @"
            UPDATE contact_persons SET
            deleted_at = CURRENT_TIMESTAMP
            WHERE id_person = @id;";
    }
}
