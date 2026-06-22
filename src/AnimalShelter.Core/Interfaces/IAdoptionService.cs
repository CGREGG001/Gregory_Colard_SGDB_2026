using AnimalShelter.Core.Models;
using AnimalShelter.Core.Enums;

namespace AnimalShelter.Core.Interfaces;

public interface IAdoptionService
{
    Task<Guid> RequestAdoptionAsync(AdoptionFile file);
    Task<IEnumerable<AdoptionFile>> GetAllAdoptionsAsync();
    Task<bool> ProcessAdoptionAsync(Guid id, AdoptionStatusEnum newStatus);

    Task<IEnumerable<AdoptionFile>> GetAnimalAdoptionsAsync(string animalId);
    Task<IEnumerable<AdoptionFile>> GetContactAdoptionsAsync(Guid contactId);
}
