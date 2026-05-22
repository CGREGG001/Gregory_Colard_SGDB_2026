namespace AnimalShelter.Core.Enums
{
    [Flags]
    public enum ContactRolesEnum : short
    {
        None = 0,
        Volunteer = 1,
        Adopter = 2,
        Candidate = 4,
        Other = 8
    }
}
