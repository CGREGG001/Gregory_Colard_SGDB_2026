using AnimalShelter.Core.Enums;

namespace AnimalShelter.WPF.Models.Animals
{
    public class AnimalListingModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public SpeciesEnum Species { get; set; }
        public SexEnum Sex { get; set; }
        public DateTime? BirthDate { get; set; }
        public AnimalStatusEnum CurrentStatus { get; set; }
        public string? Colors { get; set; }
    }
}
