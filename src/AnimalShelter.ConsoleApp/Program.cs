using AnimalShelter.ConsoleApp.UI;
using AnimalShelter.BLL.Services;
using AnimalShelter.DAL.Repositories;
using AnimalShelter.DAL.Infrastructure;
using AnimalShelter.DAL.Infrastructure.Enums;
using AnimalShelter.ConsoleApp.UI.Utilities;

// ---------------------------------------------------------
// 1. COMPOSITION ROOT (Infrastructure & Dependency Setup)
// ---------------------------------------------------------
// --- Infrastructure ---
var enumMapper = new EnumMapper();
var dbFactory = new DbConnectionFactory(enumMapper);

// --- Repositories (Couche DAL) ---
var animalRepo = new AnimalRepository(dbFactory);
var vaccinationRepo = new VaccinationRepository(dbFactory);
var compatibilityRepo = new CompatibilityRepository(dbFactory);
var contactRepo = new ContactRepository(dbFactory);
var fosterRepo = new FosterRepository(dbFactory);

// --- Services (Couche BLL) ---
// Note: VaccinationService et CompatibilityService ont besoin de animalRepo.
var vaccinationService = new VaccinationService(vaccinationRepo, animalRepo);
var compatibilityService = new CompatibilityService(compatibilityRepo, animalRepo);
var fosterService = new FosterService(fosterRepo, animalRepo);

// AnimalService reste indépendant
var animalService = new AnimalService(animalRepo);

// ContactService
var contactService = new ContactService(contactRepo);

// --- UI (Couche Présentation) ---
// L'UI Animal a besoin des trois services pour gérer les détails + les vaccins + les compatibilités
var animalUI = new AnimalConsoleUI(animalService, vaccinationService, compatibilityService);
var contactUI = new ContactConsoleUI(contactService);
var fosterUI = new FosterConsoleUI(fosterService);

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
    Console.WriteLine("3. Manage Foster Stays");
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
        case "3":
            await fosterUI.ShowMenuAsync();
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
