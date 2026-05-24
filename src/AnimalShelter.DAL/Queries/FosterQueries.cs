namespace AnimalShelter.DAL.Queries
{
    public class FosterQueries
    {
        public const string Insert = @"
            INSERT INTO foster_stays (id_animal, id_person, start_date)
            VALUES (@id_animal, @id_person, @start_date)
            RETURNING id_foster;
            ";

        public const string GetByAnimal = @"
            SELECT f.*, c.first_name, c.last_name 
            FROM foster_stays f
            JOIN contacts c ON f.id_person = c.id_person
            WHERE f.id_animal = @id_animal
            ORDER BY f.start_date DESC;
            ";

        public const string GetByContact = @"
            SELECT f.*, a.name as animal_name, a.species
            FROM foster_stays f
            JOIN animals a ON f.id_animal = a.id_animal
            WHERE f.id_person = @id_person AND f.end_date IS NULL;
            ";

        public const string EndStay = @"
            UPDATE foster_stays SET end_date = @end_date 
            WHERE id_foster = @id_foster;
            ";
    }
}
