using AnimalShelter.Core.Models;

namespace AnimalShelter.Core.Interfaces;

public interface IFosterRepository
{
    Task<Guid> AddAsync(FosterStay stay);
    Task<IEnumerable<FosterStay>> GetStaysByAnimalIdAsync(string animalId);
    Task<IEnumerable<FosterStay>> GetStaysByContactIdAsync(Guid contactId);
    Task<bool> EndStayAsync(Guid stayId, DateTime endDate);
    Task<IEnumerable<FosterStay>> GetHistoryByContactIdAsync(Guid contactId);
}
