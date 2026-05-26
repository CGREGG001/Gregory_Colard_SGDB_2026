using AnimalShelter.BLL.Validators;
using AnimalShelter.Core.Constants;
using AnimalShelter.Core.Enums;
using AnimalShelter.Core.Exceptions;
using AnimalShelter.Core.Interfaces;
using AnimalShelter.Core.Models;
using Npgsql; // On ne l'utilise QUE pour le catch !

namespace AnimalShelter.BLL.Services;

public class AnimalService(IAnimalRepository animalRepository) : IAnimalService
{
    private readonly IAnimalRepository _animalRepository = animalRepository;

    public async Task<string> RegisterAnimalAsync(Animal animal)
    {
        // 1. Validation des datas
        AnimalValidator.Validate(animal);

        // 2. Vérification de doublon métier (Nom + Espèce + Date Naissance)
        IEnumerable<Animal> existingAnimals = await _animalRepository.GetAllActiveAsync();

        if (existingAnimals.Any(a => a.Name.Equals(animal.Name, StringComparison.OrdinalIgnoreCase)
                                  && a.Species == animal.Species
                                  && a.BirthDate == animal.BirthDate))
        {
            throw new ShelterException(ExceptionMessages.AnimalAlreadyExists, ErrorTypeEnum.Conflict);
        }

        try
        {
            // 3. Appel DAL
            return await _animalRepository.AddAsync(animal);
        }
        catch (NpgsqlException ex)
        {
            // 4. Middleware Exception : On transforme l'erreur SQL en erreur métier
            // On garde l'exception d'origine (ex) en "InnerException" pour le debug
            throw new ShelterException(ExceptionMessages.DatabaseError, ErrorTypeEnum.DatabaseError, ex);
        }
    }

    public Task<Animal?> GetAnimalAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ShelterException("Invalid Animal ID.", ErrorTypeEnum.ValidationError);
        }

        return _animalRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Animal>> GetAvailableAnimalsAsync()
    {
        return await _animalRepository.GetAllActiveAsync();
    }

    public async Task<bool> UpdateAnimalAsync(Animal animal)
    {
        // 0. Validation de l'ID
        if (string.IsNullOrWhiteSpace(animal.Id))
        {
            throw new ShelterException(ExceptionMessages.InvalidId, ErrorTypeEnum.ValidationError);
        }

        // 1. Validation des nouvelles données AVANT d'aller en DB
        AnimalValidator.Validate(animal);

        // 2. Vérifier si l'animal existe
        Animal current = await _animalRepository.GetByIdAsync(animal.Id)
            ?? throw new ShelterException(ExceptionMessages.AnimalNotFound, ErrorTypeEnum.NotFound);

        // 3. Règle métier : on ne modifie pas un animal décédé
        if (current.CurrentStatus == AnimalStatusEnum.Dead && animal.CurrentStatus != AnimalStatusEnum.Dead)
        {
            throw new ShelterException(ExceptionMessages.CannotModifyDeadAnimal, ErrorTypeEnum.ValidationError);
        }

        // 4. Règle métier : pas de régression de dates
        if (current.SterilisationDate.HasValue &&
            animal.SterilisationDate < current.SterilisationDate)
        {
            throw new ShelterException(ExceptionMessages.InvalidDateRegression, ErrorTypeEnum.ValidationError);
        }

        if (current.DeathDate.HasValue &&
            animal.DeathDate < current.DeathDate)
        {
            throw new ShelterException(ExceptionMessages.InvalidDateRegression, ErrorTypeEnum.ValidationError);
        }

        try
        {
            return await _animalRepository.UpdateAsync(animal);
        }
        catch (NpgsqlException ex)
        {
            throw new ShelterException(ExceptionMessages.DatabaseError, ErrorTypeEnum.DatabaseError, ex);
        }
    }

    public async Task<bool> SoftDeleteAnimalAsync(string id)
    {
        Animal? exists = await _animalRepository.GetByIdAsync(id) ?? throw new ShelterException(ExceptionMessages.AnimalNotFound, ErrorTypeEnum.NotFound);
        return await _animalRepository.DeleteAsync(id);
    }
}
