using AnimalShelter.ConsoleApp.UI;
using AnimalShelter.BLL.Services;
using AnimalShelter.DAL.Repositories;
using AnimalShelter.DAL.Infrastructure;
using AnimalShelter.DAL.Infrastructure.Enums;
using AnimalShelter.ConsoleApp.UI.Utilities;
using AnimalShelter.ConsoleApp.TechTests; // Pour Tech Test uniquement !

// 1. Composition Root (Initialisation)
var enumMapper = new EnumMapper();
var dbFactory = new DbConnectionFactory(enumMapper);
var animalRepo = new AnimalRepository(dbFactory);
var animalService = new AnimalService(animalRepo);
var animalUI = new AnimalConsoleUI(animalService);

// 2. Splash Screen
Console.Clear();
Console.WriteLine("\n\n          === SHELTER MANAGEMENT SYSTEM v1.0 ===\n\n");
UIHelper.ShowHeader();

// 3. Main Loop
bool exit = false;
while (!exit)
{
    UIHelper.ShowTitleMenu("Animal Management");
    Console.WriteLine("1. Manage Animals");
    Console.WriteLine("2. Manage Contacts (Tech Test)");
    Console.WriteLine("0. Exit");
    Console.Write("\nChoice: ");

    switch (Console.ReadLine())
    {
        case "1": await animalUI.ShowMenuAsync(); break;
        case "2": await ContactDallTechTest.RunAsync(); break;
        case "0": exit = true; break;
        default: Console.WriteLine("Invalid option."); break;
    }
}

Console.Clear();
Console.WriteLine("Goodbye!");
