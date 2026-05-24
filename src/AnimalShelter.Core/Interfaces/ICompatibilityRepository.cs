using AnimalShelter.Core.Models;
using AnimalShelter.Core.Enums;

namespace AnimalShelter.Core.Interfaces;

public interface ICompatibilityRepository
{
    Task SaveAsync(Compatibility compatibility);
    Task<IEnumerable<Compatibility>> GetByAnimalIdAsync(string animalId);
    Task<bool> DeleteAsync(string animalId, CompatibilityTypeEnum type);
}
