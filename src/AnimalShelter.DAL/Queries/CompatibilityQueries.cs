namespace AnimalShelter.DAL.Queries
{
    public static class CompatibilityQueries
    {
        // Utilisation de ON CONFLICT pour "Ajouter ou Mettre à jour" (Upsert)
        public const string Upsert = @"
            INSERT INTO compatibilities (id_animal, target_type, value, description)
            VALUES (@id_animal, @type, @value, @desc)
            ON CONFLICT (id_animal, target_type) 
            DO UPDATE SET value = @value, description = @desc, deleted_at = NULL;
            ";

        public const string GetByAnimal = @"
            SELECT * FROM compatibilities 
            WHERE id_animal = @id_animal AND deleted_at IS NULL;
            ";

        public const string SoftDelete = @"
            UPDATE compatibilities SET deleted_at = CURRENT_TIMESTAMP 
            WHERE id_animal = @id_animal AND target_type = @type;
            ";
    }
}
