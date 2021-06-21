using FluentValidation;
using GreenFlux.SmartCharging.Api.AutoMapper;

namespace GreenFlux.SmartCharging.Api.Validators
{
    public class ChargeStationDtoValidator : AbstractValidator<ChargeStationDTO>
    {
        public ChargeStationDtoValidator()
        {
            RuleFor(dto => dto.Name)
                .NotNull()
                .NotEmpty()
                .WithMessage("The name can not be null or empty");

            RuleFor(dto => dto.Identifier)
                .NotNull()
                .NotEmpty()
                .WithMessage("The Identifier value can not be null or empty");
        }
    }
}