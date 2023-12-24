using AirportAutomationApi.Dtos.TravelClass;
using AirportAutomationApi.IService;
using AirportАutomationApi.Dtos.Response;
using AirportАutomationApi.IServices;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AirportAutomationApi.Controllers
{
	[Authorize]
	[ApiVersion("1.0")]
	[Route("api/[controller]")]
	[ApiController]
	public class TravelClassesController : ControllerBase
	{
		private readonly ITravelClassService _travelClassService;
		private readonly IPaginationValidationService _paginationValidationService;
		private readonly IMapper _mapper;
		private readonly ILogger<TravelClassesController> _logger;
		private readonly int maxPageSize;

		public TravelClassesController(ITravelClassService travelClassService, IPaginationValidationService paginationValidationService, IMapper mapper, ILogger<TravelClassesController> logger, IConfiguration configuration)
		{
			_travelClassService = travelClassService ?? throw new ArgumentNullException(nameof(travelClassService));
			_paginationValidationService = paginationValidationService ?? throw new ArgumentNullException(nameof(paginationValidationService));
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
		/// <response code="400">If the request is invalid.</response>
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
			var totalItems = _travelClassService.TravelClassesCount();
			var data = _mapper.Map<IEnumerable<TravelClassDto>>(travelClasses);
			var response = new PagedResponse<TravelClassDto>(data, page, correctedPageSize, totalItems);
			return Ok(response);
		}

		/// <summary>
		/// Endpoint for retrieving single travel class.
		/// </summary>
		/// <param name="id"></param>
		/// <returns>A single travel class that match the specified id.</returns>
		/// <response code="200">Returns a single travel class if any is found.</response>
		/// <response code="404">If no travel class is found.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpGet("{id}")]
		[ProducesResponseType(200, Type = typeof(TravelClassDto))]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<TravelClassDto>> GetTravelClass(int id)
		{
			if (!_travelClassService.TravelClassExists(id))
			{
				_logger.LogInformation("Travel class with id {id} not found.", id);
				return NotFound();
			}
			var travelClass = await _travelClassService.GetTravelClass(id);
			var travelClassDto = _mapper.Map<TravelClassDto>(travelClass);
			return Ok(travelClassDto);
		}

	}
}

