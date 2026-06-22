using AnimalShelter.Core.Models;
using AnimalShelter.Core.Enums;

namespace AnimalShelter.Core.Interfaces;

public interface IAdoptionRepository
{
    Task<Guid> AddAsync(AdoptionFile file);
    Task<IEnumerable<AdoptionFile>> GetAllAsync();
    Task<AdoptionFile?> GetByIdAsync(Guid id);
    Task<bool> UpdateStatusAsync(Guid id, AdoptionStatusEnum status);

    Task<IEnumerable<AdoptionFile>> GetByAnimalIdAsync(string animalId);
    Task<IEnumerable<AdoptionFile>> GetByContactIdAsync(Guid contactId);
}
