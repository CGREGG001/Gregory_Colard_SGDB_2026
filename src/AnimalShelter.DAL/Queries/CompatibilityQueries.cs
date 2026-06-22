namespace AnimalShelter.DAL.Queries
{
    public static class CompatibilityQueries
    {
        // Utilisation de ON CONFLICT pour "Ajouter ou Mettre à jour" (Upsert)
        public const string Upsert = "SELECT sp_compatibility_upsert(@id_animal, @type, @value, @desc)";

        public const string GetByAnimal = "SELECT * FROM sp_compatibility_get_by_animal(@id_animal)";

        public const string SoftDelete = "SELECT sp_compatibility_soft_delete(@id_animal, @type)";
    }
}
