namespace AnimalShelter.DAL.Queries
{
    public class FosterQueries
    {
        public const string Insert = @"
            INSERT INTO foster_stays (id_animal, id_person, start_date)
            VALUES (@id_animal, @id_person, @start_date);
            ";

        public const string GetByAnimal = @"
            SELECT f.*, c.*
            FROM foster_stays f
            JOIN contacts c ON f.id_person = c.id_person
            WHERE f.id_animal = @id_animal;
            ";

        public const string GetByContact = @"
            SELECT f.*, a.*
            FROM foster_stays f
            JOIN animals a ON f.id_animal = a.id_animal
            WHERE f.id_person = @id_person;
            ";
    }
}
