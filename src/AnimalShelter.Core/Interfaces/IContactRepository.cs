using AnimalShelter.Core.Models;

namespace AnimalShelter.Core.Interfaces
{
    public interface IContactRepository
    {
        Task<Guid> AddAsync(Contact contact);

        Task<Contact?> GetByIdAsync(Guid id);

        Task<IEnumerable<Contact>> GetAllAsync();

        Task<bool> UpdateAsync(Contact contact);

        Task<bool> DeleteAsync(Guid id);
    }
}
