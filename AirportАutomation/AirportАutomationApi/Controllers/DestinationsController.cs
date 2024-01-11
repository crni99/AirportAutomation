using AirportAutomation.Api.Interfaces;
using AirportAutomation.Core.Dtos.Destination;
using AirportAutomation.Core.Dtos.Response;
using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Interfaces.IServices;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace AirportАutomationApi.Controllers
{
	[Authorize]
	[ApiVersion("1.0")]
	[Route("api/[controller]")]
	[ApiController]
	public class DestinationsController : ControllerBase
	{
		private readonly IDestinationService _destinationService;
		private readonly IPaginationValidationService _paginationValidationService;
		private readonly IMapper _mapper;
		private readonly ILogger<DestinationsController> _logger;
		private readonly int maxPageSize;

		public DestinationsController(
			IDestinationService destinationService,
			IPaginationValidationService paginationValidationService,
			IMapper mapper,
			ILogger<DestinationsController> logger,
			IConfiguration configuration)
		{
			_destinationService = destinationService ?? throw new ArgumentNullException(nameof(destinationService));
			_paginationValidationService = paginationValidationService ?? throw new ArgumentNullException(nameof(paginationValidationService));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			maxPageSize = configuration.GetValue<int>("pageSettings:maxPageSize");
		}

		/// <summary>
		/// Endpoint for retrieving a paginated list of destinations.
		/// </summary>
		/// <param name="page">The page number for pagination (optional).</param>
		/// <param name="pageSize">The number of items per page (optional).</param>
		/// <returns>A paginated list of destinations.</returns>
		/// <response code="200">Returns a list of destinations wrapped in a <see cref="PagedResponse{DestinationDto}"/>.</response>
		/// <response code="204">If no destinations are found.</response>
		/// <response code="400">If the request is invalid.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpGet]
		[ProducesResponseType(200, Type = typeof(PagedResponse<DestinationDto>))]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<PagedResponse<DestinationDto>>> GetDestinations([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
		{
			var (isValid, correctedPageSize, result) = _paginationValidationService.ValidatePaginationParameters(page, pageSize, maxPageSize);
			if (!isValid)
			{
				return result;
			}
			var destinations = await _destinationService.GetDestinations(page, correctedPageSize);
			if (destinations is null || !destinations.Any())
			{
				_logger.LogInformation("Destinations not found.");
				return NoContent();
			}
			var totalItems = _destinationService.DestinationsCount();
			var data = _mapper.Map<IEnumerable<DestinationDto>>(destinations);
			var response = new PagedResponse<DestinationDto>(data, page, correctedPageSize, totalItems);
			return Ok(response);
		}

		/// <summary>
		/// Endpoint for retrieving single destination.
		/// </summary>
		/// <param name="id"></param>
		/// <returns>A single destination that match the specified id.</returns>
		/// <response code="200">Returns a single destination if any is found.</response>
		/// <response code="404">If no destination is found.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpGet("{id}")]
		[ProducesResponseType(200, Type = typeof(DestinationDto))]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<DestinationDto>> GetDestination(int id)
		{
			if (!await _destinationService.DestinationExists(id))
			{
				_logger.LogInformation("Destination with id {id} not found.", id);
				return NotFound();
			}
			var destination = await _destinationService.GetDestination(id);
			var destinationDto = _mapper.Map<DestinationDto>(destination);
			return Ok(destinationDto);
		}

		/// <summary>
		/// Endpoint for creating a new destination.
		/// </summary>
		/// <param name="destinationCreateDto"></param>
		/// <returns>The created destination.</returns>
		/// <response code="201">Returns the created destination if successful.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpPost]
		[ProducesResponseType(201, Type = typeof(DestinationCreateDto))]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<DestinationDto>> PostDestination(DestinationCreateDto destinationCreateDto)
		{
			var destination = _mapper.Map<DestinationEntity>(destinationCreateDto);
			await _destinationService.PostDestination(destination);
			return CreatedAtAction("GetDestination", new { id = destination.Id }, destination);
		}

		/// <summary>
		/// Endpoint for updating a single destination.
		/// </summary>
		/// <param name="id">The ID of the destination to update.</param>
		/// <param name="destinationDto">The updated destination data.</param>
		/// <response code="204">No content. The update was successful.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="404">If the destination with the specified ID is not found.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpPut("{id}")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		public async Task<IActionResult> PutDestination(int id, DestinationDto destinationDto)
		{
			if (id != destinationDto.Id)
			{
				_logger.LogInformation("Destination with id {id} is different from provided Destination and his id.", id);
				return BadRequest();
			}
			if (!await _destinationService.DestinationExists(id))
			{
				_logger.LogInformation("Destination with id {id} not found.", id);
				return NotFound();
			}
			var destination = _mapper.Map<DestinationEntity>(destinationDto);
			await _destinationService.PutDestination(destination);
			return NoContent();
		}

		/// <summary>
		/// Endpoint for partially updating a single destination.
		/// </summary>
		/// <param name="id">The ID of the destination to partially update.</param>
		/// <param name="destinationDocument">The JSON document containing the partial update instructions.</param>
		/// <remarks>
		/// The JSON document should follow the JSON Patch standard (RFC 6902) and contain one or more operations.
		/// Example operation:
		/// {
		///     "op": "replace",
		///     "path": "/Name",
		///     "value": "NewName"
		/// }
		/// </remarks>
		/// <response code="204">No content. The partial update was successful.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="404">If the destination with the specified ID is not found.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpPatch("{id}")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		public async Task<IActionResult> PatchDestination(int id, [FromBody] JsonPatchDocument destinationDocument)
		{
			if (!await _destinationService.DestinationExists(id))
			{
				_logger.LogInformation("Destination with id {id} not found.", id);
				return NotFound();
			}
			var updatedDestination = await _destinationService.PatchDestination(id, destinationDocument);
			return Ok(updatedDestination);
		}

		/// <summary>
		/// Endpoint for deleting a single destination by ID.
		/// </summary>
		/// <param name="id">The ID of the destination to delete.</param>
		/// <response code="204">No content. The deletion was successful.</response>
		/// <response code="404">If the destination with the specified ID is not found.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		/// <response code="409">Conflict. If the passenger cannot be deleted because it is being referenced by other entities.</response>
		[HttpDelete("{id}")]
		[ProducesResponseType(204)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		[ProducesResponseType(409)]
		public async Task<IActionResult> DeleteDestination(int id)
		{
			if (!await _destinationService.DestinationExists(id))
			{
				_logger.LogInformation("Destination with id {id} not found.", id);
				return NotFound();
			}
			bool deleted = await _destinationService.DeleteDestination(id);
			if (deleted)
			{
				return NoContent();
			}
			else
			{
				_logger.LogInformation("Destination with id {id} is being referenced by other entities and cannot be deleted.", id);
				return Conflict("Destination cannot be deleted because it is being referenced by other entities.");
			}
		}

	}
}

