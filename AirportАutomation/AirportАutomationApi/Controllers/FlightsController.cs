using AirportAutomation.Api.Interfaces;
using AirportAutomation.Core.Dtos.Flight;
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
	public class FlightsController : ControllerBase
	{
		private readonly IFlightService _flightService;
		private readonly IPaginationValidationService _paginationValidationService;
		private readonly IMapper _mapper;
		private readonly ILogger<FlightsController> _logger;
		private readonly int maxPageSize;

		public FlightsController(
			IFlightService flightService,
			IPaginationValidationService paginationValidationService,
			IMapper mapper,
			ILogger<FlightsController> logger,
			IConfiguration configuration)
		{
			_flightService = flightService ?? throw new ArgumentNullException(nameof(flightService));
			_paginationValidationService = paginationValidationService ?? throw new ArgumentNullException(nameof(paginationValidationService));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			maxPageSize = configuration.GetValue<int>("pageSettings:maxPageSize");
		}

		/// <summary>
		/// Endpoint for retrieving a paginated list of flights.
		/// </summary>
		/// <param name="page">The page number for pagination (optional).</param>
		/// <param name="pageSize">The number of items per page (optional).</param>
		/// <returns>A paginated list of flights.</returns>
		/// <response code="200">Returns a list of flights wrapped in a <see cref="PagedResponse{FlightDto}"/>.</response>
		/// <response code="204">If no flights are found.</response>
		/// <response code="400">If the request is invalid.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpGet]
		[ProducesResponseType(200, Type = typeof(PagedResponse<FlightDto>))]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<PagedResponse<FlightDto>>> GetFlights([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
		{
			var (isValid, correctedPageSize, result) = _paginationValidationService.ValidatePaginationParameters(page, pageSize, maxPageSize);
			if (!isValid)
			{
				return result;
			}
			var flights = await _flightService.GetFlights(page, correctedPageSize);
			if (flights is null || !flights.Any())
			{
				_logger.LogInformation("Flights not found.");
				return NoContent();
			}
			var totalItems = _flightService.FlightsCount();
			var data = _mapper.Map<IEnumerable<FlightDto>>(flights);
			var response = new PagedResponse<FlightDto>(data, page, correctedPageSize, totalItems);
			return Ok(response);
		}

		/// <summary>
		/// Endpoint for retrieving single flight.
		/// </summary>
		/// <param name="id"></param>
		/// <returns>A single flight that match the specified id.</returns>
		/// <response code="200">Returns a single flight if any is found.</response>
		/// <response code="404">If no flight is found.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpGet("{id}")]
		[ProducesResponseType(200, Type = typeof(FlightDto))]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<FlightDto>> GetFlight(int id)
		{
			if (!await _flightService.FlightExists(id))
			{
				_logger.LogInformation("Flight with id {id} not found.", id);
				return NotFound();
			}
			var flight = await _flightService.GetFlight(id);
			var flightDto = _mapper.Map<FlightDto>(flight);
			return Ok(flightDto);
		}

		/// <summary>
		/// Endpoint for retrieving flights between two specified dates.
		/// </summary>
		/// <param name="startDate">The start date of the date range.</param>
		/// <param name="endDate">The end date of the date range.</param>
		/// <returns>A list of flights within the specified date range.</returns>
		/// <response code="200">Returns a list of flights if any is found.</response>
		/// <response code="404">If no flights is found.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpGet("byDate")]
		[ProducesResponseType(200, Type = typeof(IEnumerable<FlightDto>))]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<IEnumerable<FlightDto>>> GetFlightsBetweenDates(
			[FromQuery] DateOnly? startDate = null,
			[FromQuery] DateOnly? endDate = null)
		{
			if (!startDate.HasValue && !endDate.HasValue)
			{
				_logger.LogInformation("Both start date and end date are missing in the request.");
				return BadRequest("Both start date and end date are missing in the request.");
			}
			var flights = await _flightService.GetFlightsBetweenDates(startDate, endDate);
			if (flights == null || flights.Count == 0)
			{
				_logger.LogInformation("Flights not found.");
				return NotFound();
			}
			var flightsDto = _mapper.Map<IEnumerable<FlightDto>>(flights);
			return Ok(flightsDto);
		}

		/// <summary>
		/// Endpoint for creating a new flight.
		/// </summary>
		/// <param name="flightCreateDto"></param>
		/// <returns>The created flight.</returns>
		/// <response code="201">Returns the created flight if successful.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpPost]
		[ProducesResponseType(201, Type = typeof(Flight))]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<Flight>> PostFlight(FlightCreateDto flightCreateDto)
		{
			var flight = _mapper.Map<Flight>(flightCreateDto);
			await _flightService.PostFlight(flight);
			return CreatedAtAction("GetFlight", new { id = flight.Id }, flight);
		}

		/// <summary>
		/// Endpoint for updating a single flight.
		/// </summary>
		/// <param name="id">The ID of the flight to update.</param>
		/// <param name="flightUpdateDto">The updated flight data.</param>
		/// <response code="204">No content. The update was successful.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="404">If the flight with the specified ID is not found.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpPut("{id}")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		public async Task<IActionResult> PutFlight(int id, FlightUpdateDto flightUpdateDto)
		{
			if (id != flightUpdateDto.Id)
			{
				_logger.LogInformation("Flight with id {id} is different from provided Flight and his id.", id);
				return BadRequest();
			}
			if (!await _flightService.FlightExists(id))
			{
				_logger.LogInformation("Flight with id {id} not found.", id);
				return NotFound();
			}
			var flight = _mapper.Map<Flight>(flightUpdateDto);
			await _flightService.PutFlight(flight);
			return NoContent();
		}

		/// <summary>
		/// Endpoint for partially updating a single flight.
		/// </summary>
		/// <param name="id">The ID of the flight to partially update.</param>
		/// <param name="flightDocument">The JSON document containing the partial update instructions.</param>
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
		/// <response code="404">If the flight with the specified ID is not found.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpPatch("{id}")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		public async Task<IActionResult> PatchFlight(int id, [FromBody] JsonPatchDocument flightDocument)
		{
			if (!await _flightService.FlightExists(id))
			{
				_logger.LogInformation("Flight with id {id} not found.", id);
				return NotFound();
			}
			var updatedFlight = await _flightService.PatchFlight(id, flightDocument);
			return Ok(updatedFlight);
		}

		/// <summary>
		/// Endpoint for deleting a single flight by ID.
		/// </summary>
		/// <param name="id">The ID of the flight to delete.</param>
		/// <response code="204">No content. The deletion was successful.</response>
		/// <response code="404">If the flight with the specified ID is not found.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		/// <response code="409">Conflict. If the passenger cannot be deleted because it is being referenced by other entities.</response>
		[HttpDelete("{id}")]
		[ProducesResponseType(204)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		[ProducesResponseType(409)]
		public async Task<IActionResult> DeleteFlight(int id)
		{
			if (!await _flightService.FlightExists(id))
			{
				_logger.LogInformation("Flight with id {id} not found.", id);
				return NotFound();
			}
			bool deleted = await _flightService.DeleteFlight(id);
			if (deleted)
			{
				return NoContent();
			}
			else
			{
				_logger.LogInformation("Flight with id {id} is being referenced by other entities and cannot be deleted.", id);
				return Conflict("Flight cannot be deleted because it is being referenced by other entities.");
			}
		}

	}
}

