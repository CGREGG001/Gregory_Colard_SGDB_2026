using AnimalShelter.Core.Models;
using AnimalShelter.Core.Exceptions;
using AnimalShelter.Core.Enums;
using AnimalShelter.Core.Constants;

namespace AnimalShelter.BLL.Validators;

public static class AdoptionValidator
{
    public static void Validate(AdoptionFile file)
    {
        if (string.IsNullOrWhiteSpace(file.AnimalId))
        {
            throw new ShelterException(ExceptionMessages.AdoptionAnimalIdRequired, ErrorTypeEnum.ValidationError);
        }
        if (file.ContactId == Guid.Empty)
        {
            throw new ShelterException(ExceptionMessages.AdoptionContactRequired, ErrorTypeEnum.ValidationError);
        }
    }
}
