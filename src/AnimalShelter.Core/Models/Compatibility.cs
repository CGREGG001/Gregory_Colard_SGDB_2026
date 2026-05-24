using AnimalShelter.Core.Enums;

namespace AnimalShelter.Core.Models
{
    public class Compatibility : BaseEntity<Guid>
    {
        public string AnimalId { get; set; } = string.Empty;
        public CompatibilityTypeEnum TargetType { get; set; }
        public CompatibilityValueEnum ValueEnum { get; set; }
        public string? Description { get; set; }
    }
}
