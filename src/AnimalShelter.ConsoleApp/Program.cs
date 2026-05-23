using AnimalShelter.Core.Models;
using AnimalShelter.Core.Enums;
using AnimalShelter.Core.Exceptions;
using AnimalShelter.DAL.Infrastructure;
using AnimalShelter.DAL.Infrastructure.Enums;
using AnimalShelter.DAL.Repositories;
using AnimalShelter.BLL.Services;

Console.WriteLine("=== Shelter Management System - BLL Validation ===");

// 1. Setup (Normalement géré par Injection de Dépendances)
var enumMapper = new EnumMapper();
var dbFactory = new DbConnectionFactory(enumMapper);
var repo = new AnimalRepository(dbFactory);
var service = new AnimalService(repo);

try
{
    // TEST 1 : Validation des dates (Doit échouer)
    Console.WriteLine("\n[Test 1] Registering animal with future birth date...");
    await service.RegisterAnimalAsync(new Animal { Name = "FutureDog", BirthDate = DateTime.Now.AddDays(1) });
}
catch (ShelterException ex)
{
    Console.WriteLine($"Expected Error: {ex.Message} (Type: {ex.ErrorType})");
}

try
{
    // TEST 2 : Doublon (Rex existe déjà en base suite à ton test précédent)
    Console.WriteLine("\n[Test 2] Registering a duplicate animal (Rex)...");
    var rex = new Animal { Name = "Rex", Species = SpeciesEnum.Dog, BirthDate = null };
    await service.RegisterAnimalAsync(rex);
}
catch (ShelterException ex)
{
    Console.WriteLine($"Expected Error: {ex.Message} (Type: {ex.ErrorType})");
}

try
{
    // TEST 3 : Succès
    Console.WriteLine("\n[Test 3] Registering a valid new animal...");
    var luna = new Animal { Name = "Luna", Species = SpeciesEnum.Cat, Sex = SexEnum.Female, BirthDate = new DateTime(2022, 01, 15) };
    string id = await service.RegisterAnimalAsync(luna);
    Console.WriteLine($"Success! Luna registered with ID: {id}");
}
catch (ShelterException ex)
{
    Console.WriteLine($"Unexpected Error: {ex.Message}");
}

Console.WriteLine("\n=== Tests Completed ===");
