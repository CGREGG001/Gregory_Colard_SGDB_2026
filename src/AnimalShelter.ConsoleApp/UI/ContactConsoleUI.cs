using AnimalShelter.Core.Interfaces;
using AnimalShelter.Core.Models;
using AnimalShelter.Core.Exceptions;
using AnimalShelter.Core.Enums;
using AnimalShelter.ConsoleApp.UI.Utilities;

namespace AnimalShelter.ConsoleApp.UI;

public class ContactConsoleUI
{
    private readonly IContactService _contactService;

    public ContactConsoleUI(IContactService contactService)
    {
        _contactService = contactService;
    }

    public async Task ShowMenuAsync()
    {
        bool exit = false;
        while (!exit)
        {
            Console.Clear();
            UIHelper.ShowTitleMenu("Contact Management");

            Console.WriteLine(" 1. List all contacts");
            Console.WriteLine(" 2. Register new contact (with Address)");
            Console.WriteLine(" 3. View contact details");
            Console.WriteLine(" 4. Delete contact (Soft Delete)");
            Console.WriteLine(" 0. Back to main menu");

            Console.Write("\nSelect an option: ");
            switch (Console.ReadLine())
            {
                case "1": Console.Clear(); await ListContactsAsync(); break;
                case "2": Console.Clear(); await RegisterContactAsync(); break;
                case "3": Console.Clear(); await ViewDetailsAsync(); break;
                case "4": Console.Clear(); await DeleteContactAsync(); break;
                case "0": exit = true; break;
                default: UIHelper.Warning("Invalid choice"); break;
            }

            if (!exit)
            {
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }
    }

    private async Task ListContactsAsync()
    {
        UIHelper.DrawBox("Contact List");
        var contacts = await _contactService.GetAllContactsAsync();

        if (!contacts.Any())
        {
            UIHelper.Warning("No contacts found.");
            return;
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("{0,-38} {1,-20} {2,-20} {3,-15}", "ID", "Last Name", "First Name", "Roles");
        Console.ResetColor();
        Console.WriteLine(new string('-', 95));

        foreach (var c in contacts)
        {
            Console.WriteLine("{0,-38} {1,-20} {2,-20} {3,-15}",
                c.Id, c.LastName, c.FirstName, c.RoleFlags);
        }
    }

    private async Task RegisterContactAsync()
    {
        UIHelper.DrawBox("Register New Contact");
        try
        {
            var contact = new Contact
            {
                FirstName = ConsoleHelper.GetRequiredString("First Name"),
                LastName = ConsoleHelper.GetRequiredString("Last Name"),
                Email = ConsoleHelper.GetString("Email (Optional)"),
                Gsm = ConsoleHelper.GetString("GSM (Optional)"),
                Phone = ConsoleHelper.GetString("Phone (Optional)"),
                RoleFlags = SelectRoles(), // Gestion spécifique du bitmask
                RgpdConsentDate = DateTime.Now
            };

            // Saisie du Registre National (sera validé/chiffré par la BLL)
            string? nationalRegister = null;
            if (contact.RoleFlags.HasFlag(ContactRolesEnum.Adopter) || contact.RoleFlags.HasFlag(ContactRolesEnum.Volunteer))
            {
                nationalRegister = ConsoleHelper.GetRequiredString("National Register (11 digits required)");
            }

            // Gestion de l'adresse (Facultative)
            if (ConsoleHelper.GetBool("Do you want to add an address"))
            {
                contact.Address = new Address
                {
                    Street = ConsoleHelper.GetRequiredString("Street"),
                    Number = ConsoleHelper.GetRequiredString("Number"),
                    Box = ConsoleHelper.GetString("Box (Optional)"),
                    PostCode = ConsoleHelper.GetRequiredString("Post Code"),
                    City = ConsoleHelper.GetRequiredString("City"),
                    Country = ConsoleHelper.GetString("Country (Default: Belgium)")
                };
                if (string.IsNullOrWhiteSpace(contact.Address.Country)) contact.Address.Country = "Belgium";
            }

            Guid id = await _contactService.RegisterContactAsync(contact, nationalRegister);
            UIHelper.Success($"Contact successfully registered with ID: {id}");
        }
        catch (ShelterException ex) { DisplayError(ex); }
    }

    private async Task ViewDetailsAsync()
    {
        UIHelper.DrawBox("Contact Details");
        string input = ConsoleHelper.GetRequiredString("Enter Contact UUID");

        if (!Guid.TryParse(input, out Guid id))
        {
            UIHelper.Error("Invalid UUID format.");
            return;
        }

        var contact = await _contactService.GetContactAsync(id);
        if (contact == null) { UIHelper.Warning("Contact not found."); return; }

        Console.WriteLine($"\nName: {contact.FirstName} {contact.LastName}");
        Console.WriteLine($"Email: {contact.Email ?? "N/A"}");
        Console.WriteLine($"Roles: {contact.RoleFlags}");
        Console.WriteLine($"RGPD Consent: {contact.RgpdConsentDate?.ToShortDateString() ?? "No"}");

        if (contact.Address != null)
        {
            UIHelper.DrawBox("Address");
            Console.WriteLine($"{contact.Address.Street}, {contact.Address.Number} {contact.Address.Box}");
            Console.WriteLine($"{contact.Address.PostCode} {contact.Address.City} ({contact.Address.Country})");
        }
    }

    private async Task DeleteContactAsync()
    {
        UIHelper.DrawBox("Delete Contact");
        string input = ConsoleHelper.GetRequiredString("Enter Contact UUID to delete");

        if (Guid.TryParse(input, out Guid id))
        {
            if (ConsoleHelper.GetBool("Are you sure you want to delete this contact"))
            {
                try
                {
                    if (await _contactService.DeleteContactAsync(id))
                        UIHelper.Success("Contact deleted (Soft Delete).");
                }
                catch (ShelterException ex) { DisplayError(ex); }
            }
        }
        else { UIHelper.Error("Invalid UUID."); }
    }

    // ---------------------------------------------------------
    // HELPER METHODS
    // ---------------------------------------------------------

    private ContactRolesEnum SelectRoles()
    {
        ContactRolesEnum selectedRoles = ContactRolesEnum.None;
        bool done = false;

        while (!done)
        {
            Console.Clear();
            UIHelper.DrawBox("Select Roles (Multiple possible)");
            Console.WriteLine($"Current selection: {selectedRoles}");
            Console.WriteLine("----------------------------------");

            var roles = Enum.GetValues<ContactRolesEnum>().Where(r => r != ContactRolesEnum.None).ToList();
            for (int i = 0; i < roles.Count; i++)
            {
                string status = selectedRoles.HasFlag(roles[i]) ? "[X]" : "[ ]";
                Console.WriteLine($"{i + 1}. {status} {roles[i]}");
            }
            Console.WriteLine("0. DONE - Finish selection");

            Console.Write("\nToggle role (number) or 0 to finish: ");
            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                if (choice == 0) done = true;
                else if (choice > 0 && choice <= roles.Count)
                {
                    // Toggle le bit correspondant via l'opérateur XOR (^)
                    selectedRoles ^= roles[choice - 1];
                }
            }
        }
        return selectedRoles;
    }

    private void DisplayError(ShelterException ex)
    {
        UIHelper.Error($"[Type: {ex.ErrorType}]\nMessage: {ex.Message}");
    }
}