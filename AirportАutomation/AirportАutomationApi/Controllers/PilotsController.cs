using AirportAutomation.Api.Interfaces;
using AirportAutomation.Core.Dtos.Pilot;
using AirportAutomation.Core.Dtos.Response;
using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Interfaces.IServices;
using AirportАutomation.Api.Controllers;
using AirportАutomation.Api.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace AirportАutomationApi.Controllers
{
	[Authorize]
	[ApiVersion("1.0")]
	public class PilotsController : BaseController
	{
		private readonly IPilotService _pilotService;
		private readonly IPaginationValidationService _paginationValidationService;
		private readonly IInputValidationService _inputValidationService;
		private readonly IMapper _mapper;
		private readonly ILogger<PilotsController> _logger;
		private readonly int maxPageSize;

		public PilotsController(
			IPilotService pilotService,
			IPaginationValidationService paginationValidationService,
			IInputValidationService inputValidationService,
			IMapper mapper,
			ILogger<PilotsController> logger,
			IConfiguration configuration)
		{
			_pilotService = pilotService ?? throw new ArgumentNullException(nameof(pilotService));
			_paginationValidationService = paginationValidationService ?? throw new ArgumentNullException(nameof(paginationValidationService));
			_inputValidationService = inputValidationService ?? throw new ArgumentNullException(nameof(inputValidationService));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			maxPageSize = configuration.GetValue<int>("pageSettings:maxPageSize");
		}

		/// <summary>
		/// Endpoint for retrieving a paginated list of pilots.
		/// </summary>
		/// <param name="page">The page number for pagination (optional).</param>
		/// <param name="pageSize">The number of items per page (optional).</param>
		/// <returns>A paginated list of pilots.</returns>
		/// <response code="200">Returns a list of pilots wrapped in a <see cref="PagedResponse{PilotDto}"/>.</response>
		/// <response code="204">If no pilots are found.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpGet]
		[ProducesResponseType(200, Type = typeof(PagedResponse<PilotDto>))]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<PagedResponse<PilotDto>>> GetPilots([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
		{
			var (isValid, correctedPageSize, result) = _paginationValidationService.ValidatePaginationParameters(page, pageSize, maxPageSize);
			if (!isValid)
			{
				return result;
			}
			var pilots = await _pilotService.GetPilots(page, correctedPageSize);
			if (pilots is null || !pilots.Any())
			{
				_logger.LogInformation("Pilots not found.");
				return NoContent();
			}
			var totalItems = _pilotService.PilotsCount();
			var data = _mapper.Map<IEnumerable<PilotDto>>(pilots);
			var response = new PagedResponse<PilotDto>(data, page, correctedPageSize, totalItems);
			return Ok(response);
		}

		/// <summary>
		/// Endpoint for retrieving single pilot.
		/// </summary>
		/// <param name="id"></param>
		/// <returns>A single pilot that match the specified id.</returns>
		/// <response code="200">Returns a single pilot if any is found.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="404">If no pilot is found.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpGet("{id}")]
		[ProducesResponseType(200, Type = typeof(PilotDto))]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<PilotDto>> GetPilot(int id)
		{
			if (!_inputValidationService.IsNonNegativeInt(id))
			{
				_logger.LogInformation("Invalid input. The ID {id} must be a non-negative integer.", id);
				return BadRequest("Invalid input. The ID must be a non-negative integer.");
			}
			if (!await _pilotService.PilotExists(id))
			{
				_logger.LogInformation("Pilot with id {id} not found.", id);
				return NotFound();
			}
			var pilot = await _pilotService.GetPilot(id);
			var pilotDto = _mapper.Map<PilotDto>(pilot);
			return Ok(pilotDto);
		}

		/// <summary>
		/// Endpoint for fetching a list of pilots by first name, last name, or both.
		/// </summary>
		/// <param name="firstName"></param>
		/// <param name="lastName"></param>
		/// <returns>A list of pilots.</returns>
		/// <response code="200">Returns a list of pilots if any are found.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="404">If no pilots are found.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpGet("byName")]
		[ProducesResponseType(200, Type = typeof(IEnumerable<PilotDto>))]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<IEnumerable<PilotDto>>> GetPilotsByName(
			[FromQuery] string? firstName = null,
			[FromQuery] string? lastName = null)
		{
			if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
			{
				_logger.LogInformation("Both first name and last name are missing in the request.");
				return BadRequest("Both first name and last name are missing in the request.");
			}
			var pilots = await _pilotService.GetPilotsByName(firstName, lastName);
			if (pilots == null || pilots.Count == 0)
			{
				_logger.LogInformation("Pilots not found.");
				return NotFound();
			}
			var pilotsDto = _mapper.Map<IEnumerable<PilotDto>>(pilots);
			return Ok(pilotsDto);
		}

		/// <summary>
		/// Endpoint for creating a new pilot.
		/// </summary>
		/// <param name="pilotCreateDto"></param>
		/// <returns>The created pilot.</returns>
		/// <response code="201">Returns the created pilot if successful.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpPost]
		[ProducesResponseType(201, Type = typeof(PilotEntity))]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<PilotEntity>> PostPilot(PilotCreateDto pilotCreateDto)
		{
			var pilot = _mapper.Map<PilotEntity>(pilotCreateDto);
			await _pilotService.PostPilot(pilot);
			return CreatedAtAction("GetPilot", new { id = pilot.Id }, pilot);
		}

		/// <summary>
		/// Endpoint for updating a single pilot.
		/// </summary>
		/// <param name="id">The ID of the pilot to update.</param>
		/// <param name="pilotDto">The updated pilot data.</param>
		/// <response code="204">No content. The update was successful.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="404">If the pilot with the specified ID is not found.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpPut("{id}")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		public async Task<IActionResult> PutPilot(int id, PilotDto pilotDto)
		{
			if (!_inputValidationService.IsNonNegativeInt(id))
			{
				_logger.LogInformation("Invalid input. The ID {id} must be a non-negative integer.", id);
				return BadRequest("Invalid input. The ID must be a non-negative integer.");
			}
			if (id != pilotDto.Id)
			{
				_logger.LogInformation("Pilot with id {id} is different from provided Pilot and his id.", id);
				return BadRequest();
			}
			if (!await _pilotService.PilotExists(id))
			{
				_logger.LogInformation("Pilot with id {id} not found.", id);
				return NotFound();
			}
			var pilot = _mapper.Map<PilotEntity>(pilotDto);
			await _pilotService.PutPilot(pilot);
			return NoContent();
		}

		/// <summary>
		/// Endpoint for partially updating a single pilot.
		/// </summary>
		/// <param name="id">The ID of the pilot to partially update.</param>
		/// <param name="pilotDocument">The JSON document containing the partial update instructions.</param>
		/// <remarks>
		/// The JSON document should follow the JSON Patch standard (RFC 6902) and contain one or more operations.
		/// Example operation:
		/// {
		///     "op": "replace",
		///     "path": "/Name",
		///     "value": "NewName"
		/// }
		/// </remarks>
		/// <response code="200">The partial update was successful.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="404">If the pilot with the specified ID is not found.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpPatch("{id}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		public async Task<IActionResult> PatchPilot(int id, [FromBody] JsonPatchDocument pilotDocument)
		{
			if (!_inputValidationService.IsNonNegativeInt(id))
			{
				_logger.LogInformation("Invalid input. The ID {id} must be a non-negative integer.", id);
				return BadRequest("Invalid input. The ID must be a non-negative integer.");
			}
			if (!await _pilotService.PilotExists(id))
			{
				_logger.LogInformation("Pilot with id {id} not found.", id);
				return NotFound();
			}
			var updatedPilot = await _pilotService.PatchPilot(id, pilotDocument);
			return Ok(updatedPilot);
		}

		/// <summary>
		/// Endpoint for deleting a single pilot by ID.
		/// </summary>
		/// <param name="id">The ID of the pilot to delete.</param>
		/// <response code="204">No content. The deletion was successful.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="404">If the pilot with the specified ID is not found.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		/// <response code="409">Conflict. If the pilot cannot be deleted because it is being referenced by other entities.</response>
		[HttpDelete("{id}")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		[ProducesResponseType(409)]
		public async Task<IActionResult> DeletePilot(int id)
		{
			if (!_inputValidationService.IsNonNegativeInt(id))
			{
				_logger.LogInformation("Invalid input. The ID {id} must be a non-negative integer.", id);
				return BadRequest("Invalid input. The ID must be a non-negative integer.");
			}
			if (!await _pilotService.PilotExists(id))
			{
				_logger.LogInformation("Pilot with id {id} not found.", id);
				return NotFound();
			}
			bool deleted = await _pilotService.DeletePilot(id);
			if (deleted)
			{
				return NoContent();
			}
			else
			{
				_logger.LogInformation("Pilot with id {id} is being referenced by other entities and cannot be deleted.", id);
				return Conflict("Pilot cannot be deleted because it is being referenced by other entities.");
			}
		}

	}
}
