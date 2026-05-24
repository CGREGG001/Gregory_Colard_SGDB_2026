namespace AnimalShelter.Core.Models
{
    public class FosterStay : BaseEntity<Guid>
    {
        public string AnimalId { get; set; } = string.Empty;
        public Guid ContactId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        // Ajout de AnimalName et ContactName pour faciliter affichage
        public string? AnimalName { get; set; }
        public string? ContactName { get; set; }
    }
}
