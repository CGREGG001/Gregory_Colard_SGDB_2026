using AnimalShelter.BLL.Helpers;
using AnimalShelter.BLL.Validators;
using AnimalShelter.Core.Constants;
using AnimalShelter.Core.Enums;
using AnimalShelter.Core.Exceptions;
using AnimalShelter.Core.Interfaces;
using AnimalShelter.Core.Models;
using Npgsql;

namespace AnimalShelter.BLL.Services
{
    public class ContactService(IContactRepository contactRepository) : IContactService
    {
        private readonly IContactRepository _contactRepository = contactRepository;

        public async Task<Guid> RegisterContactAsync(Contact contact, string? clearNationalRegister = null)
        {
            // 1. Si un registre national est fourni en clair, on le chiffre
            if (!string.IsNullOrWhiteSpace(clearNationalRegister))
            {
                // On génère le Hash (pour l'unicité DB)
                contact.NationalRegisterHash = EncryptionHelper.Hash(clearNationalRegister);

                // On génère le Chiffré (pour la lecture sécurisée)
                contact.NationalRegisterEncrypted = EncryptionHelper.Encrypt(clearNationalRegister);
            }

            // 2. Validation
            ContactValidator.Validate(contact);

            try
            {
                return await _contactRepository.AddAsync(contact);
            }
            catch (NpgsqlException ex)
            {
                // Détection de la violation de contrainte UNIQUE
                if (ex.SqlState == "23505") // PostgreSQL unique_violation
                {
                    throw new ShelterException(ExceptionMessages.NationalRegisterAlreadyExists,
                        ErrorTypeEnum.ValidationError, ex);
                }
                throw new ShelterException(ExceptionMessages.DatabaseError, ErrorTypeEnum.DatabaseError, ex);
            }
        }

        public async Task<Contact?> GetContactAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ShelterException(ExceptionMessages.InvalidId, ErrorTypeEnum.ValidationError);
            }

            return await _contactRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Contact>> GetAllContactsAsync()
        {
            return await _contactRepository.GetAllAsync();
        }

        public async Task<bool> UpdateContactAsync(Contact contact)
        {
            if (contact.Id == Guid.Empty)
            {
                throw new ShelterException(ExceptionMessages.InvalidId, ErrorTypeEnum.ValidationError);
            }

            ContactValidator.Validate(contact);

            try
            {
                return await _contactRepository.UpdateAsync(contact);
            }
            catch (NpgsqlException ex)
            {
                throw new ShelterException(ExceptionMessages.DatabaseError, ErrorTypeEnum.DatabaseError, ex);
            }
        }

        public async Task<bool> DeleteContactAsync(Guid id)
        {
            Contact? existing = await _contactRepository.GetByIdAsync(id) ?? throw new ShelterException(ExceptionMessages.ContactNotFound, ErrorTypeEnum.NotFound);
            return await _contactRepository.DeleteAsync(id);
        }
    }
}
