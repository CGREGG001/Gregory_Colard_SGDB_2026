namespace AnimalShelter.DAL.Queries
{
    public class AnimalQueries
    {
        public const string Insert = @"SELECT sp_animal_insert(
            @name, @species, @sex, @colors, @is_sterilised, @sterilisation_date::date,
            @birth_date::date, @description, @particularities
        )";

        public const string GetById = "SELECT * FROM sp_animal_get_by_id(@id)";

        public const string GetAllActive = "SELECT * FROM sp_animal_get_all_active()";

        public const string Update = @"SELECT sp_animal_update(@id, @name, @colors, 
            @description, @particularities, @status
        )";

        public const string SoftDelete = "SELECT sp_animal_soft_delete(@id)";
    }
}
