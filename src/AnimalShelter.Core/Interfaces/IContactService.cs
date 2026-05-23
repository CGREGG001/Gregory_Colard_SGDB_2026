using AnimalShelter.Core.Models;

namespace AnimalShelter.Core.Interfaces
{
    public interface IContactService
    {
        Task<Guid> RegisterContactAsync(Contact contact, string? clearNationalRegister = null);
        Task<Contact?> GetContactAsync(Guid id);
        Task<IEnumerable<Contact>> GetAllContactsAsync();
        Task<bool> UpdateContactAsync(Contact contact);
        Task<bool> DeleteContactAsync(Guid id);
    }
}
