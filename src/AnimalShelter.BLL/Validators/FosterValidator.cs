using AnimalShelter.Core.Models;
using AnimalShelter.Core.Exceptions;
using AnimalShelter.Core.Enums;
using AnimalShelter.Core.Constants;

namespace AnimalShelter.BLL.Validators;

public static class FosterValidator
{
    public static void Validate(FosterStay stay)
    {
        if (string.IsNullOrWhiteSpace(stay.AnimalId))
        {
            throw new ShelterException(ExceptionMessages.InvalidId, ErrorTypeEnum.ValidationError);
        }

        if (stay.ContactId == Guid.Empty)
        {
            throw new ShelterException(ExceptionMessages.FosterContactRequired, ErrorTypeEnum.ValidationError);
        }

        if (stay.StartDate > DateTime.Now)
        {
            throw new ShelterException(ExceptionMessages.FosterStartDateRequired, ErrorTypeEnum.ValidationError);
        }

        if (stay.EndDate.HasValue && stay.EndDate < stay.StartDate)
        {
            throw new ShelterException(ExceptionMessages.FosterEndDateBeforeStart, ErrorTypeEnum.ValidationError);
        }
    }
}
