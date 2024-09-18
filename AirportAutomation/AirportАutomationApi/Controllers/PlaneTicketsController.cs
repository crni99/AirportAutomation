using AirportAutomation.Api.Interfaces;
using AirportAutomation.Application.Dtos.PlaneTicket;
using AirportAutomation.Application.Dtos.Response;
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
	public class PlaneTicketsController : BaseController
	{
		private readonly IPlaneTicketService _planeTicketService;
		private readonly IPaginationValidationService _paginationValidationService;
		private readonly IInputValidationService _inputValidationService;
		private readonly IUtilityService _utilityService;
		private readonly IExportService _exportService;
		private readonly IMapper _mapper;
		private readonly ILogger<PlaneTicketsController> _logger;
		private readonly int maxPageSize;

		public PlaneTicketsController(
			IPlaneTicketService planeTicketService,
			IPaginationValidationService paginationValidationService,
			IInputValidationService inputValidationService,
			IUtilityService utilityService,
			IExportService exportService,
			IMapper mapper,
			ILogger<PlaneTicketsController> logger,
			IConfiguration configuration)
		{
			_planeTicketService = planeTicketService ?? throw new ArgumentNullException(nameof(planeTicketService));
			_paginationValidationService = paginationValidationService ?? throw new ArgumentNullException(nameof(paginationValidationService));
			_inputValidationService = inputValidationService ?? throw new ArgumentNullException(nameof(inputValidationService));
			_utilityService = utilityService ?? throw new ArgumentNullException(nameof(utilityService));
			_exportService = exportService ?? throw new ArgumentNullException(nameof(exportService));
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
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpGet]
		[ProducesResponseType(200, Type = typeof(PagedResponse<PlaneTicketDto>))]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<PagedResponse<PlaneTicketDto>>> GetPlaneTickets([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
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
			var totalItems = await _planeTicketService.PlaneTicketsCount();
			var data = _mapper.Map<IEnumerable<PlaneTicketDto>>(planeTickets);
			var response = new PagedResponse<PlaneTicketDto>(data, page, correctedPageSize, totalItems);
			return Ok(response);
		}

		/// <summary>
		/// Endpoint for retrieving a single plane ticket.
		/// </summary>
		/// <param name="id">The ID of the plane ticket to retrieve.</param>
		/// <returns>A single plane ticket that matches the specified ID.</returns>
		/// <response code="200">Returns a single plane ticket if found.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="404">If no plane ticket is found.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpGet("{id}")]
		[ProducesResponseType(200, Type = typeof(PlaneTicketDto))]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<PlaneTicketDto>> GetPlaneTicket(int id)
		{
			if (!_inputValidationService.IsNonNegativeInt(id))
			{
				_logger.LogInformation("Invalid input. The ID {id} must be a non-negative integer.", id);
				return BadRequest("Invalid input. The ID must be a non-negative integer.");
			}
			if (!await _planeTicketService.PlaneTicketExists(id))
			{
				_logger.LogInformation("Plane ticket with id {id} not found.", id);
				return NotFound();
			}
			var planeTicket = await _planeTicketService.GetPlaneTicket(id);
			var planeTicketDto = _mapper.Map<PlaneTicketDto>(planeTicket);
			return Ok(planeTicketDto);
		}

		/// <summary>
		/// Endpoint for retrieving a paginated list of plane tickets containing the specified name.
		/// </summary>
		/// <param name="minPrice">The minimum price to search for.</param>
		/// <param name = "maxPrice" > The maximum price to search for.</param>
		/// <param name="page">The page number for pagination (optional).</param>
		/// <param name="pageSize">The size of each page for pagination (optional).</param>
		/// <returns>A list of plane tickets that match the specified name.</returns>
		/// <response code="200">Returns a paged list of plane tickets if found.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="404">If no plane tickets are found.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpGet("byPrice")]
		[ProducesResponseType(200, Type = typeof(PagedResponse<PlaneTicketDto>))]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<PagedResponse<PlaneTicketDto>>> GetPlaneTicketsForPrice(
			[FromQuery] int? minPrice = null,
			[FromQuery] int? maxPrice = null,
			[FromQuery] int page = 1,
			[FromQuery] int pageSize = 10)
		{
			if (minPrice == null && maxPrice == null)
			{
				_logger.LogInformation("Both min price and max price are missing in the request.");
				return BadRequest("Both min price and max price are missing in the request.");
			}
			var (isValid, correctedPageSize, result) = _paginationValidationService.ValidatePaginationParameters(page, pageSize, maxPageSize);
			if (!isValid)
			{
				return result;
			}
			var planeTickets = await _planeTicketService.GetPlaneTicketsForPrice(page, correctedPageSize, minPrice, maxPrice);
			if (planeTickets is null || !planeTickets.Any())
			{
				_logger.LogInformation("Plane tickets not found.");
				return NotFound();
			}
			var totalItems = await _planeTicketService.PlaneTicketsCount(minPrice, maxPrice);
			var data = _mapper.Map<IEnumerable<PlaneTicketDto>>(planeTickets);
			var response = new PagedResponse<PlaneTicketDto>(data, page, correctedPageSize, totalItems);
			return Ok(response);
		}

		/// <summary>
		/// Endpoint for creating a new plane ticket.
		/// </summary>
		/// <param name="planeTicketCreateDto">The data to create the new plane ticket.</param>
		/// <returns>The created plane ticket.</returns>
		/// <response code="201">Returns the created plane ticket if successful.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		/// <response code="403">If the user does not have permission to access the requested resource.</response>
		[HttpPost]
		[Authorize(Policy = "RequireAdminRole")]
		[ProducesResponseType(201, Type = typeof(PlaneTicketDto))]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(403)]
		public async Task<ActionResult<PlaneTicketEntity>> PostPlaneTicket(PlaneTicketCreateDto planeTicketCreateDto)
		{
			try
			{
				var planeTicket = _mapper.Map<PlaneTicketEntity>(planeTicketCreateDto);
				await _planeTicketService.PostPlaneTicket(planeTicket);
				var planeTicketDto = _mapper.Map<PlaneTicketDto>(planeTicket);
				return CreatedAtAction("GetPlaneTicket", new { id = planeTicketDto.Id }, planeTicketDto);
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
		/// <param name="planeTicketUpdateDto">The data to update the plane ticket.</param>
		/// <returns>No content.</returns>
		/// <response code="204">Returns no content if successful.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="404">If no plane ticket is found.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		/// <response code="403">If the user does not have permission to access the requested resource.</response>
		[HttpPut("{id}")]
		[Authorize(Policy = "RequireAdminRole")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		[ProducesResponseType(403)]
		public async Task<IActionResult> PutPlaneTicket(int id, PlaneTicketUpdateDto planeTicketUpdateDto)
		{
			if (!_inputValidationService.IsNonNegativeInt(id))
			{
				_logger.LogInformation("Invalid input. The ID {id} must be a non-negative integer.", id);
				return BadRequest("Invalid input. The ID must be a non-negative integer.");
			}
			if (id != planeTicketUpdateDto.Id)
			{
				_logger.LogInformation("Plane Ticket with id {id} is different from provided Plane Ticket and his id.", id);
				return BadRequest();
			}
			if (!await _planeTicketService.PlaneTicketExists(id))
			{
				_logger.LogInformation("Plane ticket with id {id} not found.", id);
				return NotFound();
			}
			var planeTicket = _mapper.Map<PlaneTicketEntity>(planeTicketUpdateDto);
			await _planeTicketService.PutPlaneTicket(planeTicket);
			return NoContent();
		}

		/// <summary>
		/// Endpoint for partially updating a single plane ticket.
		/// </summary>
		/// <param name="id">The ID of the plane ticket to partially update.</param>
		/// <param name="planeTicketDocument">The patch document containing the changes.</param>
		/// <returns>The updated plane ticket.</returns>
		/// <remarks>
		/// The JSON document should follow the JSON Patch standard (RFC 6902) and contain one or more operations.
		/// Example operation:
		/// {
		///     "op": "replace",
		///     "path": "/Name",
		///     "value": "NewName"
		/// }
		/// </remarks>
		/// <response code="200">Returns the updated plane ticket if successful.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="404">If the plane ticket with the specified ID is not found.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		/// <response code="403">If the user does not have permission to access the requested resource.</response>
		[HttpPatch("{id}")]
		[Authorize(Policy = "RequireAdminRole")]
		[ProducesResponseType(200, Type = typeof(PlaneTicketDto))]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		[ProducesResponseType(403)]
		public async Task<IActionResult> PatchPlaneTicket(int id, [FromBody] JsonPatchDocument planeTicketDocument)
		{
			if (!_inputValidationService.IsNonNegativeInt(id))
			{
				_logger.LogInformation("Invalid input. The ID {id} must be a non-negative integer.", id);
				return BadRequest("Invalid input. The ID must be a non-negative integer.");
			}
			if (!await _planeTicketService.PlaneTicketExists(id))
			{
				_logger.LogInformation("Plane Ticket with id {id} not found.", id);
				return NotFound();
			}
			var updatedPlaneTicket = await _planeTicketService.PatchPlaneTicket(id, planeTicketDocument);
			var planeTicketDto = _mapper.Map<PlaneTicketDto>(updatedPlaneTicket);
			return Ok(planeTicketDto);
		}

		/// <summary>
		/// Endpoint for deleting a single plane ticket.
		/// </summary>
		/// <param name="id">The ID of the plane ticket to delete.</param>
		/// <returns>No content.</returns>
		/// <response code="204">Returns no content if successful.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="404">If no plane ticket is found.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		/// <response code="403">If the user does not have permission to access the requested resource.</response>
		/// <response code="409">Conflict. If the passenger cannot be deleted because it is being referenced by other entities.</response>
		[HttpDelete("{id}")]
		[Authorize(Policy = "RequireAdminRole")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		[ProducesResponseType(403)]
		[ProducesResponseType(409)]
		public async Task<IActionResult> DeletePlaneTicket(int id)
		{
			if (!_inputValidationService.IsNonNegativeInt(id))
			{
				_logger.LogInformation("Invalid input. The ID {id} must be a non-negative integer.", id);
				return BadRequest("Invalid input. The ID must be a non-negative integer.");
			}
			if (!await _planeTicketService.PlaneTicketExists(id))
			{
				_logger.LogInformation("Plane ticket with id {id} not found.", id);
				return NotFound();
			}
			bool deleted = await _planeTicketService.DeletePlaneTicket(id);
			if (deleted)
			{
				return NoContent();
			}
			else
			{
				return Conflict();
			}
		}

		/// <summary>
		/// Endpoint for exporting plane ticket data to PDF.
		/// </summary>
		/// <param name="page">The page number for pagination (optional, default is 1).</param>
		/// <param name="pageSize">The page size for pagination (optional, default is 10).</param>
		/// <param name="getAll">Flag indicating whether to retrieve all data (optional, default is false).</param>
		/// <param name="minPrice">The minimum price to search for (optional, default is null).</param>
		/// <param name="maxPrice" > The maximum price to search for (optional, default is null).</param>
		/// <returns>Returns the generated PDF document.</returns>
		/// <response code="200">Returns the generated PDF document.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpGet("export/pdf")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		public async Task<ActionResult> ExportToPdf(
			[FromQuery] int page = 1,
			[FromQuery] int pageSize = 10,
			[FromQuery] bool getAll = false,
			[FromQuery] int? minPrice = null,
			[FromQuery] int? maxPrice = null)
		{
			IList<PlaneTicketEntity> planeTickets;
			if (getAll)
			{
				planeTickets = await _planeTicketService.GetAllPlaneTickets();
			}
			else
			{
				var (isValid, correctedPageSize, result) = _paginationValidationService.ValidatePaginationParameters(page, pageSize, maxPageSize);
				if (!isValid)
				{
					return result;
				}
				if (!minPrice.HasValue && !maxPrice.HasValue)
				{
					planeTickets = await _planeTicketService.GetPlaneTickets(page, correctedPageSize);
				}
				else
				{
					planeTickets = await _planeTicketService.GetPlaneTicketsForPrice(page, correctedPageSize, minPrice, maxPrice);
				}
			}
			if (planeTickets is null || !planeTickets.Any())
			{
				_logger.LogInformation("Plane Tickets not found.");
				return NoContent();
			}
			var pdf = _exportService.ExportToPDF<PlaneTicketEntity>("Plane Tickets", planeTickets);
			string fileName = _utilityService.GenerateUniqueFileName("PlaneTickets");
			return File(pdf, "application/pdf", fileName);
		}

	}
}
