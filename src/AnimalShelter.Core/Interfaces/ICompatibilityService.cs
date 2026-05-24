using AnimalShelter.Core.Models;
using AnimalShelter.Core.Enums;

namespace AnimalShelter.Core.Interfaces;

public interface ICompatibilityService
{
    Task SetCompatibilityAsync(Compatibility compatibility);
    Task<IEnumerable<Compatibility>> GetAnimalCompatibilitiesAsync(string animalId);
    Task DeleteCompatibilityAsync(string animalId, CompatibilityTypeEnum type);
    // Pour la ligne du PDF : "Ajouter une information sur un animal (description, particularité)"
    Task UpdateAnimalNotesAsync(string animalId, string description, string particularities);
}
