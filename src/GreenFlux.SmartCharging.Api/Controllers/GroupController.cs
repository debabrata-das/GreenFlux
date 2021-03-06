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
    public class GroupController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GroupController(IUnitOfWork unitOfWork, ILoggerManager logger, IMapper mapper, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _mediator = mediator;
        }

        /// <summary>
        /// Get Group by its identifier
        /// </summary>
        /// <param name="identifier"><see cref="Group.Identifier">Group identifier</see></param>
        /// <returns><see cref="Group"/></returns>
        [HttpGet("{identifier:guid}")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<GroupDTO>> GetGroupByIdentifier(Guid identifier)
        {
            try
            {
                var group = await _unitOfWork.GroupRepository.GetByIdentifier(identifier);
                if (group == null)
                {
                    return NotFound();
                }

                _logger.Debug($"Found Groups with identifier - {identifier}");
                var groupResource = _mapper.Map<Group, GroupDTO>(group);
                return Ok(groupResource);
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }
        }

        /// <summary>
        /// Get the Total Current in Amps for a Group
        /// </summary>
        /// <param name="identifier"><see cref="Group.Identifier">Group identifier</see></param>
        /// <returns>Total Current in Amps for a <see cref="Group">Group</see></returns>
        [HttpGet("{identifier:guid}/TotalMaxCurrentInAmps")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<float>> GetTotalCurrentInAmps(Guid identifier)
        {
            try
            {
                var group = await _unitOfWork.GroupRepository.GetByIdentifier(identifier);
                if (group == null)
                {
                    return NotFound();
                }

                _logger.Debug($"Found ChargeStation with identifier - {identifier}");


                var maxCurrentInAmps = await _unitOfWork.GroupRepository.GetMaxCurrentInAmps(group);
                return Ok(maxCurrentInAmps);
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }
        }

        /// <summary>
        /// Creates a Group
        /// </summary>
        /// <returns>Created Group</returns>
        [HttpPost("")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<GroupDTO>> CreateGroup(SaveGroup command)
        {
            try
            {
                command.FromPost = true;
                var commandOutput = await _mediator.Send(command);
                _logger.Info($"Group '{commandOutput.GroupDto.Name}' with Identifier '{commandOutput.GroupDto.Identifier}' has been created");
                return Ok(commandOutput.GroupDto);
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }
        }

        /// <summary>
        /// Updates a Group
        /// </summary>
        /// <returns>Updated Group</returns>
        [HttpPatch("")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<GroupDTO>> UpdateGroup(SaveGroup command)
        {
            try
            {
                var commandOutput = await _mediator.Send(command);
                _logger.Info($"Group '{commandOutput.GroupDto.Name}' with Identifier '{commandOutput.GroupDto.Identifier}' has been updated");
                return Ok(commandOutput.GroupDto);
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }
        }

        /// <summary>
        /// Removing <see cref="Group">Group</see> 
        /// </summary>
        /// <param name="identifier"><see cref="Group.Identifier">Group Identifier</see></param>
        [HttpDelete("{identifier:guid}")]
        public async Task<ActionResult> RemoveGroup(Guid identifier)
        {
            try
            {
                await _unitOfWork.RemoveGroup(identifier);
                return Ok();
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }
        }
    }
}
