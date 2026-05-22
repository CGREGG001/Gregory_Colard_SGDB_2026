using AnimalShelter.Core.Enums;

namespace AnimalShelter.Core.Models
{
    public class AdoptionFile : BaseEntity<Guid>
    {
        public string AnimalId { get; set; } = string.Empty;
        public Guid PersonId { get; set; }
        public DateTime RequestDate { get; set; }
        public AdoptionStatusEnum Status { get; set; }
    }
}
