using AnimalShelter.Core.Interfaces;
using AnimalShelter.Core.Models;
using AnimalShelter.Core.Exceptions;
using AnimalShelter.Core.Enums;
using AnimalShelter.ConsoleApp.UI.Utilities;

namespace AnimalShelter.ConsoleApp.UI;

public class AnimalConsoleUI
{
    private readonly IAnimalService _animalService;

    public AnimalConsoleUI(IAnimalService animalService)
    {
        _animalService = animalService;
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
            Console.WriteLine(" 0. Back to main menu");
            Console.ResetColor();

            Console.Write("\nSelect an option: ");
            switch (Console.ReadLine())
            {
                case "1":
                    Console.Clear();
                    await ListAnimalsAsync();
                    break;
                case "2":
                    Console.Clear();
                    await RegisterAnimalAsync();
                    break;
                case "3":
                    Console.Clear();
                    await ViewDetailsAsync();
                    break;
                case "4":
                    Console.Clear();
                    await DeleteAnimalAsync();
                    break;
                case "0":
                    Console.Clear();
                    exit = true;
                    break;
                default:
                    Console.Clear();
                    UIHelper.Warning("Invalid choice");
                    break;
            }

            if (!exit)
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

        Console.WriteLine($"\nID: {animal.Id}");
        Console.WriteLine($"Name: {animal.Name}");
        Console.WriteLine($"Species: {animal.Species} ({animal.Sex})");
        Console.WriteLine($"Status: {animal.CurrentStatus}");
        Console.WriteLine($"Birth Date: {animal.BirthDate?.ToShortDateString() ?? "Unknown"}");
        Console.WriteLine($"Colors: {animal.Colors}");
        Console.WriteLine($"Description: {animal.Description}");
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
}
