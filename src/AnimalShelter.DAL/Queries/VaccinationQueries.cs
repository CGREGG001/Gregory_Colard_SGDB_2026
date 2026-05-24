namespace AnimalShelter.DAL.Queries
{
    public static class VaccinationQueries
    {
        public const string Insert = @"
            INSERT INTO vaccinations (id_animal, vaccine_name, vaccine_date, is_done)
            VALUES (@id_animal, @name, @date, @is_done);
            ";

        public const string GetByAnimal = @"
            SELECT *
            FROM vaccinations
            WHERE id_animal = @id_animal AND deleted_at IS NULL;
            ";
    }
}
