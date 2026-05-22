using AnimalShelter.Core.Enums;
using AnimalShelter.Core.Models;
using AnimalShelter.DAL.Infrastructure;
using AnimalShelter.DAL.Infrastructure.Enums;
using AnimalShelter.DAL.Repositories;

Console.WriteLine("=== Shelter Management - Tech Test ===");

try
{
    // 1. Initialisation de l'infrastructure
    var enumMapper = new EnumMapper();
    var connectionFactory = new DbConnectionFactory(enumMapper);
    var animalRepo = new AnimalRepository(connectionFactory);

    // 2. Test : Ajout d'un animal
    Console.WriteLine("\n[1] Adding a new animal...");
    var rex = new Animal
    {
        Name = "Rex",
        Species = SpeciesEnum.Dog,
        Sex = SexEnum.Male,
        Colors = "Brown and Black",
        Description = "A very friendly German Shepherd.",
        IsSterilised = false
    };

    string newId = await animalRepo.AddAsync(rex);
    Console.WriteLine($"Successfully added! Generated ID: {newId}");

    // 3. Test : Lecture de tous les animaux
    Console.WriteLine("\n[2] Fetching all active animals...");
    var animals = await animalRepo.GetAllActiveAsync();

    foreach (var a in animals)
    {
        Console.WriteLine($"- [{a.Id}] {a.Name} ({a.Species}) - Status: {a.CurrentStatus}");
    }

    // 4. Test : Lecture par ID
    Console.WriteLine($"\n[3] Fetching animal by ID: {newId}");
    var fetched = await animalRepo.GetByIdAsync(newId);
    if (fetched != null)
    {
        Console.WriteLine($"Found: {fetched.Name}, Colors: {fetched.Colors}");
    }

}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"\nERROR: {ex.Message}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"Inner: {ex.InnerException.Message}");
    }
    Console.ResetColor();
}

Console.WriteLine("\n=== Test Finished ===");