using AirportAutomation.Application.Interfaces.IServices;
using AirportAutomation.Core.Dtos.Passenger;
using AirportAutomation.Core.Dtos.Response;
using AirportAutomation.Core.Entities;
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
	public class PassengersController : ControllerBase
	{
		private readonly IPassengerService _passengerService;
		private readonly IPaginationValidationService _paginationValidationService;
		private readonly IMapper _mapper;
		private readonly ILogger<PassengersController> _logger;
		private readonly int maxPageSize;

		public PassengersController(
			IPassengerService passengerService,
			IPaginationValidationService paginationValidationService,
			IMapper mapper,
			ILogger<PassengersController> logger,
			IConfiguration configuration)
		{
			_passengerService = passengerService ?? throw new ArgumentNullException(nameof(passengerService));
			_paginationValidationService = paginationValidationService ?? throw new ArgumentNullException(nameof(paginationValidationService));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			maxPageSize = configuration.GetValue<int>("pageSettings:maxPageSize");
		}

		/// <summary>
		/// Endpoint for retrieving a paginated list of passengers.
		/// </summary>
		/// <param name="page">The page number for pagination (optional).</param>
		/// <param name="pageSize">The number of items per page (optional).</param>
		/// <returns>A paginated list of passengers.</returns>
		/// <response code="200">Returns a list of passengers wrapped in a <see cref="PagedResponse{PassengerDto}"/>.</response>
		/// <response code="204">If no passengers are found.</response>
		/// <response code="400">If the request is invalid.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpGet]
		[ProducesResponseType(200, Type = typeof(PagedResponse<PassengerDto>))]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<PagedResponse<PassengerDto>>> GetPassengers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
		{
			var (isValid, correctedPageSize, result) = _paginationValidationService.ValidatePaginationParameters(page, pageSize, maxPageSize);
			if (!isValid)
			{
				return result;
			}
			var passengers = await _passengerService.GetPassengers(page, correctedPageSize);
			if (passengers is null || !passengers.Any())
			{
				_logger.LogInformation("Passengers not found.");
				return NoContent();
			}
			var totalItems = _passengerService.PassengersCount();
			var data = _mapper.Map<IEnumerable<PassengerDto>>(passengers);
			var response = new PagedResponse<PassengerDto>(data, page, correctedPageSize, totalItems);
			return Ok(response);
		}

		/// <summary>
		/// Endpoint for retrieving single passenger.
		/// </summary>
		/// <param name="id"></param>
		/// <returns>A single passenger that match the specified id.</returns>
		/// <response code="200">Returns a single passenger if any is found.</response>
		/// <response code="404">If no passenger is found.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpGet("{id}")]
		[ProducesResponseType(200, Type = typeof(PassengerDto))]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<PassengerDto>> GetPassenger(int id)
		{
			if (!await _passengerService.PassengerExists(id))
			{
				_logger.LogInformation("Passenger with id {id} not found.", id);
				return NotFound();
			}
			var passenger = await _passengerService.GetPassenger(id);
			var passengerDto = _mapper.Map<PassengerDto>(passenger);
			return Ok(passengerDto);
		}

		/// <summary>
		/// Endpoint for fetching a list of passengers by first name, last name, or both.
		/// </summary>
		/// <param name="firstName"></param>
		/// <param name="lastName"></param>
		/// <returns>A list of passengers.</returns>
		/// <response code="200">Returns a list of passengers if any are found.</response>
		/// <response code="404">If no passengers are found.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpGet("byName")]
		[ProducesResponseType(200, Type = typeof(IEnumerable<PassengerDto>))]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<IEnumerable<PassengerDto>>> GetPassengersByName(
			[FromQuery] string? firstName = null,
			[FromQuery] string? lastName = null)
		{
			if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
			{
				_logger.LogInformation("Both first name and last name are missing in the request.");
				return BadRequest("Both first name and last name are missing in the request.");
			}
			var passengers = await _passengerService.GetPassengersByName(firstName, lastName);
			if (passengers == null || passengers.Count == 0)
			{
				_logger.LogInformation("Passengers not found.");
				return NotFound();
			}
			var passengersDto = _mapper.Map<IEnumerable<PassengerDto>>(passengers);
			return Ok(passengersDto);
		}

		/// <summary>
		/// Endpoint for creating a new passenger.
		/// </summary>
		/// <param name="passengerCreateDto"></param>
		/// <returns>The created passenger.</returns>
		/// <response code="201">Returns the created passenger if successful.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpPost]
		[ProducesResponseType(201, Type = typeof(PassengerCreateDto))]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<Passenger>> PostPassenger(PassengerCreateDto passengerCreateDto)
		{
			var passenger = _mapper.Map<Passenger>(passengerCreateDto);
			await _passengerService.PostPassenger(passenger);
			return CreatedAtAction("GetPassenger", new { id = passenger.Id }, passenger);
		}

		/// <summary>
		/// Endpoint for updating a single passenger.
		/// </summary>
		/// <param name="id">The ID of the passenger to update.</param>
		/// <param name="passengerDto">The updated passenger data.</param>
		/// <response code="204">No content. The update was successful.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="404">If the passenger with the specified ID is not found.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpPut("{id}")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		public async Task<IActionResult> PutPassenger(int id, PassengerDto passengerDto)
		{
			if (id != passengerDto.Id)
			{
				_logger.LogInformation("Passenger with id {id} is different from provided passenger and his id.", id);
				return BadRequest();
			}
			if (!await _passengerService.PassengerExists(id))
			{
				_logger.LogInformation("Passenger with id {id} not found.", id);
				return NotFound();
			}
			var passenger = _mapper.Map<Passenger>(passengerDto);
			await _passengerService.PutPassenger(passenger);
			return NoContent();
		}

		/// <summary>
		/// Endpoint for partially updating a single passenger.
		/// </summary>
		/// <param name="id">The ID of the passenger to partially update.</param>
		/// <param name="passengerDocument">The JSON document containing the partial update instructions.</param>
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
		/// <response code="404">If the passenger with the specified ID is not found.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpPatch("{id}")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		public async Task<IActionResult> PatchPassenger(int id, [FromBody] JsonPatchDocument passengerDocument)
		{
			if (!await _passengerService.PassengerExists(id))
			{
				_logger.LogInformation("Passenger with id {id} not found.", id);
				return NotFound();
			}
			var updatedPassenger = await _passengerService.PatchPassenger(id, passengerDocument);
			return Ok(updatedPassenger);
		}

		/// <summary>
		/// Endpoint for deleting a single passenger by ID.
		/// </summary>
		/// <param name="id">The ID of the passenger to delete.</param>
		/// <response code="204">No content. The deletion was successful.</response>
		/// <response code="404">If the passenger with the specified ID is not found.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		/// <response code="409">Conflict. If the passenger cannot be deleted because it is being referenced by other entities.</response>
		[HttpDelete("{id}")]
		[ProducesResponseType(204)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		[ProducesResponseType(409)]
		public async Task<IActionResult> DeletePassenger(int id)
		{
			if (!await _passengerService.PassengerExists(id))
			{
				_logger.LogInformation("Passenger with id {id} not found.", id);
				return NotFound();
			}
			bool deleted = await _passengerService.DeletePassenger(id);
			if (deleted)
			{
				return NoContent();
			}
			else
			{
				_logger.LogInformation("Passenger with id {id} is being referenced by other entities and cannot be deleted.", id);
				return Conflict("Passenger cannot be deleted because it is being referenced by other entities.");
			}
		}

	}
}

