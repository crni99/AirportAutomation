﻿using AirportAutomationApi.Dtos.Airline;
using AirportAutomationApi.Entities;
using AirportAutomationApi.IService;
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
	public class AirlinesController : ControllerBase
	{
		private readonly IAirlineService _airlineService;
		private readonly IPaginationValidationService _paginationValidationService;
		private readonly IMapper _mapper;
		private readonly ILogger<AirlinesController> _logger;
		private readonly int maxPageSize;

		public AirlinesController(
			IAirlineService airlineService,
			IPaginationValidationService paginationValidationService,
			IMapper mapper,
			ILogger<AirlinesController> logger,
			IConfiguration configuration
		)
		{
			_airlineService = airlineService ?? throw new ArgumentNullException(nameof(airlineService));
			_paginationValidationService = paginationValidationService ?? throw new ArgumentNullException(nameof(paginationValidationService));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			maxPageSize = configuration.GetValue<int>("pageSettings:maxPageSize");
		}

		/// <summary>
		/// Endpoint for retrieving a paginated list of airlines.
		/// </summary>
		/// <param name="page">The page number for pagination (optional).</param>
		/// <param name="pageSize">The number of items per page (optional).</param>
		/// <returns>A paginated list of airlines.</returns>
		/// <response code="200">Returns a list of airlines wrapped in a <see cref="PagedResponse{AirlineDto}"/>.</response>
		/// <response code="204">If no airlines are found.</response>
		/// <response code="400">If the request is invalid.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpGet]
		[ProducesResponseType(200, Type = typeof(PagedResponse<AirlineDto>))]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<PagedResponse<AirlineDto>>> GetAirlines([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
		{
			var (isValid, correctedPageSize, result) = _paginationValidationService.ValidatePaginationParameters(page, pageSize, maxPageSize);
			if (!isValid)
			{
				return result;
			}
			var airlines = await _airlineService.GetAirlines(page, correctedPageSize);
			if (airlines is null || !airlines.Any())
			{
				_logger.LogInformation("Airlines not found.");
				return NoContent();
			}
			var totalItems = _airlineService.AirlinesCount();
			var data = _mapper.Map<IEnumerable<AirlineDto>>(airlines);
			var response = new PagedResponse<AirlineDto>(data, page, correctedPageSize, totalItems);
			return Ok(response);
		}

		/// <summary>
		/// Endpoint for retrieving single airline.
		/// </summary>
		/// <param name="id"></param>
		/// <returns>A single airline that match the specified id.</returns>
		/// <response code="200">Returns a single airline if any is found.</response>
		/// <response code="404">If no airline is found.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpGet("{id}")]
		[ProducesResponseType(200, Type = typeof(AirlineDto))]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<AirlineDto>> GetAirline(int id)
		{

			if (!await _airlineService.AirlineExists(id))
			{
				_logger.LogInformation("Airline with id {id} not found.", id);
				return NotFound();
			}
			var airline = await _airlineService.GetAirline(id);
			var airlineDto = _mapper.Map<AirlineDto>(airline);
			return Ok(airlineDto);
		}

		/// <summary>
		/// Endpoint for retrieving a list of airlines containing the specified name.
		/// </summary>
		/// <param name="name">The name to search for.</param>
		/// <returns>A list of airlines that match the specified name.</returns>
		/// <response code="200">Returns a list of airlines if any are found.</response>
		/// <response code="404">If no airlines are found.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpGet("byName/{name}")]
		[ProducesResponseType(200, Type = typeof(IEnumerable<AirlineDto>))]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<IEnumerable<AirlineDto>>> GetAirlinesByName(string name)
		{
			var airlines = await _airlineService.GetAirlinesByName(name);
			if (airlines is null || airlines.Count == 0)
			{
				_logger.LogInformation("Airline with name {AirlineName} not found.", name);
				return NotFound();
			}
			var airlinesDto = _mapper.Map<IEnumerable<AirlineDto>>(airlines);
			return Ok(airlinesDto);
		}

		/// <summary>
		/// Endpoint for creating a new airline.
		/// </summary>
		/// <param name="airlineCreateDto"></param>
		/// <returns>The created airline.</returns>
		/// <response code="201">Returns the created airline if successful.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpPost]
		[ProducesResponseType(201, Type = typeof(Airline))]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<Airline>> PostAirline(AirlineCreateDto airlineCreateDto)
		{
			var airline = _mapper.Map<Airline>(airlineCreateDto);
			await _airlineService.PostAirline(airline);
			return CreatedAtAction("GetAirline", new { id = airline.Id }, airline);
		}

		/// <summary>
		/// Endpoint for updating a single airline.
		/// </summary>
		/// <param name="id">The ID of the airline to update.</param>
		/// <param name="airlineDto">The updated airline data.</param>
		/// <response code="204">No content. The update was successful.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="404">If the airline with the specified ID is not found.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpPut("{id}")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		public async Task<IActionResult> PutAirline(int id, AirlineDto airlineDto)
		{
			if (id != airlineDto.Id)
			{
				_logger.LogInformation("Airline with id {id} is different from provided Airline and its id.", id);
				return BadRequest();
			}

			if (!await _airlineService.AirlineExists(id))
			{
				_logger.LogInformation("Airline with id {id} not found.", id);
				return NotFound();
			}
			var airline = _mapper.Map<Airline>(airlineDto);
			await _airlineService.PutAirline(airline);
			return NoContent();
		}

		/// <summary>
		/// Endpoint for partially updating a single airline.
		/// </summary>
		/// <param name="id">The ID of the airline to partially update.</param>
		/// <param name="airlineDocument">The JSON document containing the partial update instructions.</param>
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
		/// <response code="404">If the airline with the specified ID is not found.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpPatch("{id}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		public async Task<IActionResult> PatchAirline(int id, [FromBody] JsonPatchDocument airlineDocument)
		{
			if (!await _airlineService.AirlineExists(id))
			{
				_logger.LogInformation("Airline with id {id} not found.", id);
				return NotFound();
			}
			var updatedAirline = await _airlineService.PatchAirline(id, airlineDocument);
			return Ok(updatedAirline);
		}

		/// <summary>
		/// Endpoint for deleting a single airline by ID.
		/// </summary>
		/// <param name="id">The ID of the airline to delete.</param>
		/// <response code="204">No content. The deletion was successful.</response>
		/// <response code="404">If the airline with the specified ID is not found.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		/// <response code="409">Conflict. If the passenger cannot be deleted because it is being referenced by other entities.</response>
		[HttpDelete("{id}")]
		[ProducesResponseType(204)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		[ProducesResponseType(409)]
		public async Task<IActionResult> DeleteAirline(int id)
		{

			if (!await _airlineService.AirlineExists(id))
			{
				_logger.LogInformation("Airline with id {id} not found.", id);
				return NotFound();
			}
			bool deleted = await _airlineService.DeleteAirline(id);
			if (deleted)
			{
				return NoContent();
			}
			else
			{
				_logger.LogInformation("Airline with id {id} is being referenced by other entities and cannot be deleted.", id);
				return Conflict("Airline cannot be deleted because it is being referenced by other entities.");
			}
		}

	}
}

