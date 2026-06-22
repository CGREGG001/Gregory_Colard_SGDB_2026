namespace AnimalShelter.DAL.Queries
{
    public class AdoptionQueries
    {
        public const string Insert = "SELECT sp_adoption_insert(@id_animal, @id_person, @status)";

        public const string GetAll = "SELECT * FROM sp_adoption_get_all()";

        public const string GetById = "SELECT * FROM sp_adoption_get_by_id(@id)";

        public const string UpdateStatus = "SELECT sp_adoption_update_status(@id, @status)";

        public const string GetByAnimal = "SELECT * FROM sp_adoption_get_by_animal(@id_animal)";
        
        public const string GetByContact = "SELECT * FROM sp_adoption_get_by_contact(@id_person)";
    }
}
