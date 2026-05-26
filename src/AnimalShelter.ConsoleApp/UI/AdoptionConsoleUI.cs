using AnimalShelter.Core.Interfaces;
using AnimalShelter.Core.Models;
using AnimalShelter.Core.Enums;
using AnimalShelter.Core.Exceptions;
using AnimalShelter.ConsoleApp.UI.Utilities;

namespace AnimalShelter.ConsoleApp.UI;

public class AdoptionConsoleUI(IAdoptionService adoptionService)
{
    #region fields
    private readonly IAdoptionService _adoptionService = adoptionService;

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
            UIHelper.ShowTitle("Adoption Management");

            Console.WriteLine(" 1. List adoption files");
            Console.WriteLine(" 2. Create adoption request");
            Console.WriteLine(" 3. Process adoption (Approve / Reject)");
            Console.WriteLine(" 0. Back");

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("\nSelection > ");
            Console.ResetColor();

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1": await ListAdoptionsAsync(); break;
                case "2": await CreateRequestAsync(); break;
                case "3": await ProcessRequestAsync(); break;
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
    // 1. LIST ADOPTION FILES
    // ============================================================
    private async Task ListAdoptionsAsync()
    {
        Console.Clear();
        UIHelper.ShowTitle("Adoption Files");

        var list = await _adoptionService.GetAllAdoptionsAsync();

        if (!list.Any())
        {
            UIHelper.Warning("No adoption files found.");
            return;
        }

        var rows = list
            .Select(a => new string[]
            {
                a.Id.ToString(),
                a.AnimalName ?? "Unknown",
                a.ContactName ?? "Unknown",
                a.Status.ToString()
            })
            .ToList();

        UIHelper.ShowTable(
            ["ID", "Animal", "Candidate", "Status"],
            rows
        );
    }

    // ============================================================
    // 2. CREATE ADOPTION REQUEST
    // ============================================================
    private async Task CreateRequestAsync()
    {
        Console.Clear();
        UIHelper.ShowTitle("New Adoption Request");

        try
        {
            var file = new AdoptionFile
            {
                AnimalId = ConsoleHelper.GetRequiredString("Animal ID"),
                ContactId = UIHelper.AskGuid("Candidate UUID")
            };

            await _adoptionService.RequestAdoptionAsync(file);
            UIHelper.Success("Adoption request registered.");
        }
        catch (ShelterException ex)
        {
            UIHelper.Error(ex.Message);
        }
        catch (Exception ex)
        {
            UIHelper.Error(ex.Message);
        }
    }

    // ============================================================
    // 3. PROCESS ADOPTION REQUEST
    // ============================================================
    private async Task ProcessRequestAsync()
    {
        Console.Clear();
        UIHelper.ShowTitle("Process Adoption");

        try
        {
            Guid id = UIHelper.AskGuid("Adoption File UUID");
            var status = UIHelper.AskEnum<AdoptionStatusEnum>("New Status");

            if (!UIHelper.Confirm($"Confirm status change to '{status}'"))
            {
                UIHelper.Warning("Operation cancelled.");
                return;
            }

            await _adoptionService.ProcessAdoptionAsync(id, status);

            UIHelper.Success(
                status == AdoptionStatusEnum.Approved
                ? "Adoption approved. Animal is now 'Adopted'."
                : "Adoption status updated."
            );
        }
        catch (ShelterException ex)
        {
            UIHelper.Error(ex.Message);
        }
        catch (Exception ex)
        {
            UIHelper.Error(ex.Message);
        }
    }
    #endregion
}
