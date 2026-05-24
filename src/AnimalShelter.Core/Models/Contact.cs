using AnimalShelter.Core.Enums;

namespace AnimalShelter.Core.Models
{
    public class Contact : BaseEntity<Guid>
    {
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;

        // Stocké en binaire car chiffré dans la BLL
        public byte[]? NationalRegisterEncrypted { get; set; }
        // Stocké en binaire également
        public byte[]? NationalRegisterHash { get; set; }

        public string? Gsm { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }

        // Utilise l'enum [Flags] pour stocker le bitmask (1, 2, 4, 8...)
        public ContactRolesEnum RoleFlags { get; set; }

        public DateTime? RgpdConsentDate { get; set; }
        public bool IsAnonymised { get; set; }

        // FK et propriété de navigation
        public Guid? AddressId { get; set; }
        public Address? Address { get; set; }
    }
}
