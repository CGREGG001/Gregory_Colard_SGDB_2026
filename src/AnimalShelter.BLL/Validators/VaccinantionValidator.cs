using AnimalShelter.Core.Models;
using AnimalShelter.Core.Exceptions;
using AnimalShelter.Core.Enums;

namespace AnimalShelter.BLL.Validators;

public static class VaccinationValidator
{
    public static void Validate(Vaccination v)
    {
        if (string.IsNullOrWhiteSpace(v.VaccineName))
            throw new ShelterException("Vaccine name is required.", ErrorTypeEnum.ValidationError);

        if (v.VaccineDate > DateTime.Now)
            throw new ShelterException("Vaccine date cannot be in the future.", ErrorTypeEnum.ValidationError);
    }
}
