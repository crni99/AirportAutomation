using AirportAutomationApi.Dtos.Airline;
using AirportAutomationApi.Entities;
using AirportAutomationApi.IService;
using AirportАutomationApi.Controllers;
using AirportАutomationApi.Dtos.Response;
using AirportАutomationApi.IServices;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace AirportAutomationApi.Test.Controllers
{
	public class AirlinesControllerTests
	{
		private readonly AirlinesController _controller;
		private readonly Mock<IAirlineService> _airlineServiceMock;
		private readonly Mock<IPaginationValidationService> _paginationValidationServiceMock;
		private readonly Mock<IMapper> _mapperMock;
		private readonly Mock<ILogger<AirlinesController>> _loggerMock;
		private readonly Mock<IConfiguration> _configurationMock;

		private readonly int page = 1;
		private readonly int pageSize = 10;

		private readonly Airline airline = new()
		{
			Id = 1,
			Name = "Air Serbia"
		};

		private readonly Airline airline2 = new()
		{
			Id = 1,
			Name = "Wizz Air"
		};

		private readonly AirlineCreateDto airlineCreateDto = new()
		{
			Name = "Air Serbia"
		};

		private readonly AirlineDto airlineDto = new()
		{
			Id = 1,
			Name = "Air Serbia"
		};

		public AirlinesControllerTests()
		{
			_airlineServiceMock = new Mock<IAirlineService>();
			_paginationValidationServiceMock = new Mock<IPaginationValidationService>();
			_mapperMock = new Mock<IMapper>();
			_loggerMock = new Mock<ILogger<AirlinesController>>();
			_configurationMock = new Mock<IConfiguration>();
			var configBuilder = new ConfigurationBuilder();
			configBuilder.AddInMemoryCollection(new Dictionary<string, string>
			{
				{"pageSettings:maxPageSize", "10"}
			});
			_configurationMock.Setup(x => x.GetSection(It.IsAny<string>()))
				.Returns(configBuilder.Build().GetSection(""));

			_controller = new AirlinesController(
				_airlineServiceMock.Object,
				_paginationValidationServiceMock.Object,
				_mapperMock.Object,
				_loggerMock.Object,
				_configurationMock.Object
			);
		}

		[Fact]
		[Trait("Category", "GetAirlines")]
		public async Task GetAirlines_InvalidPaginationParameters_ReturnsBadRequest()
		{
			int invalidPage = -1;
			int invalidPageSize = 0;
			var expectedBadRequestResult = new BadRequestObjectResult("Invalid pagination parameters.");

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(invalidPage, invalidPageSize, It.IsAny<int>()))
				.Returns((false, 0, expectedBadRequestResult));

			var result = await _controller.GetAirlines(invalidPage, invalidPageSize);

			Assert.IsType<BadRequestObjectResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetAirlines")]
		public async Task GetAirlines_ReturnsNoContent_WhenNoAirlinesFound()
		{
			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 0, null));
			_airlineServiceMock.Setup(service => service.GetAirlines(It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync(new List<Airline>());

			var result = await _controller.GetAirlines();

			Assert.IsType<NoContentResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetAirlines")]
		public async Task GetAirlines_ReturnsInternalServerError_WhenExceptionThrown()
		{
			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 0, null));
			_airlineServiceMock.Setup(service => service.GetAirlines(It.IsAny<int>(), It.IsAny<int>()))
				.ThrowsAsync(new Exception("Simulated exception"));

			await Assert.ThrowsAsync<Exception>(async () => await _controller.GetAirlines());
		}

		[Fact]
		[Trait("Category", "GetAirlines")]
		public async Task GetAirlines_ReturnsNoContent_WhenNoData()
		{
			List<Airline> airlines = null;
			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 0, null));
			_airlineServiceMock.Setup(service => service.GetAirlines(page, pageSize))
				.ReturnsAsync(airlines);

			var result = await _controller.GetAirlines(page, pageSize);

			var actionResult = Assert.IsType<ActionResult<PagedResponse<AirlineDto>>>(result);
			Assert.IsType<NoContentResult>(actionResult.Result);
		}

		[Fact]
		[Trait("Category", "GetAirlineById")]
		public async Task GetAirlineById_ReturnsOkResult_WhenAirlineExists()
		{
			var airlineId = 1;
			_airlineServiceMock.Setup(service => service.AirlineExists(airlineId))
				.ReturnsAsync(true);
			_airlineServiceMock.Setup(service => service.GetAirline(airlineId))
				.ReturnsAsync(airline);
			_mapperMock.Setup(mapper => mapper.Map<AirlineDto>(It.IsAny<Airline>()))
				.Returns(new AirlineDto());

			var result = await _controller.GetAirline(airlineId);

			var actionResult = Assert.IsType<ActionResult<AirlineDto>>(result);
			var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			Assert.IsType<AirlineDto>(okResult.Value);
		}

		[Fact]
		[Trait("Category", "GetAirlineById")]
		public async Task GetAirlineById_ReturnsNotFound_WhenAirlineDoesNotExist()
		{
			var airlineId = 2;
			_airlineServiceMock.Setup(service => service.AirlineExists(airlineId))
				.ReturnsAsync(false);

			var result = await _controller.GetAirline(airlineId);

			var actionResult = Assert.IsType<ActionResult<AirlineDto>>(result);
			Assert.IsType<NotFoundResult>(actionResult.Result);
		}

		[Fact]
		[Trait("Category", "GetAirlineById")]
		public async Task GetAirlineById_ThrowsException_WhenServiceThrowsException()
		{
			var airlineId = 1;
			_airlineServiceMock.Setup(service => service.AirlineExists(airlineId))
				.Throws(new Exception("Simulated exception"));

			await Assert.ThrowsAsync<Exception>(async () => await _controller.GetAirline(airlineId));
		}

		[Fact]
		[Trait("Category", "GetAirlineByName")]
		public async Task GetAirlinesByName_ReturnsOkResult_WithValidInput()
		{
			var name = "Ognjen";
			var expectedAirlines = new List<Airline>
			{
				airline
			};
			var expectedAirlinesDto = expectedAirlines.Select(a => new AirlineDto
			{
				Id = a.Id,
			});
			_airlineServiceMock.Setup(service => service.GetAirlinesByName(name))
					.ReturnsAsync(expectedAirlines);

			var result = await _controller.GetAirlinesByName(name);

			Assert.IsType<ActionResult<IEnumerable<AirlineDto>>>(result);
		}

		[Fact]
		[Trait("Category", "GetAirlineByName")]
		public async Task GetAirlinesByName_ReturnsBadRequest_WhenBothNamesAreMissing()
		{
			string name = string.Empty;
			List<Airline> airlines = null;
			_airlineServiceMock.Setup(service => service.GetAirlinesByName(name))
				.ReturnsAsync(airlines);

			var result = await _controller.GetAirlinesByName(name);

			var notFoundResult = Assert.IsType<ActionResult<IEnumerable<AirlineDto>>>(result);
			Assert.IsType<NotFoundResult>(notFoundResult.Result);
			Assert.True(string.IsNullOrEmpty(notFoundResult.Value?.ToString()));
		}

		[Fact]
		[Trait("Category", "GetAirlineByName")]
		public async Task GetAirlineByName_ReturnsNotFound_WhenNoData()
		{
			List<Airline> airlines = null;
			_airlineServiceMock.Setup(service => service.GetAirlinesByName(airline.Name))
				.ReturnsAsync(airlines);

			var result = await _controller.GetAirlinesByName(airline.Name);

			var actionResult = Assert.IsType<ActionResult<IEnumerable<AirlineDto>>>(result);
			Assert.IsType<NotFoundResult>(actionResult.Result);
		}

		[Fact]
		[Trait("Category", "GetAirlineByName")]
		public async Task GetAirlineByName_ThrowsException_WhenServiceThrowsException()
		{
			var name = "Og";

			_airlineServiceMock.Setup(service => service.GetAirlinesByName(name))
				.ThrowsAsync(new Exception("Simulated exception"));

			await Assert.ThrowsAsync<Exception>(async () => await _controller.GetAirlinesByName(name));
		}

		[Fact]
		[Trait("Category", "PostAirline")]
		public async Task PostAirline_ReturnsCreatedAtAction_WhenAirlineCreatedSuccessfully()
		{
			_mapperMock.Setup(mapper => mapper.Map<Airline>(airlineCreateDto))
			.Returns(airline);
			_airlineServiceMock.Setup(service => service.PostAirline(It.IsAny<Airline>()))
			.ReturnsAsync(airline);

			var result = await _controller.PostAirline(airlineCreateDto);

			var actionResult = Assert.IsType<ActionResult<Airline>>(result);
			var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
			Assert.Equal("GetAirline", createdAtActionResult.ActionName);
			Assert.Equal(airline.Id, createdAtActionResult.RouteValues["id"]);
			Assert.Equal(airline, createdAtActionResult.Value);
		}

		[Fact]
		[Trait("Category", "PostAirline")]
		public async Task PostAirline_ReturnsNullReferenceException_WhenAirlineCreationFails()
		{
			_airlineServiceMock.Setup(service => service.PostAirline(airline))
				.ThrowsAsync(new NullReferenceException("Simulated exception"));

			await Assert.ThrowsAsync<NullReferenceException>(async () => await _controller.PostAirline(airlineCreateDto));
		}

		[Fact]
		[Trait("Category", "PutAirline")]
		public async Task PutAirline_ValidUpdate_ReturnsNoContentResult()
		{
			var id = 1;
			_airlineServiceMock.Setup(service => service.AirlineExists(id)).ReturnsAsync(true);
			_airlineServiceMock.Setup(service => service.PutAirline(It.IsAny<Airline>())).Returns(Task.CompletedTask);

			var result = await _controller.PutAirline(id, airlineDto);

			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "PutAirline")]
		public async Task PutAirline_AirlineNotFound_ReturnsNotFoundResult()
		{
			var id = 1;
			_airlineServiceMock.Setup(service => service.AirlineExists(id)).ReturnsAsync(false);

			var result = await _controller.PutAirline(id, airlineDto);

			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		[Trait("Category", "PutAirline")]
		public async Task PutAirline_IdsMismatch_ReturnsBadRequestResult()
		{
			var id = 2;

			var result = await _controller.PutAirline(id, airlineDto);

			Assert.IsType<BadRequestResult>(result);
		}

		[Fact]
		[Trait("Category", "PutAirline")]
		public async Task PutAirline_ReturnsNullReferenceException_WhenAirlineUpdateFails()
		{
			var id = 1;
			_airlineServiceMock.Setup(service => service.AirlineExists(id)).ReturnsAsync(true);
			_airlineServiceMock.Setup(service => service.PutAirline(It.IsAny<Airline>()))
				.ThrowsAsync(new Exception("Simulated error"));

			await Assert.ThrowsAsync<Exception>(async () =>
			{
				await _controller.PutAirline(id, airlineDto);
			});
		}

		[Fact]
		[Trait("Category", "PatchAirline")]
		public async Task PatchAirline_ValidUpdate_ReturnsOkResult()
		{
			var id = 1;
			var airlineDocument = new JsonPatchDocument();
			var updatedAirline = new Airline { Id = id, Name = "OG" };
			_airlineServiceMock.Setup(service => service.AirlineExists(id)).ReturnsAsync(true);
			_airlineServiceMock.Setup(service => service.PatchAirline(id, airlineDocument)).ReturnsAsync(updatedAirline);

			var result = await _controller.PatchAirline(id, airlineDocument);

			var okResult = Assert.IsType<OkObjectResult>(result);
			var returnedAirline = Assert.IsType<Airline>(okResult.Value);
			Assert.Equal(id, returnedAirline.Id);
			Assert.Equal("OG", returnedAirline.Name);
		}

		[Fact]
		[Trait("Category", "PatchAirline")]
		public async Task PatchAirline_AirlineNotFound_ReturnsNotFoundResult()
		{
			var id = 1;
			var airlineDocument = new JsonPatchDocument();
			_airlineServiceMock.Setup(service => service.AirlineExists(id)).ReturnsAsync(false);

			var result = await _controller.PatchAirline(id, airlineDocument);

			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		[Trait("Category", "PatchAirline")]
		public async Task PatchAirline_ReturnsNullReferenceException_WhenAirlinePatchFails()
		{
			var id = 1;
			_airlineServiceMock.Setup(service => service.AirlineExists(id)).ReturnsAsync(true);
			_airlineServiceMock.Setup(service => service.PatchAirline(id, It.IsAny<JsonPatchDocument>()))
				.ThrowsAsync(new Exception("Simulated error"));

			await Assert.ThrowsAsync<Exception>(async () =>
			{
				await _controller.PatchAirline(id, It.IsAny<JsonPatchDocument>());
			});
		}

		[Fact]
		[Trait("Category", "DeleteAirline")]
		public async Task DeleteAirline_ValidDelete_ReturnsNoContentResult()
		{
			var id = 1;
			_airlineServiceMock.Setup(service => service.AirlineExists(id)).ReturnsAsync(true);
			_airlineServiceMock.Setup(service => service.DeleteAirline(id)).ReturnsAsync(true);

			var result = await _controller.DeleteAirline(id);

			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "DeleteAirline")]
		public async Task DeleteAirline_AirlineNotFound_ReturnsNotFoundResult()
		{
			var id = 1;
			_airlineServiceMock.Setup(service => service.AirlineExists(id)).ReturnsAsync(false);

			var result = await _controller.DeleteAirline(id);

			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		[Trait("Category", "DeleteAirline")]
		public async Task DeleteAirline_AirlineFoundButIsReferenced_ReturnsConflictObjectResult()
		{
			var id = 1;
			_airlineServiceMock.Setup(service => service.AirlineExists(id)).ReturnsAsync(true);
			_airlineServiceMock.Setup(service => service.DeleteAirline(id)).ReturnsAsync(false);

			var result = await _controller.DeleteAirline(id);

			Assert.IsType<ConflictObjectResult>(result);
		}

		[Fact]
		[Trait("Category", "DeleteAirline")]
		public async Task DeleteAirline_ReturnsNullReferenceException_WhenAirlineDeleteFails()
		{
			var id = 1;
			_airlineServiceMock.Setup(service => service.AirlineExists(id)).ReturnsAsync(true);
			_airlineServiceMock.Setup(service => service.DeleteAirline(id))
				.ThrowsAsync(new Exception("Simulated error"));

			await Assert.ThrowsAsync<Exception>(async () =>
			{
				await _controller.DeleteAirline(id);
			});
		}

	}
}