using AnimalShelter.Core.Interfaces;
using AnimalShelter.Core.Models;
using AnimalShelter.Core.Exceptions;
using AnimalShelter.Core.Enums;
using AnimalShelter.ConsoleApp.UI.Utilities;

namespace AnimalShelter.ConsoleApp.UI;

public class ContactConsoleUI
{
    #region fields
    private readonly IContactService _contactService;
    #endregion

    #region constructors
    public ContactConsoleUI(IContactService contactService)
    {
        _contactService = contactService;
    }
    #endregion

    #region methods
    public async Task ShowMenuAsync()
    {
        bool exit = false;

        while (!exit)
        {
            Console.Clear();
            UIHelper.ShowHeader();
            UIHelper.ShowTitleMenu("Contact Management");

            Console.WriteLine(" 1. List all contacts");
            Console.WriteLine(" 2. Register new contact (with Address)");
            Console.WriteLine(" 3. View contact details");
            Console.WriteLine(" 4. Update contact");
            Console.WriteLine(" 5. Delete contact");
            Console.WriteLine(" 0. Back");

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("\nSelection > ");
            Console.ResetColor();

            string? choice = Console.ReadLine();

            switch(choice)
            {
                case "1": Console.Clear(); await ListContactsAsync(); break;
                case "2": Console.Clear(); await RegisterContactAsync(); break;
                case "3": Console.Clear(); await ViewDetailsAsync(); break;
                case "4": Console.Clear(); await UpdateContactAsync(); break;
                case "5": Console.Clear(); await DeleteContactAsync(); break;
                case "0": exit = true; break;
                default: UIHelper.Warning("Invalid choice"); break;
            }

            if (!exit)
            {
                UIHelper.Pause();
            }
        }
    }

    // ============================================================
    //  LIST CONTACTS
    // ============================================================
    private async Task ListContactsAsync()
    {
        Console.Clear();
        UIHelper.ShowTitle("Contact List");

        var contacts = await _contactService.GetAllContactsAsync();

        if (!contacts.Any())
        {
            UIHelper.Warning("No contacts found.");
            return;
        }

        var rows = contacts
            .Select(contact => new string[]
            {
                contact.Id.ToString(),
                contact.LastName,
                contact.FirstName,
                contact.RoleFlags.ToString()
            })
            .ToList();

        UIHelper.ShowTable(
            new[] { "ID", "Last Name", "First Name", "Roles" },
            rows
        );
    }       

