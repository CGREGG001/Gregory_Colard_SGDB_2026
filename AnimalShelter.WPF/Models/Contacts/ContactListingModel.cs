using AnimalShelter.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnimalShelter.WPF.Models.Contacts
{
    public class ContactListingModel
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Gsm { get; set; }
        public ContactRolesEnum RoleFlags { get; set; }
    }
}
