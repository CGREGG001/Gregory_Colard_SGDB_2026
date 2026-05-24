namespace AnimalShelter.DAL.Queries
{
    public class AdoptionQueries
    {
        public const string Insert = @"
            INSERT INTO adoption_files (id_animal, id_person, status)
            VALUES (@id_animal, @id_person, @status)
            RETURNING id_adoption;
            ";

        public const string GetAll = @"
            SELECT ad.*, a.name as animal_name, c.first_name, c.last_name
            FROM adoption_files ad
            JOIN animals a ON ad.id_animal = a.id_animal
            JOIN contacts c ON ad.id_person = c.id_person
            WHERE ad.deleted_at IS NULL
            ORDER BY ad.request_date DESC;
        ";

        public const string GetById = @"
            SELECT ad.*, a.name as animal_name, c.first_name, c.last_name
            FROM adoption_files ad
            JOIN animals a ON ad.id_animal = a.id_animal
            JOIN contacts c ON ad.id_person = c.id_person
            WHERE ad.id_adoption = @id;
        ";

        public const string UpdateStatus = @"
            UPDATE adoption_files SET status = @status
            WHERE id_adoption = @id;
            ";
    }
}
