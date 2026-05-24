namespace AnimalShelter.Core.Constants;

public static class ExceptionMessages
{
    // Validation de base
    public const string AnimalNameRequired = "The animal's name is mandatory.";
    public const string NameTooLong = "The name cannot exceed 100 characters.";
    public const string ColorsTooLong = "The colors description cannot exceed 100 characters.";

    // Cohérence des dates
    public const string BirthDateInFuture = "The birth date cannot be in the future.";
    public const string SterilizationInFuture = "The sterilization date cannot be in the future.";
    public const string DeathInFuture = "The death date cannot be in the future.";
    public const string SterilizationBeforeBirth = "The sterilization date cannot be before the birth date.";
    public const string DeathBeforeBirth = "The death date cannot be before the birth date.";
    public const string InvalidDateRegression = "The provided dates are inconsistent with the existing record.";

    // État et existence
    public const string AnimalNotFound = "The specified animal was not found.";
    public const string AnimalAlreadyExists = "An animal with the same characteristics already exists in the system.";
    public const string CannotUpdateDeadAnimal = "Cannot update an animal that is already marked as deceased.";
    public const string CannotModifyDeadAnimal = "A deceased animal cannot be modified.";

    // Technique
    public const string DatabaseError = "A technical error occurred with the database.";
    public const string InvalidId = "The provided ID format is invalid.";

    // Contacts
    public const string ContactLastNameRequired = "Last name is mandatory.";
    public const string ContactFirstNameRequired = "First name is mandatory.";
    public const string InvalidEmail = "The email address format is invalid.";
    public const string InvalidPhone = "The phone or GSM number is invalid.";
    public const string AtLeastOneRoleRequired = "The contact must have at least one role assigned.";
    public const string ContactNotFound = "The specified contact was not found.";
    public const string NationalRegisterRequired = "The National Register number is required for this role.";
    public const string NationalRegisterAlreadyExists = "This National Register number is already registered.";
}
