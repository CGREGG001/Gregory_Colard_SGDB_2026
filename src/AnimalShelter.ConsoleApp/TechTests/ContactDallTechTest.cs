using AnimalShelter.ConsoleApp.UI.Utilities;
using AnimalShelter.Core.Enums;
using AnimalShelter.Core.Models;
using AnimalShelter.DAL.Infrastructure;
using AnimalShelter.DAL.Infrastructure.Enums;
using AnimalShelter.DAL.Repositories;

namespace AnimalShelter.ConsoleApp.TechTests
{
    public class ContactDallTechTest
    {
        public static async Task RunAsync()
        {
            UIHelper.ShowTitleMenu("Contact DAL Tech Test");

            try
            {
                // 1. Setup
                var enumMapper = new EnumMapper();
                var dbFactory = new DbConnectionFactory(enumMapper);
                var contactRepo = new ContactRepository(dbFactory);

                // 2. Création d'un contact avec une adresse (Test de la Transaction)
                Console.WriteLine("\n[1] Creating a contact with a new address...");

                var newContact = new Contact
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com",
                    Gsm = "0470123456",
                    // Test du Bitmask : Volunteer (1) + Adopter (2) = 3
                    RoleFlags = ContactRolesEnum.Volunteer | ContactRolesEnum.Adopter,
                    RgpdConsentDate = DateTime.Now,
                    Address = new Address
                    {
                        Street = "Rue de la Paix",
                        Number = "42",
                        PostCode = "1000",
                        City = "Brussels",
                        Country = "Belgium"
                    }
                };

                Guid contactId = await contactRepo.AddAsync(newContact);
                UIHelper.Success($"Contact created successfully! ID: {contactId}");

                // 3. Test de la Jointure (GetById avec Address)
                Console.WriteLine($"\n[2] Fetching contact {contactId} with its address...");
                var fetched = await contactRepo.GetByIdAsync(contactId);

                if (fetched != null)
                {
                    Console.WriteLine($"Name: {fetched.FirstName} {fetched.LastName}");
                    Console.WriteLine($"Roles: {fetched.RoleFlags}");
                    // Devrait afficher "Volunteer, Adopter"

                    if (fetched.Address != null)
                    {
                        Console.WriteLine($"Address: {fetched.Address.Number} {fetched.Address.Street}, {fetched.Address.City}");
                    }
                    else
                    {
                        UIHelper.Error("Address was not loaded! Check your JOIN in ContactQueries.");
                    }
                }

                // 4. Test du List (GetAll)
                Console.WriteLine("\n[3] Listing all contacts...");
                var allContacts = await contactRepo.GetAllAsync();
                foreach (var c in allContacts)
                {
                    Console.WriteLine($"- {c.LastName} {c.FirstName} ({c.RoleFlags})");
                }

            }
            catch (Exception ex)
            {
                UIHelper.Error($"{ex.GetType().Name}: {ex.Message}");
                if (ex.InnerException != null) Console.WriteLine($"Inner: {ex.InnerException.Message}");
            }

            Console.WriteLine("\n=== End of Contact Tech Test ===");
        }
    }
}
