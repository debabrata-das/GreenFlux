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
    public class SaveGroupHandler : IRequestHandler<SaveGroup, SaveGroupOutput>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _loggerManager;

        public SaveGroupHandler(IUnitOfWork unitOfWork, IMapper mapper, ILoggerManager loggerManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _loggerManager = loggerManager;
        }

        public async Task<SaveGroupOutput> Handle(SaveGroup request, CancellationToken cancellationToken)
        {
            List<ValidationException> errors = await ValidateModel(request);
            if (errors.Any())
            {
                throw new DataValidationException("Group validation failed", new AggregateException(errors));
            }

            try
            {
                var newGroupDto = await SaveGroup(request);

                return new SaveGroupOutput() {GroupDto = newGroupDto};
            }
            catch (Exception exc)
            {
                _loggerManager.Error($"Exception while creating Group! \r\n{exc}");
                throw;
            }
        }

        private async Task<GroupDTO> SaveGroup(SaveGroup request)
        {
            var groupToCreate = _mapper.Map<Group>(request);
            _loggerManager.Info($"Trying to save Group with identifier '{request.Identifier}' with name '{request.Name}' (is from post - {request.FromPost}");

            var existingGroup = await _unitOfWork.GroupRepository.GetByIdentifier(groupToCreate.Identifier);
            if (existingGroup != null)
            {
                if (request.FromPost)
                {
                    throw new CannotAddDuplicateEntityException("Group", request.Identifier);
                }

                await _unitOfWork.CheckGroupCapacity(existingGroup, request.Capacity);

                existingGroup.Capacity = request.Capacity;
                existingGroup.Identifier = request.Identifier;
                existingGroup.Name = request.Name;
                // update linked ChargeStations
                foreach (var chargeStation in existingGroup.ChargeStations)
                {
                    chargeStation.Group = existingGroup;
                }
            }
            else
            {
                await _unitOfWork.GroupRepository.AddGroup(groupToCreate);
            }

            await _unitOfWork.SaveAsync();
            var newGroupDto = _mapper.Map<Group, GroupDTO>(existingGroup ?? groupToCreate);
            return newGroupDto;
        }

        private static async Task<List<ValidationException>> ValidateModel(GroupDTO groupDto)
        {
            var validator = new GroupDtoValidator();
            var validationResult = await validator.ValidateAsync(groupDto);
            List<ValidationException> validationErrors = new List<ValidationException>();
            if (!validationResult.IsValid)
            {
                validationErrors.AddRange(validationResult.Errors.Select(failure => new ValidationException(failure.ErrorMessage)));
            }

            return validationErrors;
        }
    }
}
