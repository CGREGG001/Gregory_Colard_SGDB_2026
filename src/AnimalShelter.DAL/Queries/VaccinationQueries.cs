namespace AnimalShelter.DAL.Queries
{
    public static class VaccinationQueries
    {
        public const string Insert = "SELECT sp_vaccination_insert(@id_animal, @name, @date::date, @is_done)";

        public const string GetByAnimal = "SELECT * FROM sp_vaccination_get_by_animal(@id_animal)";

        public const string Update = "SELECT sp_vaccination_update(@id, @name, @date::date, @is_done)";

        public const string SoftDelete = "SELECT sp_vaccination_soft_delete(@id)";
 
    }
}
