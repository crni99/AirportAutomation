using AirportAutomationDomain.Dtos.ApiUser;
using AirportAutomationDomain.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AirportАutomationApi.Authentication
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthenticationController : ControllerBase
	{
		private readonly IAuthenticationRepository _authenticationRepository;
		private readonly IConfiguration _configuration;
		private readonly IMapper _mapper;
		private readonly ILogger<AuthenticationController> _logger;

		public AuthenticationController(IAuthenticationRepository authenticationRepository, IConfiguration configuration, IMapper mapper, ILogger<AuthenticationController> logger)
		{
			_authenticationRepository = authenticationRepository ?? throw new ArgumentNullException(nameof(authenticationRepository));
			_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		/// <summary>
		/// Authenticates a user with the provided credentials and returns an access token.
		/// </summary>
		/// <param name="apiUserDto">The user's credentials for authentication.</param>
		/// <returns>
		/// An access token if authentication is successful.
		/// </returns>
		/// <response code="200">Returns an access token if authentication is successful.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="401">If authentication fails due to incorrect credentials.</response>
		[HttpPost]
		public ActionResult<string> Authenticate(ApiUserDto apiUserDto)
		{
			var apiUser = _mapper.Map<ApiUser>(apiUserDto);
			var user = _authenticationRepository.ValidateUser(apiUser.UserName, apiUser.Password);

			if (user is null)
			{
				_logger.LogInformation("User with username: {UserName} and password: {Password} don’t have permission to access this resource",
					apiUser.UserName, apiUser.Password);
				return Unauthorized("Provided username or password is incorrect.");
			}
			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Authentication:SecretForKey"]));
			var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

			var claimsForToken = new List<Claim>
			{
				new Claim(ClaimTypes.Name, user.UserName),
				new Claim(ClaimTypes.Role, user.Roles)
			};
			var jwtSecurityToken = new JwtSecurityToken(
				_configuration["Authentication:Issuer"],
				_configuration["Authentication:Audience"],
				claimsForToken,
				DateTime.UtcNow,
				DateTime.UtcNow.AddDays(1),
				signingCredentials);
			var tokenToReturn = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
			return Ok(tokenToReturn);
		}
	}
}
