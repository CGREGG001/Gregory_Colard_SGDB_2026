using AnimalShelter.Core.Interfaces;
using AnimalShelter.Core.Models;
using AnimalShelter.Core.Enums;
using AnimalShelter.Core.Exceptions;
using AnimalShelter.BLL.Validators;

namespace AnimalShelter.BLL.Services;

public class AdoptionService : IAdoptionService
{
    private readonly IAdoptionRepository _adoptionRepo;
    private readonly IAnimalRepository _animalRepo;

    public AdoptionService(IAdoptionRepository adoptionRepo, IAnimalRepository animalRepo)
    {
        _adoptionRepo = adoptionRepo;
        _animalRepo = animalRepo;
    }

    public async Task<Guid> RequestAdoptionAsync(AdoptionFile file)
    {
        var animal = await _animalRepo.GetByIdAsync(file.AnimalId) ?? throw new ShelterException("Animal not found", ErrorTypeEnum.NotFound);
        
        if (animal.CurrentStatus == AnimalStatusEnum.Adopted || animal.CurrentStatus == AnimalStatusEnum.Dead)
            throw new ShelterException("This animal is not available for adoption.", ErrorTypeEnum.Conflict);

        AdoptionValidator.Validate(file);
        file.Status = AdoptionStatusEnum.Requested;
        return await _adoptionRepo.AddAsync(file);
    }

    public async Task<IEnumerable<AdoptionFile>> GetAllAdoptionsAsync() => await _adoptionRepo.GetAllAsync();

    public async Task<bool> ProcessAdoptionAsync(Guid id, AdoptionStatusEnum newStatus)
    {
        var file = await _adoptionRepo.GetByIdAsync(id) ?? throw new ShelterException("Adoption file not found", ErrorTypeEnum.NotFound);
        
        bool success = await _adoptionRepo.UpdateStatusAsync(id, newStatus);

        if (success && newStatus == AdoptionStatusEnum.Approved)
        {
            var animal = await _animalRepo.GetByIdAsync(file.AnimalId);
            if (animal != null)
            {
                animal.CurrentStatus = AnimalStatusEnum.Adopted;
                await _animalRepo.UpdateAsync(animal);
            }
        }
        return success;
    }
}
