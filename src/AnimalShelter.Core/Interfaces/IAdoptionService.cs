using AnimalShelter.Core.Models;
using AnimalShelter.Core.Enums;

namespace AnimalShelter.Core.Interfaces;

public interface IAdoptionService
{
    Task<Guid> RequestAdoptionAsync(AdoptionFile file);
    Task<IEnumerable<AdoptionFile>> GetAllAdoptionsAsync();
    Task<bool> ProcessAdoptionAsync(Guid id, AdoptionStatusEnum newStatus);
}
