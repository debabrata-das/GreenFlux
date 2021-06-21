using System.Collections.Generic;
using FluentValidation;
using GreenFlux.SmartCharging.Api.AutoMapper;

namespace GreenFlux.SmartCharging.Api.Validators
{
    public class ConnectorDtoValidator : AbstractValidator<ConnectorDTO>
    {
        readonly List<int> _possibleIdentifierValue = new List<int>() { 1, 2, 3, 4, 5};
        public ConnectorDtoValidator()
        {
            RuleFor(dto => dto.Identifier)
                .Must(item => _possibleIdentifierValue.Contains(item))
                .WithMessage("For a Connector the only possible values for Identifier are - " + string.Join(", ", _possibleIdentifierValue));

            RuleFor(dto => dto.ChargeStationIdentifier)
                .NotNull()
                .NotEmpty()
                .WithMessage("For a Connector the Charge Station Identifier value can not be null or empty");

            RuleFor(dto => dto.MaxCurrentInAmps)
                .GreaterThan(0)
                .WithMessage("For a Connector the maximum current in amps should be greater than 0");
        }
    }
}