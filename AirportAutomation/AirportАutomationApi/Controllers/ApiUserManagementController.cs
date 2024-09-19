﻿using AirportAutomation.Api.Interfaces;
using AirportAutomation.Application.Dtos.Airline;
using AirportAutomation.Application.Dtos.ApiUser;
using AirportAutomation.Application.Dtos.Response;
using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Interfaces.IServices;
using AirportАutomation.Api.Controllers;
using AirportАutomation.Api.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AirportАutomationApi.Controllers
{
	[Authorize(Policy = "RequireSuperAdminRole")]
	[ApiVersion("1.0")]
	public class ApiUserManagementController : BaseController
	{
		private readonly IApiUserManagementService _apiUserService;
		private readonly IPaginationValidationService _paginationValidationService;
		private readonly IInputValidationService _inputValidationService;
		private readonly IUtilityService _utilityService;
		private readonly IMapper _mapper;
		private readonly ILogger<ApiUserManagementController> _logger;
		private readonly int maxPageSize;

		public ApiUserManagementController(
			IApiUserManagementService apiUserService,
			IPaginationValidationService paginationValidationService,
			IInputValidationService inputValidationService,
			IUtilityService utilityService,
			IMapper mapper,
			ILogger<ApiUserManagementController> logger,
			IConfiguration configuration
		)
		{
			_apiUserService = apiUserService ?? throw new ArgumentNullException(nameof(apiUserService));
			_paginationValidationService = paginationValidationService ?? throw new ArgumentNullException(nameof(paginationValidationService));
			_inputValidationService = inputValidationService ?? throw new ArgumentNullException(nameof(inputValidationService));
			_utilityService = utilityService ?? throw new ArgumentNullException(nameof(utilityService));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			maxPageSize = configuration.GetValue<int>("pageSettings:maxPageSize");
		}

		/// <summary>
		/// Endpoint for retrieving a paginated list of api users.
		/// </summary>
		/// <param name="page">The page number for pagination (optional).</param>
		/// <param name="pageSize">The number of items per page (optional).</param>
		/// <returns>A paginated list of api users.</returns>
		/// <response code="200">Returns a list of api users wrapped in a <see cref="PagedResponse{ApiUserRoleDto}"/>.</response>
		/// <response code="204">If no apiUsers are found.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpGet]
		[ProducesResponseType(200, Type = typeof(PagedResponse<ApiUserRoleDto>))]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<PagedResponse<ApiUserRoleDto>>> GetApiUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
		{
			var (isValid, correctedPageSize, result) = _paginationValidationService.ValidatePaginationParameters(page, pageSize, maxPageSize);
			if (!isValid)
			{
				return result;
			}
			var apiUsers = await _apiUserService.GetApiUsers(page, correctedPageSize);
			if (apiUsers is null || !apiUsers.Any())
			{
				_logger.LogInformation("Api Users not found.");
				return NoContent();
			}
			var totalItems = await _apiUserService.ApiUsersCount();
			var data = _mapper.Map<IEnumerable<ApiUserRoleDto>>(apiUsers);
			var response = new PagedResponse<ApiUserRoleDto>(data, page, correctedPageSize, totalItems);
			return Ok(response);
		}

		/// <summary>
		/// Endpoint for retrieving a single api user.
		/// </summary>
		/// <param name="id">The ID of the api user to retrieve.</param>
		/// <returns>A single apiUser that matches the specified ID.</returns>
		/// <response code="200">Returns a single api user if found.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="404">If no api user is found.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpGet("{id}")]
		[ProducesResponseType(200, Type = typeof(ApiUserRoleDto))]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<ApiUserRoleDto>> GetApiUser(int id)
		{
			if (!_inputValidationService.IsNonNegativeInt(id))
			{
				_logger.LogInformation("Invalid input. The ID {id} must be a non-negative integer.", id);
				return BadRequest("Invalid input. The ID must be a non-negative integer.");
			}
			if (!await _apiUserService.ApiUserExists(id))
			{
				_logger.LogInformation("Api User with id {id} not found.", id);
				return NotFound();
			}
			var apiUser = await _apiUserService.GetApiUser(id);
			var apiUserRoleDto = _mapper.Map<ApiUserRoleDto>(apiUser);
			return Ok(apiUserRoleDto);
		}

		/// <summary>
		/// Endpoint for retrieving a paginated list of api users containing the specified role.
		/// </summary>
		/// <param name="role">The role to search for.</param>
		/// <param name="page">The page number for pagination (optional).</param>
		/// <param name="pageSize">The size of each page for pagination (optional).</param>
		/// <returns>A list of api users that match the specified role.</returns>
		/// <response code="200">Returns a paged list of api users if found.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="404">If no apiUsers are found.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpGet("byRole/{role}")]
		[ProducesResponseType(200, Type = typeof(PagedResponse<ApiUserRoleDto>))]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<PagedResponse<ApiUserRoleDto>>> GetApiUsersByRole(string role, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
		{
			if (!_inputValidationService.IsValidString(role))
			{
				_logger.LogInformation("Invalid input. The role must be a valid non-empty string.");
				return BadRequest("Invalid input. The role must be a valid non-empty string.");
			}
			var (isValid, correctedPageSize, result) = _paginationValidationService.ValidatePaginationParameters(page, pageSize, maxPageSize);
			if (!isValid)
			{
				return result;
			}
			var apiUsers = await _apiUserService.GetApiUsersByRole(page, correctedPageSize, role);
			if (apiUsers is null || apiUsers.Count == 0)
			{
				_logger.LogInformation("ApiUser with role {role} not found.", role);
				return NotFound();
			}
			var totalItems = await _apiUserService.ApiUsersCount(role);
			var data = _mapper.Map<IEnumerable<ApiUserRoleDto>>(apiUsers);
			var response = new PagedResponse<ApiUserRoleDto>(data, page, correctedPageSize, totalItems);
			return Ok(response);
		}

		/// <summary>
		/// Endpoint for updating a single api user.
		/// </summary>
		/// <param name="id">The ID of the api user to update.</param>
		/// <param name="apiUserRoleDto">The data to update the api user.</param>
		/// <returns>No content.</returns>
		/// <response code="204">Returns no content if successful.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="404">If no apiUser is found.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		/// <response code="403">If the user does not have permission to access the requested resource.</response>
		[HttpPut("{id}")]
		[ProducesResponseType(204)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		[ProducesResponseType(403)]
		public async Task<IActionResult> PutApiUser(int id, ApiUserRoleDto apiUserRoleDto)
		{
			if (!_inputValidationService.IsNonNegativeInt(id))
			{
				_logger.LogInformation("Invalid input. The ID {id} must be a non-negative integer.", id);
				return BadRequest("Invalid input. The ID must be a non-negative integer.");
			}
			if (id != apiUserRoleDto.Id)
			{
				_logger.LogInformation("Api User with id {id} is different from provided Api User and its id.", id);
				return BadRequest();
			}
			if (!await _apiUserService.ApiUserExists(id))
			{
				_logger.LogInformation("Api User with id {id} not found.", id);
				return NotFound();
			}
			var apiUser = _mapper.Map<ApiUserEntity>(apiUserRoleDto);
			await _apiUserService.PutApiUser(apiUser);
			return NoContent();
		}

		/// <summary>
		/// Endpoint for deleting a single api user.
		/// </summary>
		/// <param name="id">The ID of the api user to delete.</param>
		/// <returns>No content.</returns>
		/// <response code="204">Returns no content if successful.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="404">If no apiUser is found.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		/// <response code="403">If the user does not have permission to access the requested resource.</response>
		[HttpDelete("{id}")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		[ProducesResponseType(403)]
		public async Task<IActionResult> DeleteApiUser(int id)
		{
			if (!_inputValidationService.IsNonNegativeInt(id))
			{
				_logger.LogInformation("Invalid input. The ID {id} must be a non-negative integer.", id);
				return BadRequest("Invalid input. The ID must be a non-negative integer.");
			}
			if (!await _apiUserService.ApiUserExists(id))
			{
				_logger.LogInformation("Api User with id {id} not found.", id);
				return NotFound();
			}
			await _apiUserService.DeleteApiUser(id);
			return NoContent();
		}

	}
}
