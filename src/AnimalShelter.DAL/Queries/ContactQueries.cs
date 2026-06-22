namespace AnimalShelter.DAL.Queries
{
    public class ContactQueries
    {
        public const string Register = @"SELECT sp_contact_register(
            @street, 
            @number, 
            @box, 
            @post_code, 
            @city, 
            @country, 
            @last_name, 
            @first_name, 
            @nr_encrypted, 
            @nr_hash, 
            @gsm, 
            @phone, 
            @email, 
            @role_flags, 
            @rgpd_date
        )";

        public const string GetById = "SELECT * FROM sp_contact_get_by_id(@id)";

        public const string GetAll = "SELECT * FROM sp_contact_get_all()";

        public const string Updatefull = @"SELECT sp_contact_update_full(
            @contact_id, 
            @last_name, 
            @first_name, 
            @gsm, 
            @phone, 
            @email, 
            @role_flags, 
            @id_address, 
            @street, 
            @number, 
            @box, 
            @post_code, 
            @city, 
            @country
        )";

        public const string SoftDelete = "SELECT sp_contact_soft_delete(@id)";
    }
}
