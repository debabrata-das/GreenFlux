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
    public class ConnectorController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;

        public ConnectorController(IUnitOfWork unitOfWork, ILoggerManager logger, IMapper mapper, IMediator mediator)
        {
            _logger = logger;
            _mapper = mapper;
            _mediator = mediator;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Get Charge Station by its identifier
        /// </summary>
        /// <param name="identifier"><see cref="Connector.Identifier">Connector identifier</see></param>
        /// <param name="chargeStationIdentifier"><see cref="ChargeStation.Identifier">ChargeStation Identifier of this Connector</see></param>
        /// <returns><see cref="Connector"/></returns>
        [HttpGet("{identifier:int}")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ConnectorDTO>> GetConnector(int identifier, Guid chargeStationIdentifier)
        {
            try
            {
                var connector = await _unitOfWork.ConnectorRepository.GetByIdentifierAndChargeStation(identifier, chargeStationIdentifier);
                if (connector == null)
                {
                    return NotFound();
                }

                _logger.Debug($"Found Connector with identifier - {identifier}");

                var connectorDto = _mapper.Map<Connector, ConnectorDTO>(connector);
                return Ok(connectorDto);
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }
        }

        /// <summary>
        /// Create Connector
        /// </summary>
        /// <returns>Created Connector</returns>
        [HttpPost("")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ConnectorDTO>> CreateConnector(SaveConnector command)
        {
            command.FromPost = true;
            var commandOutput = await _mediator.Send(command);
            _logger.Info($"Connector '{commandOutput.ConnectorDto.Identifier}' for Charge Station '{commandOutput.ConnectorDto.ChargeStationIdentifier} has been created");
            return Ok(commandOutput.ConnectorDto);
        }

        /// <summary>
        /// Update Connector
        /// </summary>
        /// <returns>Updated Connector</returns>
        [HttpPatch("")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ConnectorDTO>> UpdateConnector(SaveConnector command)
        {
            var commandOutput = await _mediator.Send(command);
            _logger.Info($"Connector '{commandOutput.ConnectorDto.Identifier}' for Charge Station '{commandOutput.ConnectorDto.ChargeStationIdentifier} has been updated");
            return Ok(commandOutput.ConnectorDto);
        }

        /// <summary>
        /// Removing <see cref="Group">Group</see> 
        /// </summary>
        /// <param name="identifier"><see cref="Connector.Identifier">Connector Identifier</see></param>
        /// <param name="chargeStationIdentifier">ChargeStation Identifier for this Connector</param>
        [HttpDelete("{identifier}")]
        public async Task<ActionResult> RemoveConnector(int identifier, Guid chargeStationIdentifier)
        {
            await _unitOfWork.ConnectorRepository.RemoveConnectorByIdentifierAndChargeStation(identifier, chargeStationIdentifier);
            await _unitOfWork.SaveAsync();
            return Ok();
        }
    }
}
