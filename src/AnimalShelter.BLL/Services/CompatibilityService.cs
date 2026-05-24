using AnimalShelter.Core.Interfaces;
using AnimalShelter.Core.Models;
using AnimalShelter.Core.Enums;
using AnimalShelter.Core.Exceptions;
using AnimalShelter.Core.Constants;
using Npgsql;

namespace AnimalShelter.BLL.Services;

public class CompatibilityService : ICompatibilityService
{
    private readonly ICompatibilityRepository _compRepo;
    private readonly IAnimalRepository _animalRepo;

    public CompatibilityService(ICompatibilityRepository compRepo, IAnimalRepository animalRepo)
    {
        _compRepo = compRepo;
        _animalRepo = animalRepo;
    }

    public async Task SetCompatibilityAsync(Compatibility c)
    {
        var animal = await _animalRepo.GetByIdAsync(c.AnimalId) ?? throw new ShelterException(ExceptionMessages.AnimalNotFound, ErrorTypeEnum.NotFound);
        try { await _compRepo.SaveAsync(c); }
        catch (NpgsqlException ex) { throw new ShelterException(ExceptionMessages.DatabaseError, ErrorTypeEnum.DatabaseError, ex); }
    }

    public async Task<IEnumerable<Compatibility>> GetAnimalCompatibilitiesAsync(string animalId)
    {
        return await _compRepo.GetByAnimalIdAsync(animalId);
    }

    public async Task DeleteCompatibilityAsync(string animalId, CompatibilityTypeEnum type)
    {
        await _compRepo.DeleteAsync(animalId, type);
    }

    public async Task UpdateAnimalNotesAsync(string animalId, string description, string particularities)
    {
        var animal = await _animalRepo.GetByIdAsync(animalId) ?? throw new ShelterException(ExceptionMessages.AnimalNotFound, ErrorTypeEnum.NotFound);
        animal.Description = description;
        animal.Particularities = particularities;
        await _animalRepo.UpdateAsync(animal);
    }
}
