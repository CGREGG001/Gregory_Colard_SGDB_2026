using AnimalShelter.Core.Interfaces;
using AnimalShelter.Core.Models;
using AnimalShelter.Core.Exceptions;
using AnimalShelter.Core.Enums;
using AnimalShelter.Core.Constants;
using AnimalShelter.BLL.Validators;
using Npgsql;

namespace AnimalShelter.BLL.Services;

public class VaccinationService : IVaccinationService
{
    private readonly IVaccinationRepository _vaccinationRepository;
    private readonly IAnimalRepository _animalRepository; // Nécessaire pour vérifier l'existence de l'animal

    public VaccinationService(IVaccinationRepository vaccinationRepository, IAnimalRepository animalRepository)
    {
        _vaccinationRepository = vaccinationRepository;
        _animalRepository = animalRepository;
    }

    public async Task<Guid> RegisterVaccinationAsync(Vaccination vaccination)
    {
        // 1. Vérifier si l'animal existe
        var animal = await _animalRepository.GetByIdAsync(vaccination.AnimalId);

        if (animal == null)
        {
            throw new ShelterException(ExceptionMessages.AnimalNotFound, ErrorTypeEnum.NotFound);
        }

        // 2. Validation métier
        VaccinationValidator.Validate(vaccination);

        try
        {
            return await _vaccinationRepository.AddAsync(vaccination);
        }
        catch (NpgsqlException ex)
        {
            throw new ShelterException(ExceptionMessages.DatabaseError, ErrorTypeEnum.DatabaseError, ex);
        }
    }

    public async Task<IEnumerable<Vaccination>> GetAnimalVaccinationHistoryAsync(string animalId)
    {
        // On pourrait vérifier si l'animal existe ici aussi pour être très corporate
        var animal = await _animalRepository.GetByIdAsync(animalId);
        if (animal == null) throw new ShelterException(ExceptionMessages.AnimalNotFound, ErrorTypeEnum.NotFound);

        return await _vaccinationRepository.GetByAnimalIdAsync(animalId);
    }
}
