using AnimalShelter.Core.Models;

namespace AnimalShelter.Core.Interfaces;

public interface IFosterService
{
    Task<Guid> StartFosterStayAsync(FosterStay stay);
    Task<bool> EndFosterStayAsync(Guid stayId, DateTime endDate);
    Task<IEnumerable<FosterStay>> GetAnimalHistoryAsync(string animalId);
    Task<IEnumerable<FosterStay>> GetFamilyCurrentAnimalsAsync(Guid contactId);
}
