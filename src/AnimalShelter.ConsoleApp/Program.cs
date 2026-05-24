using AnimalShelter.ConsoleApp.UI;
using AnimalShelter.ConsoleApp.UI.Utilities;
using AnimalShelter.BLL.Services;
using AnimalShelter.DAL.Repositories;
using AnimalShelter.DAL.Infrastructure;
using AnimalShelter.DAL.Infrastructure.Enums;

// ============================================================
// 1. COMPOSITION ROOT (Infrastructure & Dependency Setup)
// ============================================================

// --- Infrastructure ---
var enumMapper = new EnumMapper();
var dbFactory = new DbConnectionFactory(enumMapper);

// --- Repositories (DAL) ---
var animalRepo = new AnimalRepository(dbFactory);
var vaccinationRepo = new VaccinationRepository(dbFactory);
var compatibilityRepo = new CompatibilityRepository(dbFactory);
var contactRepo = new ContactRepository(dbFactory);
var fosterRepo = new FosterRepository(dbFactory);
var adoptionRepo = new AdoptionRepository(dbFactory);

// --- Services (BLL) ---
var vaccinationService = new VaccinationService(vaccinationRepo, animalRepo);
var compatibilityService = new CompatibilityService(compatibilityRepo, animalRepo);
var fosterService = new FosterService(fosterRepo, animalRepo);
var adoptionService = new AdoptionService(adoptionRepo, animalRepo);
var animalService = new AnimalService(animalRepo);
var contactService = new ContactService(contactRepo);

// --- UI (Presentation Layer) ---
var animalUI = new AnimalConsoleUI(animalService, vaccinationService, compatibilityService);
var contactUI = new ContactConsoleUI(contactService);
var fosterUI = new FosterConsoleUI(fosterService);
var adoptionUI = new AdoptionConsoleUI(adoptionService);

// ============================================================
// 2. SPLASH SCREEN
// ============================================================
UIHelper.SplashScreen();

// ============================================================
// 3. MAIN APPLICATION LOOP
// ============================================================
bool exit = false;

while (!exit)
{
    Console.Clear();
    UIHelper.ShowHeader();
    UIHelper.ShowTitle("PawShelter Management");

    Console.WriteLine(" 1. Manage Animals");
    Console.WriteLine(" 2. Manage Contacts");
    Console.WriteLine(" 3. Manage Foster Stays");
    Console.WriteLine(" 4. Manage Adoption Files");
    Console.WriteLine(" 0. Exit");

    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.Write("\nSelection > ");
    Console.ResetColor();

    string? choice = Console.ReadLine();

    switch (choice)
    {
        case "1": await animalUI.ShowMenuAsync(); break;
        case "2": await contactUI.ShowMenuAsync(); break;
        case "3": await fosterUI.ShowMenuAsync(); break;
        case "4": await adoptionUI.ShowMenuAsync(); break;
        case "0": exit = true; break;
        default:
            UIHelper.Warning("Invalid selection. Please choose a valid menu option.");
            Thread.Sleep(900);
            break;
    }
}

// ============================================================
// 4. EXIT SEQUENCE
// ============================================================
Console.Clear();
UIHelper.ShowTitle("System Shutdown");
UIHelper.LoadingDots("Saving session data");
Thread.Sleep(800);
Console.WriteLine("Goodbye!");
Thread.Sleep(600);
