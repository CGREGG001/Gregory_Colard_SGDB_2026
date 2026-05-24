namespace AnimalShelter.DAL.Queries
{
    public class AddressQueries
    {
        public const string Insert = @"
        INSERT INTO addresses (street, number, box, post_code, city, country)
        VALUES (@street, @number, @box, @post_code, @city, @country)
        RETURNING id_address;";

        public const string GetById = @"
            SELECT *
            FROM addresses
            WHERE id_address = @id AND deleted_at IS NULL;";

        public const string Update = @"
            UPDATE addresses SET
                street = @street, number = @number, box = @box,
                post_code = @post_code, city = @city, country = @country
            WHERE id_address = @id;";
    }
}
