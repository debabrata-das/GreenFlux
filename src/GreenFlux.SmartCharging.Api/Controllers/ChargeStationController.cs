using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using AutoMapper;
using GreenFlux.SmartCharging.Api.AutoMapper;
using GreenFlux.SmartCharging.Api.Mediators;
using GreenFlux.SmartCharging.Domain.Models;
using GreenFlux.SmartCharging.Persistence.Repository;
using MediatR;

namespace GreenFlux.SmartCharging.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChargeStationController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public ChargeStationController(IUnitOfWork unitOfWork, ILoggerManager logger, IMapper mapper, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _mediator = mediator;
        }

        /// <summary>
        /// Get the Total Current in Amps for a Charge Station
        /// </summary>
        /// <param name="identifier"><see cref="ChargeStation.Identifier">ChargeStation identifier</see></param>
        /// <returns>Total Current in Amps for a <see cref="ChargeStation">Charge Station</see></returns>
        [HttpGet("{identifier:guid}/TotalMaxCurrentInAmps")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(float))]
        public async Task<ActionResult<float>> GetTotalCurrentInAmps(Guid identifier)
        {
            try
            {
                var chargeStation = await _unitOfWork.ChargeStationRepository.GetByIdentifier(identifier);
                if (chargeStation == null)
                {
                    return NotFound();
                }

                _logger.Debug($"Found ChargeStation with identifier - {identifier}");


                var maxCurrentInAmps = await _unitOfWork.ChargeStationRepository.GetMaxCurrentInAmps(chargeStation);
                return Ok(maxCurrentInAmps);
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }
        }

        /// <summary>
        /// Get Charge Station by its identifier
        /// </summary>
        /// <param name="identifier"><see cref="ChargeStation.Identifier">ChargeStation identifier</see></param>
        /// <returns><see cref="ChargeStation"/></returns>
        [HttpGet("{identifier:guid}")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ChargeStationDTO>> GetChargeStationByIdentifier(Guid identifier)
        {
            try
            {
                var chargeStation = await _unitOfWork.ChargeStationRepository.GetByIdentifier(identifier);
                if (chargeStation == null)
                {
                    return NotFound();
                }
                
                _logger.Debug($"Found ChargeStation with identifier - {identifier}");
                
                var chargeStationDto = _mapper.Map<ChargeStation, ChargeStationDTO>(chargeStation);
                return Ok(chargeStationDto);
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }
        }

        /// <summary>
        /// Create ChargeStation
        /// </summary>
        /// <returns>Created ChargeStation</returns>
        [HttpPost("")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ChargeStationDTO>> CreateChargeStation(SaveChargeStation command)
        {
            command.FromPost = true;
            var commandOutput = await _mediator.Send(command);
            _logger.Info($"Charge Station '{commandOutput.ChargeStationDto.Identifier}' for group {commandOutput.ChargeStationDto.GroupIdentifier} has been created");
            return Ok(commandOutput.ChargeStationDto);
        }

        /// <summary>
        /// Update ChargeStation
        /// </summary>
        /// <returns>Updated ChargeStation</returns>
        [HttpPatch("")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ChargeStationDTO>> UpdateChargeStation(SaveChargeStation command)
        {
            var commandOutput = await _mediator.Send(command);
            _logger.Info($"Charge Station '{commandOutput.ChargeStationDto.Identifier}' for group {commandOutput.ChargeStationDto.GroupIdentifier} has been updated");
            return Ok(commandOutput.ChargeStationDto);
        }

        /// <summary>
        /// Removing <see cref="Group">Group</see> 
        /// </summary>
        /// <param name="identifier"><see cref="Group.Identifier">Group Identifier</see></param>
        [HttpDelete("{identifier}")]
        public async Task<ActionResult> RemoveChargeStation(Guid identifier)
        {
            await _unitOfWork.RemoveChargeStation(identifier);
            return Ok();
        }
    }
}
