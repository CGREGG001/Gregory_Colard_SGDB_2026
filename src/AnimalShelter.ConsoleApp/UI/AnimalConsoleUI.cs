using AnimalShelter.Core.Interfaces;
using AnimalShelter.Core.Models;
using AnimalShelter.Core.Exceptions;
using AnimalShelter.Core.Enums;
using AnimalShelter.ConsoleApp.UI.Utilities;

namespace AnimalShelter.ConsoleApp.UI;

public class AnimalConsoleUI
{
    private readonly IAnimalService _animalService;
    private readonly IVaccinationService _vaccinationService;
    private readonly ICompatibilityService _compatService;

    public AnimalConsoleUI(
        IAnimalService animalService,
        IVaccinationService vaccinationService,
        ICompatibilityService compatService)
    {
        _animalService = animalService;
        _vaccinationService = vaccinationService;
        _compatService = compatService;
    }

    public async Task ShowMenuAsync()
    {
        bool exit = false;
        while (!exit)
        {
            Console.Clear();
            UIHelper.ShowTitleMenu("Animal Management");

            Console.WriteLine(" 1. List all active animals");
            Console.WriteLine(" 2. Register new animal");
            Console.WriteLine(" 3. View animal details");
            Console.WriteLine(" 4. Delete animal");
            Console.WriteLine(" 5. Vaccination Management");
            Console.WriteLine(" 6. Manage Info & Compatibility");
            Console.WriteLine(" 0. Back to main menu");

            Console.Write("\nSelect an option: ");
            switch (Console.ReadLine())
            {
                case "1": await ListAnimalsAsync(); break;
                case "2": await RegisterAnimalAsync(); break;
                case "3": await ViewDetailsAsync(); break;
                case "4": await DeleteAnimalAsync(); break;
                case "5": await ShowVaccinationMenuAsync(); break; // Vers le sous-menu
                case "6": await ShowCompatibilityMenuAsync(); break; // Vers le sous-menu
                case "0": exit = true; break;
                default: UIHelper.Warning("Invalid choice"); break;
            }

            if (!exit && Console.CursorLeft != 0) // Évite d'attendre si on vient du sous-menu
            {
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }
    }

    // --- SOUS-MENU VACCINATION ---
    private async Task ShowVaccinationMenuAsync()
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            UIHelper.ShowTitleMenu("Vaccination Management");
            Console.WriteLine(" 1. View animal vaccination history");
            Console.WriteLine(" 2. Add new vaccine to animal");
            Console.WriteLine(" 0. Back to Animal Management");

            Console.Write("\nChoice: ");
            switch (Console.ReadLine())
            {
                case "1": await ViewVaccinationHistoryAsync(); break;
                case "2": await AddVaccineAsync(); break;
                case "0": back = true; break;
                default: UIHelper.Warning("Invalid choice"); break;
            }

            if (!back)
            {
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }
    }

    // --- SOUS-MENU COMPATIBILITE ---
    private async Task ShowCompatibilityMenuAsync()
    {
        Console.Clear();
        UIHelper.ShowTitleMenu("Info & Compatibility");
        string animalId = ConsoleHelper.GetRequiredString("Enter Animal ID");

        bool back = false;
        while (!back)
        {
            Console.WriteLine("\n1. View/Add Compatibility (OK Cat, etc.)");
            Console.WriteLine("2. Update Description & Particularities");
            Console.WriteLine("3. Remove Compatibility");
            Console.WriteLine("0. Back");
            
            switch (Console.ReadLine())
            {
                case "1":
                    var c = new Compatibility {
                        AnimalId = animalId,
                        TargetType = ConsoleHelper.GetEnum<CompatibilityTypeEnum>("Target Type"),
                        ValueEnum = ConsoleHelper.GetEnum<CompatibilityValueEnum>("Value"),
                        Description = ConsoleHelper.GetString("Additional Note")
                    };
                    await _compatService.SetCompatibilityAsync(c);
                    UIHelper.Success("Compatibility updated.");
                    break;
                case "2":
                    string desc = ConsoleHelper.GetString("Description");
                    string part = ConsoleHelper.GetString("Particularities");
                    await _compatService.UpdateAnimalNotesAsync(animalId, desc, part);
                    UIHelper.Success("Animal notes updated.");
                    break;
                case "3":
                    var type = ConsoleHelper.GetEnum<CompatibilityTypeEnum>("Type to remove");
                    await _compatService.DeleteCompatibilityAsync(animalId, type);
                    UIHelper.Success("Compatibility removed.");
                    break;
                case "0": back = true; break;
            }
        }
    }

