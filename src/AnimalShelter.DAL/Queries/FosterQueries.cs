namespace AnimalShelter.DAL.Queries
{
    public class FosterQueries
    {
        public const string Insert = @"SELECT sp_foster_insert(
            @id_animal, @id_person, @start_date::date
        )";

        public const string GetByAnimal = "SELECT * FROM sp_foster_get_by_animal(@id_animal)";

        public const string GetByContact = "SELECT * FROM sp_foster_get_active_by_contact(@id_person)";

        public const string EndStay = "SELECT sp_foster_end_stay(@id_foster, @end_date::date)";

        public const string GetHistoryByContact = "SELECT * FROM sp_foster_get_history_by_contact(@id_person)";
    }
}
