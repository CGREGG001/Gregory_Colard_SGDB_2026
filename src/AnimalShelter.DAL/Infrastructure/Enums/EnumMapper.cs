using AnimalShelter.Core.Enums;
using AnimalShelter.DAL.Infrastructure.Interfaces;
using Npgsql;

namespace AnimalShelter.DAL.Infrastructure.Enums
{
    public class EnumMapper : IEnumMapper
    {
        public void MapEnums(NpgsqlDataSourceBuilder builder)
        {
            builder.MapEnum<SpeciesEnum>("species_enum");
            builder.MapEnum<SexEnum>("sex_enum");
            builder.MapEnum<AnimalStatusEnum>("animal_status_enum");
            builder.MapEnum<CompatibilityTypeEnum>("compatibility_type_enum");
            builder.MapEnum<CompatibilityValueEnum>("compatibility_value_enum");
            builder.MapEnum<IntakeReasonEnum>("intake_reason_enum");
            builder.MapEnum<ExitReasonEnum>("exit_reason_enum");
            builder.MapEnum<AdoptionStatusEnum>("adoption_status_enum");
        }
    }
}
