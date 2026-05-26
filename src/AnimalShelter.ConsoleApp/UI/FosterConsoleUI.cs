using AnimalShelter.Core.Interfaces;
using AnimalShelter.Core.Models;
using AnimalShelter.ConsoleApp.UI.Utilities;

namespace AnimalShelter.ConsoleApp.UI;

public class FosterConsoleUI(IFosterService fosterService)
{
    #region fields
    private readonly IFosterService _fosterService = fosterService;

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
            UIHelper.ShowTitle("Foster Care Management");

            Console.WriteLine(" 1. Animal foster history");
            Console.WriteLine(" 2. Animals currently in a family");
            Console.WriteLine(" 3. Register new foster stay");
            Console.WriteLine(" 4. End a foster stay");
            Console.WriteLine(" 0. Back");

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("\nSelection > ");
            Console.ResetColor();

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1": await ListFamiliesByAnimalAsync(); break;
                case "2": await ListAnimalsByFamilyAsync(); break;
                case "3": await RegisterNewStayAsync(); break;
                case "4": await EndStayAsync(); break;
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
    // 1. HISTORY OF FOSTER FAMILIES FOR AN ANIMAL
    // ============================================================
    private async Task ListFamiliesByAnimalAsync()
    {
        Console.Clear();
        UIHelper.ShowTitle("Animal Foster History");

        string animalId = ConsoleHelper.GetRequiredString("Enter Animal ID");
        var history = await _fosterService.GetAnimalHistoryAsync(animalId);

        if (!history.Any())
        {
            UIHelper.Warning("No foster history found for this animal.");
            return;
        }

        var rows = history
            .Select(s => new string[]
            {
                s.StartDate.ToString("yyyy-MM-dd"),
                s.EndDate?.ToString("yyyy-MM-dd") ?? "CURRENT",
                s.ContactName ?? "Unknown"
            })
            .ToList();

        UIHelper.ShowTable(
            ["Start", "End", "Family"],
            rows
        );
    }

    // ============================================================
    // 2. ANIMALS CURRENTLY IN A FAMILY
    // ============================================================
    private async Task ListAnimalsByFamilyAsync()
    {
        Console.Clear();
        UIHelper.ShowTitle("Animals in Family");

        Guid contactId = UIHelper.AskGuid("Enter Contact UUID");
        var animals = await _fosterService.GetFamilyCurrentAnimalsAsync(contactId);

        if (!animals.Any())
        {
            UIHelper.Warning("This family is not hosting any animals.");
            return;
        }

        var rows = animals
            .Select(a => new string[]
            {
                a.AnimalName ?? "Unknown",
                a.StartDate.ToString("yyyy-MM-dd")
            })
            .ToList();

        UIHelper.ShowTable(
            ["Animal", "Since"],
            rows
        );
    }

    // ============================================================
    // 3. REGISTER NEW FOSTER STAY
    // ============================================================
    private async Task RegisterNewStayAsync()
    {
        Console.Clear();
        UIHelper.ShowTitle("New Foster Placement");

        try
        {
            var stay = new FosterStay
            {
                AnimalId = ConsoleHelper.GetRequiredString("Animal ID"),
                ContactId = UIHelper.AskGuid("Contact UUID"),
                StartDate = UIHelper.AskDate("Start Date", optional: true)
            };

            await _fosterService.StartFosterStayAsync(stay);
            UIHelper.Success("Placement successful. Animal status updated to 'Fostered'.");
        }
        catch (Exception ex)
        {
            UIHelper.Error(ex.Message);
        }
    }

    // ============================================================
    // 4. END FOSTER STAY
    // ============================================================
    private async Task EndStayAsync()
    {
        Console.Clear();
        UIHelper.ShowTitle("End Foster Stay");

        Guid stayId = UIHelper.AskGuid("Enter Stay UUID");

        if (!UIHelper.Confirm("Confirm ending this foster stay"))
        {
            UIHelper.Warning("Operation cancelled.");
            return;
        }

        try
        {
            await _fosterService.EndFosterStayAsync(stayId, DateTime.Now);
            UIHelper.Success("Stay ended. Animal is now back at the shelter.");
        }
        catch (Exception ex)
        {
            UIHelper.Error(ex.Message);
        }
    }
    #endregion
}