    // --- FONCTIONNALITÉS ANIMAL ---
    private async Task ListAnimalsAsync()
    {
        UIHelper.DrawBox("Active Animals");
        var animals = await _animalService.GetAvailableAnimalsAsync();

        if (!animals.Any())
        {
            UIHelper.Warning("No animals found.");
            return;
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("{0,-12} {1,-15} {2,-10} {3,-10}", "ID", "Name", "Species", "Status");
        Console.ResetColor();
        Console.WriteLine(new string('-', 50));

        foreach (var a in animals)
        {
            Console.WriteLine("{0,-12} {1,-15} {2,-10} {3,-10}", a.Id, a.Name, a.Species, a.CurrentStatus);
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    private async Task RegisterAnimalAsync()
    {
        UIHelper.DrawBox("Register New Animal");
        try
        {
            var animal = new Animal
            {
                Name = ConsoleHelper.GetRequiredString("Name"),
                Species = ConsoleHelper.GetEnum<SpeciesEnum>("Species"),
                Sex = ConsoleHelper.GetEnum<SexEnum>("Sex"),
                BirthDate = ConsoleHelper.GetOptionalDate("Birth Date"),
                Colors = ConsoleHelper.GetString("Colors"),
                Description = ConsoleHelper.GetString("Description")
            };

            string id = await _animalService.RegisterAnimalAsync(animal);
            UIHelper.Success($"Animal registered successfully! Assigned ID: {id}");
        }
        catch (ShelterException ex) { DisplayError(ex); }
    }

    private async Task ViewDetailsAsync()
    {
        UIHelper.DrawBox("Animal Details");
        string id = ConsoleHelper.GetRequiredString("Enter Animal ID");
        var animal = await _animalService.GetAnimalAsync(id);

        if (animal == null) { UIHelper.Warning("Animal not found."); return; }

        // --- Infos de base ---
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("ID: " + animal.Id);
        Console.WriteLine("Name: " + animal.Name);
        Console.WriteLine("Species: " + animal.Species);
        Console.WriteLine("Sex: " + animal.Sex);
        Console.WriteLine("Birth Date: " + (animal.BirthDate?.ToShortDateString() ?? "Unknown"));
        Console.WriteLine("Colors: " + animal.Colors);
        Console.WriteLine("Description: " + animal.Description);
        Console.WriteLine("Particularities: " + animal.Particularities);
        Console.ResetColor();

        Console.WriteLine("\n----------------------------------------");

        // --- Vaccinations ---
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Vaccination History:");
        Console.ResetColor();

        var vaccines = await _vaccinationService.GetAnimalVaccinationHistoryAsync(id);

        if (!vaccines.Any())
        {
            Console.WriteLine("  No vaccination records.");
        }
        else
        {
            foreach (var v in vaccines)
            {
                Console.WriteLine($"  - {v.VaccineDate.ToShortDateString()} : {v.VaccineName} ({(v.IsDone ? "DONE" : "PENDING")})");
            }
        }

        Console.WriteLine("\n----------------------------------------");

        // --- Compatibilités ---
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Compatibility:");
        Console.ResetColor();

        var compat = await _compatService.GetAnimalCompatibilitiesAsync(id);

        if (!compat.Any())
        {
            Console.WriteLine("  No compatibility data.");
        }
        else
        {
            foreach (var c in compat)
            {
                Console.WriteLine($"  - {c.TargetType}: {c.ValueEnum} {(string.IsNullOrWhiteSpace(c.Description) ? "" : $"({c.Description})")}");
            }
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    private async Task DeleteAnimalAsync()
    {
        UIHelper.DrawBox("Delete Animal");
        string id = ConsoleHelper.GetRequiredString("Enter Animal ID to delete");

        if (!ConsoleHelper.GetBool("Are you sure you want to delete this animal"))
        {
            UIHelper.Warning("Deletion cancelled.");
            return;
        }

        try
        {
            if (await _animalService.SoftDeleteAnimalAsync(id))
                UIHelper.Success("Animal deleted successfully.");
        }
        catch (ShelterException ex) { DisplayError(ex); }
    }

    private void DisplayError(ShelterException ex)
    {
        UIHelper.Error($"[Type: {ex.ErrorType}]\nMessage: {ex.Message}");
    }

    // --- FONCTIONNALITÉS VACCINATION ---
    private async Task ViewVaccinationHistoryAsync()
    {
        UIHelper.DrawBox("Vaccination History");
        string animalId = ConsoleHelper.GetRequiredString("Enter Animal ID");

        try
        {
            var history = await _vaccinationService.GetAnimalVaccinationHistoryAsync(animalId);

            if (!history.Any())
            {
                UIHelper.Warning("No vaccination records found for this animal.");
                return;
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("{0,-15} {1,-20} {2,-10}", "Date", "Vaccine Name", "Status");
            Console.ResetColor();
            Console.WriteLine(new string('-', 45));

            foreach (var v in history)
            {
                Console.WriteLine("{0,-15} {1,-20} {2,-10}",
                    v.VaccineDate.ToShortDateString(), v.VaccineName, v.IsDone ? "DONE" : "PENDING");
            }
        }
        catch (ShelterException ex) { DisplayError(ex); }
    }

    private async Task AddVaccineAsync()
    {
        UIHelper.DrawBox("Add New Vaccine");
        string animalId = ConsoleHelper.GetRequiredString("Enter Animal ID");

        try
        {
            var vaccine = new Vaccination
            {
                AnimalId = animalId,
                VaccineName = ConsoleHelper.GetRequiredString("Vaccine Name"),
                VaccineDate = ConsoleHelper.GetOptionalDate("Date (Leave empty for today)") ?? DateTime.Now,
                IsDone = ConsoleHelper.GetBool("Is the vaccine already administered")
            };

            await _vaccinationService.RegisterVaccinationAsync(vaccine);
            UIHelper.Success("Vaccination record added successfully.");
        }
        catch (ShelterException ex) { DisplayError(ex); }
    }

    // --- FONCTIONNALITÉS COMPATIBILITE ---
    private async Task ManageCompatibilityMenuAsync()
    {
        UIHelper.DrawBox("Info & Compatibility");
        string id = ConsoleHelper.GetRequiredString("Enter Animal ID");
        
        Console.WriteLine("1. View/Add Compatibility (OK Cat, OK Dog...)");
        Console.WriteLine("2. Update Description & Particularities");
        Console.WriteLine("3. Remove a Compatibility");
        
        string choice = Console.ReadLine();

        if (choice == "1") {
            var compatibility = new Compatibility {
                AnimalId = id,
                TargetType = ConsoleHelper.GetEnum<CompatibilityTypeEnum>("Target Type"),
                ValueEnum = ConsoleHelper.GetEnum<CompatibilityValueEnum>("Value"),
                Description = ConsoleHelper.GetString("Note (Optional)")
            };
            await _compatService.SetCompatibilityAsync(compatibility);
            UIHelper.Success("Compatibility updated.");
        }
        else if (choice == "2") {
            string desc = ConsoleHelper.GetString("New Description");
            string part = ConsoleHelper.GetString("New Particularities");
            await _compatService.UpdateAnimalNotesAsync(id, desc, part);
            UIHelper.Success("Notes updated.");
        }
    }
}
