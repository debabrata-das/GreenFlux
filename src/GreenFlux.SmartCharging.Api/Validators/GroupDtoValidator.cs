using FluentValidation;
using GreenFlux.SmartCharging.Api.AutoMapper;

namespace GreenFlux.SmartCharging.Api.Validators
{
    public class GroupDtoValidator : AbstractValidator<GroupDTO>
    {
        public GroupDtoValidator()
        {
            RuleFor(dto => dto.Name)
                .NotNull()
                .NotEmpty()
                .WithMessage("The Name value can not be null or empty");

            RuleFor(dto => dto.Identifier)
                .NotNull()
                .NotEmpty()
                .WithMessage("The Identifier value can not be null or empty");

            RuleFor(dto => dto.Capacity)
                .NotNull()
                .NotEmpty()
                .WithMessage("The capacity can not be null or empty");

            RuleFor(dto => dto.Capacity)
                .GreaterThan(0)
                .WithMessage("The capacity cannot be smaller than 0.");

            RuleFor(dto => dto.Capacity)
                .LessThan(float.MaxValue)
                .WithMessage($"The capacity must be less than {float.MaxValue}");
        }
    }
}