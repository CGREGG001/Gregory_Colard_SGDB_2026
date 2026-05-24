using AnimalShelter.Core.Interfaces;
using AnimalShelter.Core.Models;
using AnimalShelter.Core.Enums;
using AnimalShelter.ConsoleApp.UI.Utilities;

namespace AnimalShelter.ConsoleApp.UI;

public class AdoptionConsoleUI
{
    private readonly IAdoptionService _adoptionService;

    public AdoptionConsoleUI(IAdoptionService adoptionService) => _adoptionService = adoptionService;

    public async Task ShowMenuAsync()
    {
        bool exit = false;
        while (!exit)
        {
            Console.Clear();
            UIHelper.ShowTitleMenu("Adoption Management");
            Console.WriteLine(" 1. List all adoption files");
            Console.WriteLine(" 2. Create new adoption request");
            Console.WriteLine(" 3. Process adoption (Approve/Reject)");
            Console.WriteLine(" 0. Back");

            switch (Console.ReadLine())
            {
                case "1": await ListAdoptionsAsync(); break;
                case "2": await CreateRequestAsync(); break;
                case "3": await ProcessRequestAsync(); break;
                case "0": exit = true; break;
            }
            if (!exit) { Console.WriteLine("\nPress any key..."); Console.ReadKey(); }
        }
    }

    private async Task ListAdoptionsAsync()
    {
        var list = await _adoptionService.GetAllAdoptionsAsync();
        UIHelper.DrawBox("Adoption Files");
        Console.WriteLine("{0,-38} {1,-15} {2,-20} {3,-10}", "ID", "Animal", "Candidate", "Status");
        foreach (var a in list)
            Console.WriteLine($"{a.Id} | {a.AnimalName} | {a.ContactName} | {a.Status}");
    }

    private async Task CreateRequestAsync()
    {
        try {
            var file = new AdoptionFile {
                AnimalId = ConsoleHelper.GetRequiredString("Animal ID"),
                ContactId = Guid.Parse(ConsoleHelper.GetRequiredString("Candidate UUID"))
            };
            await _adoptionService.RequestAdoptionAsync(file);
            UIHelper.Success("Adoption request registered.");
        } catch (Exception ex) { UIHelper.Error(ex.Message); }
    }

    private async Task ProcessRequestAsync()
    {
        try {
            Guid id = Guid.Parse(ConsoleHelper.GetRequiredString("Adoption File UUID"));
            var status = ConsoleHelper.GetEnum<AdoptionStatusEnum>("New Status");
            await _adoptionService.ProcessAdoptionAsync(id, status);
            UIHelper.Success("Status updated. If approved, animal is now 'Adopted'.");
        } catch (Exception ex) { UIHelper.Error(ex.Message); }
    }
}
