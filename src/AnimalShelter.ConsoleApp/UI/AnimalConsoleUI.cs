using AnimalShelter.Core.Interfaces;
using AnimalShelter.Core.Models;
using AnimalShelter.Core.Exceptions;
using AnimalShelter.Core.Enums;
using AnimalShelter.ConsoleApp.UI.Utilities;

namespace AnimalShelter.ConsoleApp.UI;

public class AnimalConsoleUI(
    IAnimalService animalService,
    IVaccinationService vaccinationService,
    ICompatibilityService compatService)
{
    #region fields
    private readonly IAnimalService _animalService = animalService;
    private readonly IVaccinationService _vaccinationService = vaccinationService;
    private readonly ICompatibilityService _compatService = compatService;

    #endregion
    #region constructors
    #endregion

    #region methods
    public async Task ShowMenuAsync()
    {
        bool exit = false;

        while (!exit)
        {
            Console.Clear();
            UIHelper.ShowHeader();
            UIHelper.ShowTitle("Animal Management");

            Console.WriteLine(" 1. List active animals");
            Console.WriteLine(" 2. Register new animal");
            Console.WriteLine(" 3. View animal details");
            Console.WriteLine(" 4. Delete animal");
            Console.WriteLine(" 5. Vaccination Management");
            Console.WriteLine(" 6. Info & Compatibility");
            Console.WriteLine(" 0. Back");

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("\nSelection > ");
            Console.ResetColor();

            string? choice = Console.ReadLine();

            switch (choice)
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

            if (!exit)
            {
                UIHelper.Pause();
            }
        }
    }

    // ============================================================
    //  LIST ANIMALS
    // ============================================================
    private async Task ListAnimalsAsync()
    {
        Console.Clear();
        UIHelper.ShowTitle("Active Animals");

        var animals = await _animalService.GetAvailableAnimalsAsync();

        if (!animals.Any())
        {
            UIHelper.Warning("No animals found.");
            return;
        }

        var rows = animals
            .Select(a => new string[]
            {
                a.Id,
                a.Name,
                a.Species.ToString(),
                a.CurrentStatus.ToString()
            })
            .ToList();

        UIHelper.ShowTable(
            ["ID", "Name", "Species", "Status"],
            rows
        );
    }

    // ============================================================
    //  REGISTER ANIMAL
    // ============================================================
    private async Task RegisterAnimalAsync()
    {
        Console.Clear();
        UIHelper.ShowTitle("Register New Animal");

        try
        {
            var animal = new Animal
            {
                Name = ConsoleHelper.GetRequiredString("Name"),
                Species = UIHelper.AskEnum<SpeciesEnum>("Species"),
                Sex = UIHelper.AskEnum<SexEnum>("Sex"),
                BirthDate = UIHelper.AskDate("Birth Date", optional: true),
                Colors = ConsoleHelper.GetString("Colors"),
                Description = ConsoleHelper.GetString("Description")
            };

            string id = await _animalService.RegisterAnimalAsync(animal);
            UIHelper.Success($"Animal registered successfully! Assigned ID: {id}");
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
        UIHelper.ShowTitle("Animal Details");

        string id = ConsoleHelper.GetRequiredString("Enter Animal ID");
        var animal = await _animalService.GetAnimalAsync(id);

        if (animal == null)
        {
            UIHelper.Warning("Animal not found.");
            return;
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"ID: {animal.Id}");
        Console.WriteLine($"Name: {animal.Name}");
        Console.WriteLine($"Species: {animal.Species}");
        Console.WriteLine($"Sex: {animal.Sex}");
        Console.WriteLine($"Birth Date: {animal.BirthDate?.ToShortDateString() ?? "Unknown"}");
        Console.WriteLine($"Colors: {animal.Colors}");
        Console.WriteLine($"Description: {animal.Description}");
        Console.WriteLine($"Particularities: {animal.Particularities}");
        Console.ResetColor();

        Console.WriteLine("\n----------------------------------------");

        await ShowVaccinationSectionAsync(id);
        await ShowCompatibilitySectionAsync(id);
    }

    private async Task ShowVaccinationSectionAsync(string animalId)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Vaccination History:");
        Console.ResetColor();

        var vaccines = await _vaccinationService.GetAnimalVaccinationHistoryAsync(animalId);

        if (!vaccines.Any())
        {
            Console.WriteLine("  No vaccination records.");
            return;
        }

        var rows = vaccines
            .Select(vaccine => new string[]
            {
                vaccine.VaccineDate.ToShortDateString(),
                vaccine.VaccineName,
                vaccine.IsDone ? "DONE" : "PENDING"
            })
            .ToList();

        UIHelper.ShowTable(
            ["Date", "Vaccine", "Status"],
            rows
        );
    }

    private async Task ShowCompatibilitySectionAsync(string animalId)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\nCompatibility:");
        Console.ResetColor();

        var compat = await _compatService.GetAnimalCompatibilitiesAsync(animalId);

        if (!compat.Any())
        {
            Console.WriteLine("  No compatibility data.");
            return;
        }

        var rows = compat
            .Select(c => new string[]
            {
                c.TargetType.ToString(),
                c.ValueEnum.ToString(),
                c.Description ?? ""
            })
            .ToList();

        UIHelper.ShowTable(
            ["Type", "Value", "Description"],
            rows
        );
    }

    // ============================================================
    //  DELETE ANIMAL
    // ============================================================
    private async Task DeleteAnimalAsync()
    {
        Console.Clear();
        UIHelper.ShowTitle("Delete Animal");

        string id = ConsoleHelper.GetRequiredString("Enter Animal ID");

        if (!UIHelper.Confirm("Are you sure you want to delete this animal"))
        {
            UIHelper.Warning("Deletion cancelled.");
            return;
        }

        try
        {
            if (await _animalService.SoftDeleteAnimalAsync(id))
                UIHelper.Success("Animal deleted successfully.");
        }
        catch (ShelterException ex)
        {
            UIHelper.Error(ex.Message);
        }
    }

    // ============================================================
    //  SUB-MENUS
    // ============================================================
    private async Task ShowVaccinationMenuAsync()
    {

        bool back = false;

        while (!back)
        {
            Console.Clear();
            UIHelper.ShowTitle("Vaccination Management");

            Console.WriteLine(" 1. View vaccination history");
            Console.WriteLine(" 2. Add new vaccine");
            Console.WriteLine(" 0. Back");

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("\nSelection > ");
            Console.ResetColor();

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1": await ViewVaccinationHistoryAsync(); break;
                case "2": await AddVaccineAsync(); break;
                case "0": back = true; break;
                default: UIHelper.Warning("Invalid choice"); break;
            }

        }
    }

    private async Task ShowCompatibilityMenuAsync()
    {
        Console.Clear();
        UIHelper.ShowTitle("Info & Compatibility");

        string id = ConsoleHelper.GetRequiredString("Enter Animal ID");

        Console.WriteLine(" 1. Add/Update Compatibility");
        Console.WriteLine(" 2. Update Description & Particularities");
        Console.WriteLine(" 3. Remove Compatibility");
        Console.WriteLine(" 0. Back");

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("\nSelection > ");
        Console.ResetColor();

        string? choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                var c = new Compatibility
                {
                    AnimalId = id,
                    TargetType = UIHelper.AskEnum<CompatibilityTypeEnum>("Target Type"),
                    ValueEnum = UIHelper.AskEnum<CompatibilityValueEnum>("Value"),
                    Description = ConsoleHelper.GetString("Additional Note")
                };
                await _compatService.SetCompatibilityAsync(c);
                UIHelper.Success("Compatibility updated.");
                break;

            case "2":
                string desc = ConsoleHelper.GetString("Description");
                string part = ConsoleHelper.GetString("Particularities");
                await _compatService.UpdateAnimalNotesAsync(id, desc, part);
                UIHelper.Success("Notes updated.");
                break;

            case "3":
                var type = UIHelper.AskEnum<CompatibilityTypeEnum>("Type to remove");
                await _compatService.DeleteCompatibilityAsync(id, type);
                UIHelper.Success("Compatibility removed.");
                break;
        }
    }

    // ============================================================
    //  VACCINATION ACTIONS
    // ============================================================
    private async Task ViewVaccinationHistoryAsync()
    {
        string id = ConsoleHelper.GetRequiredString("Enter Animal ID");

        Console.Clear();
        UIHelper.ShowTitle("Vaccination History");

        await ShowVaccinationSectionAsync(id);

        UIHelper.Pause();
    }

    private async Task AddVaccineAsync()
    {
        Console.Clear();
        UIHelper.ShowTitle("Add New Vaccine");

        string id = ConsoleHelper.GetRequiredString("Enter Animal ID");

        try
        {
            var vaccine = new Vaccination
            {
                AnimalId = id,
                VaccineName = ConsoleHelper.GetRequiredString("Vaccine Name"),
                VaccineDate = UIHelper.AskDate("Date", optional: true),
                IsDone = UIHelper.Confirm("Is the vaccine already administered")
            };

            await _vaccinationService.RegisterVaccinationAsync(vaccine);
            UIHelper.Success("Vaccination record added.");
        }
        catch (ShelterException ex)
        {
            UIHelper.Error(ex.Message);
        }
    }
    #endregion
}
