namespace AnimalShelter.DAL.Queries
{
    public class CompatibilityQueries
    {
        public const string Insert = @"
            INSERT INTO compatibilities (id_animal, target_type, value, description)
            VALUES (@id_animal, @type, @val, @desc)
            ON CONFLICT (id_animal, target_type) DO UPDATE SET
            value = @val, description = @desc;
            ";

        public const string Delete = @"
            UPDATE compatibilities SET
            deleted_at = CURRENT_TIMESTAMP
            WHERE id_animal = @id_animal AND target_type = @type;
            ";
    }
}
