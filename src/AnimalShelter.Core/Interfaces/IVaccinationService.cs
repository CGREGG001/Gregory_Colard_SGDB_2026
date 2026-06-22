using AnimalShelter.Core.Models;

namespace AnimalShelter.Core.Interfaces;

public interface IVaccinationService
{
    Task<Guid> RegisterVaccinationAsync(Vaccination vaccination);
    Task<IEnumerable<Vaccination>> GetAnimalVaccinationHistoryAsync(string animalId);
    Task<bool> UpdateVaccinationAsync(Vaccination vaccination);
    Task<bool> DeleteVaccinationAsync(Guid id);
}
