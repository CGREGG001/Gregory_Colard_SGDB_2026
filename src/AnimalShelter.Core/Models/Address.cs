namespace AnimalShelter.Core.Models
{
    public class Address : BaseEntity<Guid>
    {
        public string Street { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
        public string? Box { get; set; }
        public string PostCode { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = "Belgium";
    }
}
