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

    public AnimalConsoleUI(IAnimalService animalService, IVaccinationService vaccinationService)
    {
        _animalService = animalService;
        _vaccinationService = vaccinationService;
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
            Console.WriteLine(" 4. Delete animal (Soft Delete)");
            Console.WriteLine(" 5. Vaccination Management"); // Nouvelle option
            Console.WriteLine(" 0. Back to main menu");

            Console.Write("\nSelect an option: ");
            switch (Console.ReadLine())
            {
                case "1": await ListAnimalsAsync(); break;
                case "2": await RegisterAnimalAsync(); break;
                case "3": await ViewDetailsAsync(); break;
                case "4": await DeleteAnimalAsync(); break;
                case "5": await ShowVaccinationMenuAsync(); break; // Vers le sous-menu
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
}
