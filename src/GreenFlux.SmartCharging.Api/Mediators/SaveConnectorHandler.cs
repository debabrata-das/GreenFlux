using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using GreenFlux.SmartCharging.Api.AutoMapper;
using GreenFlux.SmartCharging.Api.Exceptions;
using GreenFlux.SmartCharging.Api.Validators;
using GreenFlux.SmartCharging.Domain.Models;
using GreenFlux.SmartCharging.Persistence.Repository;
using MediatR;

namespace GreenFlux.SmartCharging.Api.Mediators
{
    public class SaveConnectorHandler : IRequestHandler<SaveConnector, SaveConnectorOutput>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _loggerManager;

        public SaveConnectorHandler(IUnitOfWork unitOfWork, IMapper mapper, ILoggerManager loggerManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _loggerManager = loggerManager;
        }

        public async Task<SaveConnectorOutput> Handle(SaveConnector request, CancellationToken cancellationToken)
        {
            List<ValidationException> errors = await ValidateModel(request);
            if (errors.Any())
            {
                throw new DataValidationException("Connector validation failed", new AggregateException(errors));
            }

            try
            {
                var newGroupDto = await SaveConnector(request);

                return new SaveConnectorOutput() { ConnectorDto = newGroupDto };
            }
            catch (Exception exc)
            {
                _loggerManager.Error($"Exception while creating Group! \r\n{exc}");
                throw;
            }
        }

        private async Task<ConnectorDTO> SaveConnector(SaveConnector request)
        {
            var chargeStation = await _unitOfWork.ChargeStationRepository.GetByIdentifier(request.ChargeStationIdentifier);
            if (chargeStation == null)
            {
                throw new DataValidationException($"Cannot create Connector since the Charge Station with Identifier '{request.ChargeStationIdentifier}' not found", null);
            }

            var group = await _unitOfWork.GroupRepository.GetByIdentifier(chargeStation.GroupIdentifier);
            if (group == null)
            {
                throw new DataValidationException($"Cannot create Connector since the Group with Identifier '{chargeStation.GroupIdentifier}' not found", null);
            }

            var connectorToSave = _mapper.Map<Connector>(request);

            await SaveConnector(group, connectorToSave, chargeStation, request.FromPost);

            var newGroupDto = _mapper.Map<Connector, ConnectorDTO>(connectorToSave);
            return newGroupDto;
        }

        private async Task SaveConnector(Group group, Connector connectorToSave, ChargeStation chargeStation, bool fromPost)
        {
            _loggerManager.Info($"In SaveConnector trying to save {connectorToSave} for group {group} (is from post - {fromPost}");
            var capacity = group.Capacity;
            var capacityToBeAdded = connectorToSave.MaxCurrentInAmps;
            var connector = await _unitOfWork.ConnectorRepository.GetByIdentifierAndChargeStation(connectorToSave.Identifier, chargeStation.Identifier);
            // if existing connector then subtract to get the added new capacity
            if (connector != null)
            {
                capacityToBeAdded = System.Math.Abs(capacityToBeAdded - connector.MaxCurrentInAmps);
            }

            (bool canAddToGroupCapacity, float total) = await _unitOfWork.CheckIfCanAddToCurrentCapacityAndGetCurrentTotalMaxAmpsForGroup(group, capacityToBeAdded);
            if (!canAddToGroupCapacity)
            {
                throw new ConnectorCapacityExceededForGroupException(capacity, total, capacityToBeAdded);
            }

            if (connector != null)
            {
                if (fromPost)
                {
                    throw new CannotAddDuplicateEntityException("Connector in ChargeStation", connectorToSave.Identifier);
                }

                connector.MaxCurrentInAmps = connectorToSave.MaxCurrentInAmps;
                connector.ChargeStation = chargeStation;
                connector.Identifier = connectorToSave.Identifier;
                await _unitOfWork.SaveAsync();
                _loggerManager.Debug($"SaveConnector updated {connector}");
            }
            else
            {
                connectorToSave.ChargeStation = chargeStation;
                await _unitOfWork.ConnectorRepository.AddConnector(connectorToSave);
                _loggerManager.Debug($"SaveConnector inserted {connectorToSave}");
            }
        }

        private static async Task<List<ValidationException>> ValidateModel(SaveConnector request)
        {
            var validator = new ConnectorDtoValidator();
            var validationResult = await validator.ValidateAsync(request);
            List<ValidationException> validationErrors = new List<ValidationException>();
            if (!validationResult.IsValid)
            {
                validationErrors.AddRange(validationResult.Errors.Select(failure => new ValidationException(failure.ErrorMessage)));
            }

            return validationErrors;
        }
    }
}