    // ============================================================
    //  REGISTER CONTACT
    // ============================================================
    private async Task RegisterContactAsync()
    {
        Console.Clear();
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

            if (UIHelper.Confirm("Add an address"))
            {
                contact.Address = new Address
                {
                    Street = ConsoleHelper.GetRequiredString("Street"),
                    Number = ConsoleHelper.GetRequiredString("Number"),
                    Box = ConsoleHelper.GetString("Box (Optional)"),
                    PostCode = ConsoleHelper.GetRequiredString("Post Code"),
                    City = ConsoleHelper.GetRequiredString("City"),
                    Country = ConsoleHelper.GetString("Country (Default: Belgium)") ?? "Belgium"
                };
            };

            if (contact.RoleFlags.HasFlag(ContactRolesEnum.Adopter) || contact.RoleFlags.HasFlag(ContactRolesEnum.Volunteer))
            {
                nationalRegister = ConsoleHelper.GetRequiredString("National Register (11 digits required)");
            }

            Guid id = await _contactService.RegisterContactAsync(contact, nationalRegister);
            UIHelper.Success($"Contact successfully registered with ID: {id}");
        }
        catch (ShelterException ex)
        {
            UIHelper.Error(ex.Message);
        }
    }

    // ============================================================
    //  VIEW DETAILS
    // ============================================================
    private async Task ViewDetailsAsync()
    {
        Console.Clear();
        UIHelper.ShowTitle("Contact Details");

        Guid id = UIHelper.AskGuid("Enter Contact UUID");
        var contact = await _contactService.GetContactAsync(id);

        if (contact == null)
        {
            UIHelper.Warning("Contact not found.");
            return;
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\nName: {contact.FirstName} {contact.LastName}");
        Console.WriteLine($"Email: {contact.Email ?? "N/A"}");
        Console.WriteLine($"Roles: {contact.RoleFlags}");
        Console.WriteLine($"RGPD Consent: {contact.RgpdConsentDate?.ToShortDateString() ?? "No"}");
        Console.ResetColor();

        if (contact.Address != null)
        {
            UIHelper.ShowTitle("Address");
            Console.WriteLine($"{contact.Address.Street}, {contact.Address.Number} {contact.Address.Box}");
            Console.WriteLine($"{contact.Address.PostCode} {contact.Address.City} ({contact.Address.Country})");
        }
    }

    // ============================================================
    //  UPDATE CONTACT
    // ============================================================
    private async Task UpdateContactAsync()
    {
        Console.Clear();
        UIHelper.ShowTitle("Update Contact Details");

        Guid id = UIHelper.AskGuid("Enter Contact UUID to update");
        var contact = await _contactService.GetContactAsync(id);

        if (contact == null)
        {
            UIHelper.Warning("Contact not found.");
            return;
        }

        UIHelper.DrawBox($"Editing: {contact.FirstName} {contact.LastName}");
        Console.WriteLine("(Press Enter to keep the current value)\n");

        // Mise à jour des infos de base
        contact.FirstName = ConsoleHelper.GetStringWithDefault("First Name", contact.FirstName);
        contact.LastName = ConsoleHelper.GetStringWithDefault("Last Name", contact.LastName);
        contact.Email = ConsoleHelper.GetStringWithDefault("Email", contact.Email ?? "");
        contact.Gsm = ConsoleHelper.GetStringWithDefault("GSM", contact.Gsm ?? "");
        contact.Phone = ConsoleHelper.GetStringWithDefault("Phone", contact.Phone ?? "");

        // Mise à jour de l'adresse si elle existe
        if (contact.Address != null)
        {
            Console.WriteLine("\n--- Address Details ---");
            contact.Address.Street = ConsoleHelper.GetStringWithDefault("Street", contact.Address.Street);
            contact.Address.Number = ConsoleHelper.GetStringWithDefault("Number", contact.Address.Number);
            contact.Address.Box = ConsoleHelper.GetStringWithDefault("Box", contact.Address.Box ?? "");
            contact.Address.PostCode = ConsoleHelper.GetStringWithDefault("Post Code", contact.Address.PostCode);
            contact.Address.City = ConsoleHelper.GetStringWithDefault("City", contact.Address.City);
        }

        try
        {
            UIHelper.LoadingDots("Updating contact");
            await _contactService.UpdateContactAsync(contact);
            UIHelper.Success("Contact and address updated successfully.");
        }
        catch (Exception ex)
        {
            UIHelper.Error(ex.Message);
        }
    }

    // ============================================================
    //  DELETE CONTACT
    // ============================================================
    private async Task DeleteContactAsync()
    {
        Console.Clear();
        UIHelper.ShowTitle("Delete Contact");

        Guid id = UIHelper.AskGuid("Enter Contact UUID");

        if (!UIHelper.Confirm("Are you sure you want to delete this contact"))
        {
            UIHelper.Warning("Deletion cancelled.");
            return;
        }

        try
        {
            if (await _contactService.DeleteContactAsync(id))
                UIHelper.Success("Contact deleted (Soft Delete).");
        }
        catch (ShelterException ex)
        {
            UIHelper.Error(ex.Message);
        }
    }

    // ============================================================
    //  ROLE SELECTION (BITMASK)
    // ============================================================
    private ContactRolesEnum SelectRoles()
    {
        ContactRolesEnum selected = ContactRolesEnum.None;
        bool done = false;

        while (!done)
        {
            Console.Clear();
            UIHelper.ShowTitle("Select Roles");

            Console.WriteLine($"Current: {selected}");
            Console.WriteLine("----------------------------------");

            var roles = Enum.GetValues<ContactRolesEnum>()
                .Where(r => r != ContactRolesEnum.None)
                .ToList();

            for (int i = 0; i < roles.Count; i++)
            {
                string mark = selected.HasFlag(roles[i]) ? "[X]" : "[ ]";
                Console.WriteLine($"{i + 1}. {mark} {roles[i]}");
            }

            Console.WriteLine("0. DONE");

            Console.Write("\nToggle role: ");
            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                if (choice == 0) done = true;
                else if (choice > 0 && choice <= roles.Count)
                    selected ^= roles[choice - 1];
            }
        }

        return selected;
    }
    #endregion
}
