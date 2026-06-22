namespace AnimalShelter.DAL.Queries
{
    public class AddressQueries
    {
        public const string GetById = "SELECT * FROM sp_address_get_by_id(@id)";
    }
}
