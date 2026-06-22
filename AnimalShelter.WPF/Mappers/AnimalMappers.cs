using AnimalShelter.Core.Models;
using AnimalShelter.WPF.Models.Animals;

namespace AnimalShelter.WPF.Mappers
{
    public static class AnimalMappers
    {
        public static AnimalListingModel ToListingModel(this Animal a) => new()
        {
            Id = a.Id,
            Name = a.Name,
            Species = a.Species,
            Sex = a.Sex,
            BirthDate = a.BirthDate,
            CurrentStatus = a.CurrentStatus,
            Colors = a.Colors,
        };

        public static AnimalDetailsModel ToDetailsModel(this Animal a) => new()
        {
            Id = a.Id,
            Name = a.Name,
            Species = a.Species,
            Sex = a.Sex,
            Colors = a.Colors,
            IsSterilised = a.IsSterilised,
            SterilisationDate = a.SterilisationDate,
            BirthDate = a.BirthDate,
            DeathDate = a.DeathDate,
            Description = a.Description,
            Particularities = a.Particularities,
            CurrentStatus = a.CurrentStatus,
            CreatedAt = a.CreatedAt,
        };
    }
}
