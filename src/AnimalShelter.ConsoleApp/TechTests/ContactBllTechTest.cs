using AnimalShelter.BLL.Helpers;
using AnimalShelter.BLL.Services;
using AnimalShelter.ConsoleApp.UI.Utilities;
using AnimalShelter.Core.Enums;
using AnimalShelter.Core.Exceptions;
using AnimalShelter.Core.Models;
using AnimalShelter.DAL.Infrastructure;
using AnimalShelter.DAL.Infrastructure.Enums;
using AnimalShelter.DAL.Repositories;

namespace AnimalShelter.ConsoleApp.TechTests
{
    public class ContactBllTechTest
    {
        public static async Task RunAsync()
        {
            UIHelper.ShowTitleMenu("Contact BLL Tech Test");

            // 1. Setup
            var enumMapper = new EnumMapper();
            var dbFactory = new DbConnectionFactory(enumMapper);
            var repo = new ContactRepository(dbFactory);
            var service = new ContactService(repo);

            try
            {
                // --- TEST 1 : Validation Email Invalide ---
                Console.WriteLine("\n[Test 1] Registering with invalid email...");
                try
                {
                    var c = new Contact
                    {
                        FirstName = "A",
                        LastName = "B",
                        Email = "invalid-email",
                        RoleFlags = ContactRolesEnum.Other
                    };

                    await service.RegisterContactAsync(c);
                }
                catch (ShelterException ex)
                {
                    Console.WriteLine($"Expected Error: {ex.Message}");
                }

                // --- TEST 2 : Registre National Invalide (Mauvais Checksum) ---
                Console.WriteLine("\n[Test 2] Registering with invalid National Register (Bad Checksum)...");
                try
                {
                    var c = new Contact
                    {
                        FirstName = "John",
                        LastName = "Doe",
                        RoleFlags = ContactRolesEnum.Volunteer
                    };

                    await service.RegisterContactAsync(c, "12345678901"); // Mauvais checksum
                }
                catch (ShelterException ex)
                {
                    Console.WriteLine($"Expected Error: {ex.Message}");
                }

                // --- TEST 3 : Succès avec Chiffrement et Validation RN ---
                string validRN = "80072517726";
                Console.WriteLine($"\n[Test 3] Registering valid contact with RN: {validRN}...");

                var validContact = new Contact
                {
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "jane.smith@shelter.be",
                    RoleFlags = ContactRolesEnum.Volunteer | ContactRolesEnum.Adopter,
                    Address = new Address
                    {
                        Street = "Main St",
                        Number = "1",
                        PostCode = "1000",
                        City = "Brussels"
                    }
                };

                Guid newId = await service.RegisterContactAsync(validContact, validRN);
                UIHelper.Success($"Contact registered! ID: {newId}");

                // --- TEST 4 : Vérification du déchiffrement ---
                Console.WriteLine("\n[Test 4] Verifying decryption of stored National Register...");
                var fetched = await service.GetContactAsync(newId);

                if (fetched?.NationalRegisterEncrypted != null)
                {
                    string decrypted = EncryptionHelper.Decrypt(fetched.NationalRegisterEncrypted);
                    Console.WriteLine($"Decrypted RN: {decrypted}");

                    if (decrypted == validRN)
                        UIHelper.Success("Decryption matches original value!");
                    else
                        UIHelper.Error("Decryption mismatch!");
                }
            }
            catch (Exception ex)
            {
                UIHelper.Error($"{ex.GetType().Name}: {ex.Message}");
            }

            Console.WriteLine("\n=== End of Contact BLL Tech Test ===");
        }
    }
}
