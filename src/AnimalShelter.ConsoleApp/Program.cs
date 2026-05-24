using AnimalShelter.ConsoleApp.UI;
using AnimalShelter.BLL.Services;
using AnimalShelter.DAL.Repositories;
using AnimalShelter.DAL.Infrastructure;
using AnimalShelter.DAL.Infrastructure.Enums;
using AnimalShelter.ConsoleApp.UI.Utilities;

// ---------------------------------------------------------
// 1. COMPOSITION ROOT (Infrastructure & Dependency Setup)
// ---------------------------------------------------------
// Infrastructure
var enumMapper = new EnumMapper();
var dbFactory = new DbConnectionFactory(enumMapper);

// Animal Module
var animalRepo = new AnimalRepository(dbFactory);
var animalService = new AnimalService(animalRepo);
var animalUI = new AnimalConsoleUI(animalService);

// Contact Module
var contactRepo = new ContactRepository(dbFactory);
var contactService = new ContactService(contactRepo);
var contactUI = new ContactConsoleUI(contactService);

// ---------------------------------------------------------
// 2. SPLASH SCREEN (PawShelter Experience)
// ---------------------------------------------------------
Console.Clear();
Console.WriteLine("\n\n          === SHELTER MANAGEMENT SYSTEM v1.0 ===\n\n");
// UIHelper.ShowHeader();

// ---------------------------------------------------------
// 3. MAIN APPLICATION LOOP
// ---------------------------------------------------------
bool exit = false;
while (!exit)
{
    UIHelper.ShowTitleMenu("Animal Management");
    Console.WriteLine("1. Manage Animals");
    Console.WriteLine("2. Manage Contacts");
    Console.WriteLine("0. Exit");

    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.Write("\nSelection > ");
    Console.ResetColor();

    string? choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            await animalUI.ShowMenuAsync();
            break;
        case "2":
            await contactUI.ShowMenuAsync();
            break;
        case "0":
            exit = true;
            break;
        default:
            UIHelper.Warning("Invalid selection. Please choose a valid menu option.");
            Thread.Sleep(1000);
            break;
    }
}

// Exit sequence
Console.Clear();
UIHelper.DrawBox("System Shutdown");
Console.WriteLine("\nThank you for using PawShelter. Saving session data...");
Thread.Sleep(1000);
Console.WriteLine("Goodbye!");
