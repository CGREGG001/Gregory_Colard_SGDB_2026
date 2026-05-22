using AnimalShelter.Core.Models;

namespace AnimalShelter.Core.Interfaces
{
    public interface IAnimalService
    {
        Task<string> RegisterAnimalAsync(Animal animal);
        Task<Animal?> GetAnimalAsync(string id);
        Task<IEnumerable<Animal>> GetAvailableAnimalsAsync();
        Task<bool> UpdateAnimalAsync(Animal animal);
        Task<bool> SoftDeleteAnimalAsync(string id);
    }
}
