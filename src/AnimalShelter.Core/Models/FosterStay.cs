namespace AnimalShelter.Core.Models
{
    public class FosterStay : BaseEntity<Guid>
    {
        public string AnimalId { get; set; } = string.Empty;
        public Guid PersonId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
