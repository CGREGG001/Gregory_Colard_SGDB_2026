using AnimalShelter.Core.Interfaces;
using AnimalShelter.Core.Models;
using AnimalShelter.ConsoleApp.UI.Utilities;

namespace AnimalShelter.ConsoleApp.UI;

public class FosterConsoleUI
{
    private readonly IFosterService _fosterService;

    public FosterConsoleUI(IFosterService fosterService)
    {
        _fosterService = fosterService;
    }

    public async Task ShowMenuAsync()
    {
        bool exit = false;
        while (!exit)
        {
            Console.Clear();
            UIHelper.ShowTitleMenu("Foster Care Management");

            Console.WriteLine(" 1. List families for an animal (History)");
            Console.WriteLine(" 2. List animals currently in a family");
            Console.WriteLine(" 3. Register new foster stay (Move animal)");
            Console.WriteLine(" 4. End a foster stay (Return to shelter)");
            Console.WriteLine(" 0. Back to main menu");

            Console.Write("\nSelect an option: ");
            switch (Console.ReadLine())
            {
                case "1": await ListFamiliesByAnimalAsync(); break;
                case "2": await ListAnimalsByFamilyAsync(); break;
                case "3": await RegisterNewStayAsync(); break;
                case "4": await EndStayAsync(); break;
                case "0": exit = true; break;
                default: UIHelper.Warning("Invalid choice"); break;
            }

            if (!exit) { Console.WriteLine("\nPress any key to continue..."); Console.ReadKey(); }
        }
    }

    private async Task ListFamiliesByAnimalAsync()
    {
        UIHelper.DrawBox("Animal Foster History");
        string animalId = ConsoleHelper.GetRequiredString("Enter Animal ID");
        var history = await _fosterService.GetAnimalHistoryAsync(animalId);

        if (!history.Any()) { UIHelper.Warning("No history found for this animal."); return; }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("{0,-12} {1,-12} {2,-25}", "Start", "End", "Family Name");
        Console.ResetColor();
        foreach (var s in history)
        {
            Console.WriteLine($"{s.StartDate:yyyy-MM-dd} | {s.EndDate?.ToString("yyyy-MM-dd") ?? "CURRENT"} | {s.ContactName}");
        }
    }

    private async Task ListAnimalsByFamilyAsync()
    {
        UIHelper.DrawBox("Animals in Family");
        var contactId = Guid.Parse(ConsoleHelper.GetRequiredString("Enter Contact UUID"));
        var animals = await _fosterService.GetFamilyCurrentAnimalsAsync(contactId);

        if (!animals.Any()) { UIHelper.Warning("No animals currently in this family."); return; }

        foreach (var s in animals)
            Console.WriteLine($"- {s.AnimalName} (Since {s.StartDate:yyyy-MM-dd})");
    }

    private async Task RegisterNewStayAsync()
    {
        UIHelper.DrawBox("New Foster Placement");
        try {
            var stay = new FosterStay {
                AnimalId = ConsoleHelper.GetRequiredString("Animal ID"),
                ContactId = Guid.Parse(ConsoleHelper.GetRequiredString("Contact UUID")),
                StartDate = ConsoleHelper.GetOptionalDate("Start Date") ?? DateTime.Now
            };
            await _fosterService.StartFosterStayAsync(stay);
            UIHelper.Success("Placement successful. Animal status updated to 'Fostered'.");
        }
        catch (Exception ex) { UIHelper.Error(ex.Message); }
    }

    private async Task EndStayAsync()
    {
        UIHelper.DrawBox("End Foster Stay");
        Guid stayId = Guid.Parse(ConsoleHelper.GetRequiredString("Enter Stay UUID"));
        try {
            await _fosterService.EndFosterStayAsync(stayId, DateTime.Now);
            UIHelper.Success("Stay ended. Animal is now back at the shelter.");
        }
        catch (Exception ex) { UIHelper.Error(ex.Message); }
    }
}
