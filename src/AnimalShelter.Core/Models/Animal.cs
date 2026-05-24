using AnimalShelter.Core.Enums;

namespace AnimalShelter.Core.Models
{
    public class Animal : BaseEntity<string>
    {
        public string Name { get; set; } = string.Empty;
        public SpeciesEnum Species { get; set; }
        public SexEnum Sex { get; set; }
        public string? Colors { get; set; }
        public bool IsSterilised { get; set; }
        public DateTime? SterilisationDate { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? DeathDate { get; set; }
        public string? Description { get; set; }
        public string? Particularities { get; set; }
        public AnimalStatusEnum CurrentStatus { get; set; }
    }
}
