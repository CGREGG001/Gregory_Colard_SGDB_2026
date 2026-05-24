using AnimalShelter.Core.Enums;

namespace AnimalShelter.Core.Models
{
    public class ExitHistory : BaseEntity<Guid>
    {
        public string AnimalId { get; set; } = string.Empty;
        public Guid? ContacId { get; set; } // nullable si décès
        public DateTime ExitDate { get; set; }
        public ExitReasonEnum ExitReason { get; set; }
    }
}
