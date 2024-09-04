﻿using AirportAutomation.Api.Interfaces;
using AirportAutomation.Application.Dtos.Response;
using AirportAutomation.Application.Dtos.TravelClass;
using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Interfaces.IServices;
using AirportАutomation.Api.Controllers;
using AirportАutomation.Api.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AirportАutomationApi.Controllers
{
	[Authorize]
	[ApiVersion("1.0")]
	public class TravelClassesController : BaseController
	{
		private readonly ITravelClassService _travelClassService;
		private readonly IPaginationValidationService _paginationValidationService;
		private readonly IInputValidationService _inputValidationService;
		private readonly IUtilityService _utilityService;
		private readonly IExportService _exportService;
		private readonly IMapper _mapper;
		private readonly ILogger<TravelClassesController> _logger;
		private readonly int maxPageSize;

		public TravelClassesController(
			ITravelClassService travelClassService,
			IPaginationValidationService paginationValidationService,
			IInputValidationService inputValidationService,
			IUtilityService utilityService,
			IExportService exportService,
			IMapper mapper,
			ILogger<TravelClassesController> logger,
			IConfiguration configuration
		)
		{
			_travelClassService = travelClassService ?? throw new ArgumentNullException(nameof(travelClassService));
			_paginationValidationService = paginationValidationService ?? throw new ArgumentNullException(nameof(paginationValidationService));
			_inputValidationService = inputValidationService ?? throw new ArgumentNullException(nameof(inputValidationService));
			_utilityService = utilityService ?? throw new ArgumentNullException(nameof(utilityService));
			_exportService = exportService ?? throw new ArgumentNullException(nameof(exportService));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			maxPageSize = configuration.GetValue<int>("pageSettings:maxPageSize");
		}

		/// <summary>
		/// Endpoint for retrieving a paginated list of travel classes.
		/// </summary>
		/// <param name="page">The page number for pagination (optional).</param>
		/// <param name="pageSize">The number of items per page (optional).</param>
		/// <returns>A paginated list of travel classes.</returns>
		/// <response code="200">Returns a list of travel classes wrapped in a <see cref="PagedResponse{TravelClassDto}"/>.</response>
		/// <response code="204">If no travel classes are found.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpGet]
		[ProducesResponseType(200, Type = typeof(PagedResponse<TravelClassDto>))]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<PagedResponse<TravelClassDto>>> GetTravelClasses([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
		{
			var (isValid, correctedPageSize, result) = _paginationValidationService.ValidatePaginationParameters(page, pageSize, maxPageSize);
			if (!isValid)
			{
				return result;
			}
			var travelClasses = await _travelClassService.GetTravelClasses(page, correctedPageSize);
			if (travelClasses is null || !travelClasses.Any())
			{
				_logger.LogInformation("Travel classes not found.");
				return NoContent();
			}
			var totalItems = await _travelClassService.TravelClassesCount();
			var data = _mapper.Map<IEnumerable<TravelClassDto>>(travelClasses);
			var response = new PagedResponse<TravelClassDto>(data, page, correctedPageSize, totalItems);
			return Ok(response);
		}

		/// <summary>
		/// Endpoint for retrieving a single travel class.
		/// </summary>
		/// <param name="id">The ID of the travel class to retrieve.</param>
		/// <returns>A single travel class that matches the specified ID.</returns>
		/// <response code="200">Returns a single travel class if found.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="404">If no travel class is found.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpGet("{id}")]
		[ProducesResponseType(200, Type = typeof(TravelClassDto))]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<TravelClassDto>> GetTravelClass(int id)
		{
			if (!_inputValidationService.IsNonNegativeInt(id))
			{
				_logger.LogInformation("Invalid input. The ID {id} must be a non-negative integer.", id);
				return BadRequest("Invalid input. The ID must be a non-negative integer.");
			}
			if (!await _travelClassService.TravelClassExists(id))
			{
				_logger.LogInformation("Travel class with id {id} not found.", id);
				return NotFound();
			}
			var travelClass = await _travelClassService.GetTravelClass(id);
			var travelClassDto = _mapper.Map<TravelClassDto>(travelClass);
			return Ok(travelClassDto);
		}

		/// <summary>
		/// Endpoint for exporting travel class data to PDF.
		/// </summary>
		/// <param name="page">The page number for pagination (optional, default is 1).</param>
		/// <param name="pageSize">The page size for pagination (optional, default is 10).</param>
		/// <returns>Returns the generated PDF document.</returns>
		/// <response code="200">Returns the generated PDF document.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpGet("export/pdf")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		public async Task<ActionResult> ExportToPdf([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
		{
			var (isValid, correctedPageSize, result) = _paginationValidationService.ValidatePaginationParameters(page, pageSize, maxPageSize);
			if (!isValid)
			{
				return result;
			}
			var travelClasses = await _travelClassService.GetTravelClasses(page, correctedPageSize);
			if (travelClasses is null || !travelClasses.Any())
			{
				_logger.LogInformation("Travel Classes not found.");
				return NoContent();
			}
			var pdf = _exportService.ExportToPDF<TravelClassEntity>("Travel Classes", travelClasses);
			string fileName = _utilityService.GenerateUniqueFileName("TravelClasses");
			return File(pdf, "application/pdf", fileName);
		}

	}
}
