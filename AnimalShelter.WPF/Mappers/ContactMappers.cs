using AnimalShelter.Core.Models;
using AnimalShelter.WPF.Models.Contacts;

namespace AnimalShelter.WPF.Mappers
{
    public static class ContactMappers
    {
        public static ContactListingModel ToListingModel(this Contact c) => new()
        {
            Id = c.Id,
            FullName = $"{c.FirstName} {c.LastName}",
            Email = c.Email,
            Gsm = c.Gsm,
            RoleFlags = c.RoleFlags,
        };

        public static ContactDetailsModel ToDetailsModel(this Contact c) => new()
        {
            Id = c.Id,
            FirstName = c.FirstName,
            LastName = c.LastName,
            Email = c.Email,
            Gsm = c.Gsm,
            Phone = c.Phone,
            RoleFlags = c.RoleFlags,
            RgpdConsentDate = c.RgpdConsentDate,
            IsAnonymised = c.IsAnonymised,
            Street = c.Address?.Street,
            Number = c.Address?.Number,
            Box = c.Address?.Box,
            PostCode = c.Address?.PostCode,
            City = c.Address?.City,
            Country = c.Address?.Country,
        };
    }
}
