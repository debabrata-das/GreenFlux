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
    public class SaveChargeStationHandler : IRequestHandler<SaveChargeStation, SaveChargeStationOutput>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _loggerManager;

        public SaveChargeStationHandler(IUnitOfWork unitOfWork, IMapper mapper, ILoggerManager loggerManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _loggerManager = loggerManager;
        }

        public async Task<SaveChargeStationOutput> Handle(SaveChargeStation request, CancellationToken cancellationToken)
        {
            List<ValidationException> errors = await ValidateModel(request);
            if (errors.Any())
            {
                throw new DataValidationException("Charge Station validation failed", new AggregateException(errors));
            }

            try
            {
                return await SaveChargeStation(request);
            }
            catch (Exception exc)
            {
                _loggerManager.Error($"Exception while creating Charge Station! \r\n{exc}");
                throw;
            }
        }

        private async Task<SaveChargeStationOutput> SaveChargeStation(SaveChargeStation request)
        {
            _loggerManager.Info($"Trying to save ChargeStation {request.Identifier} for group {request.GroupIdentifier} (is from post - {request.FromPost}");
            var chargeStationToCreate = _mapper.Map<ChargeStation>(request);

            var group = await _unitOfWork.GroupRepository.GetByIdentifier(chargeStationToCreate.GroupIdentifier);
            if (group == null)
            {
                throw new DataValidationException($"Cannot create Charge Station since the Group with Identifier '{chargeStationToCreate.GroupIdentifier}' not found", null);
            }

            var existingChargeStation = await _unitOfWork.ChargeStationRepository.GetByIdentifier(chargeStationToCreate.Identifier);
            if (existingChargeStation != null)
            {
                if (request.FromPost)
                {
                    throw new CannotAddDuplicateEntityException("ChargeStation", request.Identifier);
                }

                existingChargeStation.Identifier = request.Identifier;
                existingChargeStation.Name = request.Name;
                existingChargeStation.Group = group;
                existingChargeStation.GroupIdentifier = request.GroupIdentifier;
                group.ChargeStations.Remove(existingChargeStation);
                group.ChargeStations.Add(chargeStationToCreate);
            }
            else
            {
                chargeStationToCreate.Group = group;
                group.ChargeStations.Add(chargeStationToCreate);
                await _unitOfWork.ChargeStationRepository.Add(chargeStationToCreate);
            }

            await _unitOfWork.SaveAsync();

            var dtoOut = _mapper.Map<ChargeStation, ChargeStationDTO>(existingChargeStation ?? chargeStationToCreate);

            return new SaveChargeStationOutput() {ChargeStationDto = dtoOut};
        }

        private static async Task<List<ValidationException>> ValidateModel(SaveChargeStation request)
        {
            var validator = new ChargeStationDtoValidator();
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
