using AnimalShelter.Core.Models;

namespace AnimalShelter.Core.Interfaces
{
    public interface IAnimalRepository
    {
        // Retourne l'ID généré par la DB (yymmdd...)
        Task<string> AddAsync(Animal animal);

        Task<Animal?> GetByIdAsync(string id);

        // Liste uniquement les animaux non supprimés (Soft Delete)
        Task<IEnumerable<Animal>> GetAllActiveAsync();

        Task<bool> UpdateAsync(Animal animal);

        // Soft Delete
        Task<bool> DeleteAsync(string id);
    }
}
