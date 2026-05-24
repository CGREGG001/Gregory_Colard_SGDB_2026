using AnimalShelter.Core.Constants;
using AnimalShelter.Core.Enums;
using AnimalShelter.Core.Exceptions;
using AnimalShelter.Core.Models;

namespace AnimalShelter.BLL.Validators
{
    public class AnimalValidator
    {
        public static void Validate(Animal animal)
        {
            // Validation des longueurs champs DB
            if (string.IsNullOrWhiteSpace(animal.Name))
                throw new ShelterException(ExceptionMessages.AnimalNameRequired, ErrorTypeEnum.ValidationError);

            if (animal.Name.Length > 100)
                throw new ShelterException(ExceptionMessages.NameTooLong, ErrorTypeEnum.ValidationError);

            if (animal.Colors?.Length > 100)
                throw new ShelterException(ExceptionMessages.ColorsTooLong, ErrorTypeEnum.ValidationError);

            // Validation chronologique des dates
            DateTime now = DateTime.Now;

            if (animal.BirthDate > now)
                throw new ShelterException(ExceptionMessages.BirthDateInFuture, ErrorTypeEnum.ValidationError);

            if (animal.SterilisationDate > now)
                throw new ShelterException(ExceptionMessages.SterilizationInFuture, ErrorTypeEnum.ValidationError);

            if (animal.DeathDate > now)
                throw new ShelterException(ExceptionMessages.DeathInFuture, ErrorTypeEnum.ValidationError);

            // Comparaisons entre dates
            if (animal.BirthDate.HasValue)
            {
                if (animal.SterilisationDate.HasValue && animal.SterilisationDate < animal.BirthDate)
                    throw new ShelterException(ExceptionMessages.SterilizationBeforeBirth, ErrorTypeEnum.ValidationError);

                if (animal.DeathDate.HasValue && animal.DeathDate < animal.BirthDate)
                    throw new ShelterException(ExceptionMessages.DeathBeforeBirth, ErrorTypeEnum.ValidationError);
            }
        }
    }
}
