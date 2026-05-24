using AnimalShelter.Core.Enums;

namespace AnimalShelter.Core.Models
{
    public class IntakeHistory : BaseEntity<Guid>
    {
        public string AnimalId { get; set; } = string.Empty;
        public Guid? ContactId { get; set; } // nullable si errant
        public DateTime IntaleDate { get; set; }
        public IntakeReasonEnum Reason { get; set; }
    }
}
