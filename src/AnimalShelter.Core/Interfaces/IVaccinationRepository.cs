using AnimalShelter.Core.Models;

namespace AnimalShelter.Core.Interfaces;

public interface IVaccinationRepository
{
    Task<Guid> AddAsync(Vaccination vaccination);
    Task<IEnumerable<Vaccination>> GetByAnimalIdAsync(string animalId);
    Task<bool> UpdateAsync(Vaccination vaccination);
    Task<bool> DeleteAsync(Guid id);
}
