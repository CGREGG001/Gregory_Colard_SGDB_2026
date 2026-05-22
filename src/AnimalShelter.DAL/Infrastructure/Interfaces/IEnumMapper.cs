using Npgsql;

namespace AnimalShelter.DAL.Infrastructure.Interfaces
{
    public interface IEnumMapper
    {
        void MapEnums(NpgsqlDataSourceBuilder builder);
    }
}
