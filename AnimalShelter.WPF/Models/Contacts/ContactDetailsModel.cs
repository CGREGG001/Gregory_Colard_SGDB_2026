using AnimalShelter.Core.Enums;

namespace AnimalShelter.WPF.Models.Contacts
{
    public class ContactDetailsModel
    {
        public Guid Id { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Gsm { get; set; }
        public string? Phone { get; set; }
        public ContactRolesEnum RoleFlags { get; set; }
        public DateTime? RgpdConsentDate { get; set; }
        public bool IsAnonymised { get; set; }
        public string? Street { get; set; }
        public string? Number { get; set; }
        public string? Box { get; set; }
        public string? PostCode { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
    }
}
