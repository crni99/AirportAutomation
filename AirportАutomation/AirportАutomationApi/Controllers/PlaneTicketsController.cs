using AirportAutomationApi.Dtos.PlaneTicket;
using AirportAutomationApi.Entities;
using AirportAutomationApi.IService;
using AirportАutomationApi.Dtos.PlaneTicket;
using AirportАutomationApi.Dtos.Response;
using AirportАutomationApi.IServices;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace AirportAutomationApi.Controllers
{
	[Authorize]
	[ApiVersion("1.0")]
	[Route("api/[controller]")]
	[ApiController]
	public class PlaneTicketsController : ControllerBase
	{
		private readonly IPlaneTicketService _planeTicketService;
		private readonly IPaginationValidationService _paginationValidationService;
		private readonly IMapper _mapper;
		private readonly ILogger<PlaneTicketsController> _logger;
		private readonly int maxPageSize;

		public PlaneTicketsController(
			IPlaneTicketService planeTicketService,
			IPaginationValidationService paginationValidationService,
			IMapper mapper,
			ILogger<PlaneTicketsController> logger,
			IConfiguration configuration)
		{
			_planeTicketService = planeTicketService ?? throw new ArgumentNullException(nameof(planeTicketService));
			_paginationValidationService = paginationValidationService ?? throw new ArgumentNullException(nameof(paginationValidationService));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			maxPageSize = configuration.GetValue<int>("pageSettings:maxPageSize");
		}

		/// <summary>
		/// Endpoint for retrieving a paginated list of plane tickets.
		/// </summary>
		/// <param name="page">The page number for pagination (optional).</param>
		/// <param name="pageSize">The number of items per page (optional).</param>
		/// <returns>A paginated list of plane tickets.</returns>
		/// <response code="200">Returns a list of plane tickets wrapped in a <see cref="PagedResponse{PlaneTicketDto}"/>.</response>
		/// <response code="204">If no plane tickets are found.</response>
		/// <response code="400">If the request is invalid.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpGet]
		[ProducesResponseType(200, Type = typeof(PagedResponse<PlaneTicketDto>))]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<PagedResponse<PlaneTicketDto>>> GetPlaneTickets([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
		{
			try
			{
				var (isValid, correctedPageSize, result) = _paginationValidationService.ValidatePaginationParameters(page, pageSize, maxPageSize);
				if (!isValid)
				{
					return result;
				}
				var planeTickets = await _planeTicketService.GetPlaneTickets(page, correctedPageSize);
				if (planeTickets is null || !planeTickets.Any())
				{
					_logger.LogInformation("Plane tickets not found.");
					return NoContent();
				}
				var totalItems = _planeTicketService.PlaneTicketsCount();
				var data = _mapper.Map<IEnumerable<PlaneTicketDto>>(planeTickets);
				var response = new PagedResponse<PlaneTicketDto>(data, page, correctedPageSize, totalItems);
				return Ok(response);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting plane tickets list.");
				throw;
			}
		}

		/// <summary>
		/// Endpoint for retrieving a list of plane tickets based on the specified price range.
		/// </summary>
		/// <param name="minPrice">The minimum price for plane tickets to include in the result.</param>
		/// <param name="maxPrice">The maximum price for plane tickets to include in the result.</param>
		/// <returns>
		/// An IActionResult containing a list of plane tickets within the specified price range.
		/// </returns>
		/// <response code="200">Returns a list of plane tickets if any are found.</response>
		/// <response code="404">If no plane tickets are found.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpGet("byPrice")]
		[ProducesResponseType(200, Type = typeof(IEnumerable<PlaneTicketDto>))]
		[ProducesResponseType(204)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<IEnumerable<PlaneTicketDto>>> GetPlaneTicketsForPrice(
			[FromQuery] int? minPrice = null,
			[FromQuery] int? maxPrice = null)
		{
			try
			{
				if (!minPrice.HasValue && !maxPrice.HasValue)
				{
					_logger.LogInformation("Both min price and max price are missing in the request.");
					return BadRequest("Both min price and max price are missing in the request.");
				}
				var planeTickets = await _planeTicketService.GetPlaneTicketsForPrice(minPrice, maxPrice);
				if (planeTickets is null || !planeTickets.Any())
				{
					_logger.LogInformation("Plane tickets not found.");
					return NoContent();
				}
				var planeTicketsDto = _mapper.Map<IEnumerable<PlaneTicketDto>>(planeTickets);
				return Ok(planeTicketsDto);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting plane tickets list.");
				throw;
			}
		}

		/// <summary>
		/// Endpoint for retrieving single plane ticket.
		/// </summary>
		/// <param name="id"></param>
		/// <returns>A single plane ticket that match the specified id.</returns>
		/// <response code="200">Returns a single plane ticket if any is found.</response>
		/// <response code="404">If no plane ticket is found.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpGet("{id}")]
		[ProducesResponseType(200, Type = typeof(PlaneTicketDto))]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<PlaneTicketDto>> GetPlaneTicket(int id)
		{
			try
			{
				if (!_planeTicketService.PlaneTicketExists(id))
				{
					_logger.LogInformation("Plane ticket with id {id} not found.", id);
					return NotFound();
				}
				var planeTicket = await _planeTicketService.GetPlaneTicket(id);
				var planeTicketDto = _mapper.Map<PlaneTicketDto>(planeTicket);
				return Ok(planeTicketDto);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting plane ticket with id: {id} .", id);
				throw;
			}
		}

		/// <summary>
		/// Endpoint for creating a new plane ticket.
		/// </summary>
		/// <param name="planeTicketCreateDto"></param>
		/// <returns>The created plane ticket.</returns>
		/// <response code="200">Returns the created plane ticket if successful.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpPost]
		[ProducesResponseType(201, Type = typeof(PlaneTicket))]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<PlaneTicket>> PostPlaneTicket(PlaneTicketCreateDto planeTicketCreateDto)
		{
			try
			{
				var planeTicket = _mapper.Map<PlaneTicket>(planeTicketCreateDto);
				await _planeTicketService.PostPlaneTicket(planeTicket);
				return CreatedAtAction("GetPlaneTicket", new { id = planeTicket.Id }, planeTicket);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating plane ticket.");
				throw;
			}
		}

		/// <summary>
		/// Endpoint for updating a single plane ticket.
		/// </summary>
		/// <param name="id">The ID of the plane ticket to update.</param>
		/// <param name="planeTicketUpdateDto">The updated plane ticket data.</param>
		/// <response code="204">No content. The update was successful.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="404">If the plane ticket with the specified ID is not found.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpPut("{id}")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		public async Task<IActionResult> PutPlaneTicket(int id, PlaneTicketUpdateDto planeTicketUpdateDto)
		{
			try
			{
				if (id != planeTicketUpdateDto.Id)
				{
					_logger.LogInformation("Plane Ticket with id {id} is different from provided Plane Ticket and his id.", id);
					return BadRequest();
				}
				if (!_planeTicketService.PlaneTicketExists(id))
				{
					_logger.LogInformation("Plane ticket with id {id} not found.", id);
					return NotFound();
				}
				var planeTicket = _mapper.Map<PlaneTicket>(planeTicketUpdateDto);
				await _planeTicketService.PutPlaneTicket(planeTicket);
				return NoContent();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating plane ticket with id: {id} .", id);
				throw;
			}
		}

		/// <summary>
		/// Endpoint for partially updating a single plane ticket.
		/// </summary>
		/// <param name="id">The ID of the plane ticket to partially update.</param>
		/// <param name="planeTicketDocument">The JSON document containing the partial update instructions.</param>
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
		/// <response code="404">If the plane ticket with the specified ID is not found.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpPatch("{id}")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		public async Task<IActionResult> PatchPlaneTicket(int id, [FromBody] JsonPatchDocument planeTicketDocument)
		{
			try
			{
				if (!_planeTicketService.PlaneTicketExists(id))
				{
					_logger.LogInformation("Plane Ticket with id {id} not found.", id);
					return NotFound();
				}
				var updatedPatchPlane = await _planeTicketService.PatchPlaneTicket(id, planeTicketDocument);
				return Ok(updatedPatchPlane);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error patching plane ticket with id: {id} .", id);
				throw;
			}
		}

		/// <summary>
		/// Endpoint for deleting a single plane ticket by ID.
		/// </summary>
		/// <param name="id">The ID of the plane ticket to delete.</param>
		/// <response code="204">No content. The deletion was successful.</response>
		/// <response code="404">If the plane ticket with the specified ID is not found.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpDelete("{id}")]
		[ProducesResponseType(204)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		public async Task<IActionResult> DeletePlaneTicket(int id)
		{
			try
			{
				if (!_planeTicketService.PlaneTicketExists(id))
				{
					_logger.LogInformation("Plane ticket with id {id} not found.", id);
					return NotFound();
				}
				await _planeTicketService.DeletePlaneTicket(id);
				return NoContent();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error deleting plane ticket with id: {id} .", id);
				throw;
			}
		}

	}
}

