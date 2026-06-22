namespace AnimalShelter.DAL.Queries
{
    public class AnimalQueries
    {
        public const string Insert = @"
            INSERT INTO animals 
                (name,
                species,
                sex,
                colors,
                is_sterilised,
                sterilisation_date,
                birth_date,
                description,
                particularities)
            VALUES 
                (@name,
                @species,
                @sex,
                @colors,
                @is_sterilised,
                @sterilisation_date,
                @birth_date,
                @description,
                @particularities)
            RETURNING id_animal;
        ";

        public const string GetById = @"
            SELECT *
            FROM animals
            WHERE id_animal = @id AND deleted_at IS NULL;
        ";

        public const string GetAllActive = @"
            SELECT *
            FROM animals
            WHERE deleted_at IS NULL 
            ORDER BY created_at DESC;
        ";

        public const string Update = @"
            UPDATE animals SET 
                name = @name,
                colors = @colors,
                description = @description,
                particularities = @particularities,
                current_status = @status
            WHERE id_animal = @id;
        ";

        public const string SoftDelete = @"
            UPDATE animals SET
                deleted_at = CURRENT_TIMESTAMP
            WHERE id_animal = @id;
        ";
    }
}