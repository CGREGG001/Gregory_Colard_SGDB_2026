using AnimalShelter.Core.Constants;
using AnimalShelter.Core.Enums;
using AnimalShelter.Core.Exceptions;
using AnimalShelter.Core.Interfaces;
using AnimalShelter.Core.Models;

public class FosterService : IFosterService
{
    private readonly IFosterRepository _fosterRepo;
    private readonly IAnimalRepository _animalRepo;

    public FosterService(IFosterRepository fosterRepo, IAnimalRepository animalRepo)
    {
        _fosterRepo = fosterRepo;
        _animalRepo = animalRepo;
    }

    public async Task<Guid> StartFosterStayAsync(FosterStay stay)
    {
        var animal = await _animalRepo.GetByIdAsync(stay.AnimalId);
        
        if (animal is null)
        {
            throw new ShelterException(ExceptionMessages.AnimalNotFound, ErrorTypeEnum.NotFound);
        }


        if (animal.CurrentStatus == AnimalStatusEnum.Fostered)
        {
            throw new ShelterException(ExceptionMessages.AnimalAlreadyFostered, ErrorTypeEnum.Conflict);
        }

        Guid id = await _fosterRepo.AddAsync(stay);
        
        // Mise à jour automatique du statut de l'animal
        animal.CurrentStatus = AnimalStatusEnum.Fostered;
        await _animalRepo.UpdateAsync(animal);
        
        return id;
    }

    public async Task<bool> EndFosterStayAsync(Guid stayId, DateTime endDate)
    {
        // Logique : Mettre fin au séjour et repasser l'animal en statut 'Shelter'
        // (Pour simplifier ici, on suppose qu'on récupère l'animalId via le stayId d'abord)
        return await _fosterRepo.EndStayAsync(stayId, endDate);
    }

    public async Task<IEnumerable<FosterStay>> GetAnimalHistoryAsync(string animalId)
    {
        return await _fosterRepo.GetStaysByAnimalIdAsync(animalId);    
    }

    public async Task<IEnumerable<FosterStay>> GetFamilyCurrentAnimalsAsync(Guid contactId)
    {
        return await _fosterRepo.GetStaysByContactIdAsync(contactId);
    }
}
