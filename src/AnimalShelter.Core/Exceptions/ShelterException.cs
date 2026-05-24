using AnimalShelter.Core.Enums;

namespace AnimalShelter.Core.Exceptions
{
    public class ShelterException : Exception
    {
        public ErrorTypeEnum ErrorType { get; set; }
        public ShelterException(string message, ErrorTypeEnum errorType) : base(message)
        {
            ErrorType = errorType;
        }

        public ShelterException(string message, ErrorTypeEnum errorType, Exception innerException)
        : base(message, innerException)
        {
            ErrorType = errorType;
        }
    }
}
