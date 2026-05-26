using AnimalShelter.Core.Models;
using AnimalShelter.Core.Exceptions;
using AnimalShelter.Core.Enums;
using AnimalShelter.Core.Constants;

namespace AnimalShelter.BLL.Validators;

public static class CompatibilityValidator
{
    public static void Validate(Compatibility compat)
    {
        if (string.IsNullOrWhiteSpace(compat.AnimalId))
        {
            throw new ShelterException(ExceptionMessages.InvalidId, ErrorTypeEnum.ValidationError);
        }

        // Les enums sont gérés par le ConsoleHelper, mais on valide par sécurité
        if (!Enum.IsDefined(compat.TargetType))
        {
            throw new ShelterException(ExceptionMessages.CompatibilityTypeRequired, ErrorTypeEnum.ValidationError);
        }

        if (!Enum.IsDefined(compat.ValueEnum))
        {
            throw new ShelterException(ExceptionMessages.CompatibilityValueRequired, ErrorTypeEnum.ValidationError);
        }
    }
}
