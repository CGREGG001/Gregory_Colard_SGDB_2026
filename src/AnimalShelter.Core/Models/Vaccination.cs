namespace AnimalShelter.Core.Models
{
    public class Vaccination : BaseEntity<Guid>
    {
        public string AnimalId { get; set; } = string.Empty;
        public string VaccineName { get; set; } = string.Empty;
        public DateTime VaccineDate { get; set; }
        public bool IsDone { get; set; }
    }
}
