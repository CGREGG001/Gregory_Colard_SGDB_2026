using System.Text.RegularExpressions;
using AnimalShelter.BLL.Helpers;
using AnimalShelter.Core.Constants;
using AnimalShelter.Core.Enums;
using AnimalShelter.Core.Exceptions;
using AnimalShelter.Core.Models;

namespace AnimalShelter.BLL.Validators
{
    public static partial class ContactValidator
    {
        /// <summary>
        /// Ensures that contacts with specific roles (Adopter or Volunteer)
        /// provide a valid Belgian National Register number.
        /// The encrypted value is decrypted before format and checksum validation.
        /// </summary>
        /// <exception cref="ShelterException">
        /// Thrown when the national register is missing or invalid for the required roles.
        /// </exception>
        public static void Validate(Contact contact)
        {
            if (string.IsNullOrWhiteSpace(contact.LastName))
                throw new ShelterException(ExceptionMessages.ContactLastNameRequired, ErrorTypeEnum.ValidationError);

            if (string.IsNullOrWhiteSpace(contact.FirstName))
                throw new ShelterException(ExceptionMessages.ContactFirstNameRequired, ErrorTypeEnum.ValidationError);

            if (!string.IsNullOrWhiteSpace(contact.Email) &&
                !MyRegex().IsMatch(contact.Email))
            {
                throw new ShelterException(ExceptionMessages.InvalidEmail, ErrorTypeEnum.ValidationError);
            }

            if (contact.RoleFlags == ContactRolesEnum.None)
                throw new ShelterException(ExceptionMessages.AtLeastOneRoleRequired, ErrorTypeEnum.ValidationError);

            // Registre National obligatoire pour Adoptant ou Bénévole
            if ((contact.RoleFlags.HasFlag(ContactRolesEnum.Adopter) ||
                contact.RoleFlags.HasFlag(ContactRolesEnum.Volunteer)))
            {
                if (contact.NationalRegisterEncrypted == null || contact.NationalRegisterEncrypted.Length == 0)
                    throw new ShelterException(ExceptionMessages.NationalRegisterRequired, ErrorTypeEnum.ValidationError);

                // Déchiffrement pour validation du format
                string nr = EncryptionHelper.Decrypt(contact.NationalRegisterEncrypted);

                if (!IsValidNationalRegister(nr))
                    throw new ShelterException("Invalid National Register format.", ErrorTypeEnum.ValidationError);
            }
        }

        /// <summary>
        /// Validates a Belgian National Register number.
        /// Ensures the value contains exactly 11 digits and verifies the official checksum
        /// (modulo 97), supporting both pre‑2000 and post‑2000 formats.
        /// </summary>
        /// <param name="nr">The plain-text national register number.</param>
        /// <returns>
        /// True if the number is correctly formatted and passes checksum validation; otherwise false.
        /// </returns>
        private static bool IsValidNationalRegister(string nr)
        {
            // Doit contenir exactement 11 chiffres
            if (string.IsNullOrWhiteSpace(nr) || nr.Length != 11 || !nr.All(char.IsDigit))
                return false;

            // Séparation : 6 chiffres date + 3 séquence + 2 checksum
            string baseNumber = nr[..9];
            string checksumStr = nr.Substring(9, 2);

            if (!long.TryParse(baseNumber, out long baseVal) ||
                !int.TryParse(checksumStr, out int checksum))
                return false;

            // Cas 1 : personnes nées avant 2000
            int expected1 = 97 - (int)(baseVal % 97);
            if (expected1 == checksum)
                return true;

            // Cas 2 : personnes nées après 2000 → on ajoute 2 milliards
            long baseVal2000 = baseVal + 2000000000;
            int expected2 = 97 - (int)(baseVal2000 % 97);

            return expected2 == checksum;
        }

        [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
        private static partial Regex MyRegex();
    }
}
