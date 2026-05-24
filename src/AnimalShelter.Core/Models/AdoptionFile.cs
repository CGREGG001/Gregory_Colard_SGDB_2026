using AnimalShelter.Core.Enums;

namespace AnimalShelter.Core.Models
{
    public class AdoptionFile : BaseEntity<Guid>
    {
        public string AnimalId { get; set; } = string.Empty;
        public Guid ContactId { get; set; }
        public DateTime RequestDate { get; set; }
        public AdoptionStatusEnum Status { get; set; }
        
        // Ajout de AnimalName et ContactName pour faciliter affichage
        public string? AnimalName { get; set; }
        public string? ContactName { get; set; }
    }
}
