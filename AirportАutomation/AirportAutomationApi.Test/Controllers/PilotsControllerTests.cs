using AirportAutomationApi.Controllers;
using AirportAutomationApi.Dtos.Pilot;
using AirportAutomationApi.Entities;
using AirportAutomationApi.IService;
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
	public class PilotsControllerTests
	{
		private readonly PilotsController _controller;
		private readonly Mock<IPilotService> _pilotServiceMock;
		private readonly Mock<IPaginationValidationService> _paginationValidationServiceMock;
		private readonly Mock<IMapper> _mapperMock;
		private readonly Mock<ILogger<PilotsController>> _loggerMock;
		private readonly Mock<IConfiguration> _configurationMock;

		private readonly int page = 1;
		private readonly int pageSize = 10;

		private readonly Pilot pilot = new()
		{
			Id = 1,
			FirstName = "Ognjen",
			LastName = "Andjelic",
			UPRN = "1234567890123",
			FlyingHours = 100
		};

		private readonly Pilot pilot2 = new()
		{
			Id = 1,
			FirstName = "Alex",
			LastName = "Walker",
			UPRN = "1234567890123",
			FlyingHours = 300
		};

		private readonly PilotCreateDto pilotCreateDto = new()
		{
			FirstName = "Ognjen",
			LastName = "Andjelic",
			UPRN = "1234567890123",
			FlyingHours = 100
		};

		private readonly PilotDto pilotDto = new()
		{
			Id = 1,
			FirstName = "Ognjen",
			LastName = "Andjelic",
			UPRN = "1234567890123",
			FlyingHours = 100
		};

		public PilotsControllerTests()
		{
			_pilotServiceMock = new Mock<IPilotService>();
			_paginationValidationServiceMock = new Mock<IPaginationValidationService>();
			_mapperMock = new Mock<IMapper>();
			_loggerMock = new Mock<ILogger<PilotsController>>();
			_configurationMock = new Mock<IConfiguration>();
			var configBuilder = new ConfigurationBuilder();
			configBuilder.AddInMemoryCollection(new Dictionary<string, string>
			{
				{"pageSettings:maxPageSize", "10"}
			});
			_configurationMock.Setup(x => x.GetSection(It.IsAny<string>()))
				.Returns(configBuilder.Build().GetSection(""));

			_controller = new PilotsController(
				_pilotServiceMock.Object,
				_paginationValidationServiceMock.Object,
				_mapperMock.Object,
				_loggerMock.Object,
				_configurationMock.Object
			);
		}

		[Fact]
		[Trait("Category", "GetPilots")]
		public async Task GetPilots_InvalidPaginationParameters_ReturnsBadRequest()
		{
			int invalidPage = -1;
			int invalidPageSize = 0;
			var expectedBadRequestResult = new BadRequestObjectResult("Invalid pagination parameters.");

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(invalidPage, invalidPageSize, It.IsAny<int>()))
				.Returns((false, 0, expectedBadRequestResult));

			var result = await _controller.GetPilots(invalidPage, invalidPageSize);

			Assert.IsType<BadRequestObjectResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetPilots")]
		public async Task GetPilots_ReturnsNoContent_WhenNoPilotsFound()
		{
			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 0, null));
			_pilotServiceMock.Setup(service => service.GetPilots(It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync(new List<Pilot>());

			var result = await _controller.GetPilots();

			Assert.IsType<NoContentResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetPilots")]
		public async Task GetPilots_ReturnsInternalServerError_WhenExceptionThrown()
		{
			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 0, null));
			_pilotServiceMock.Setup(service => service.GetPilots(It.IsAny<int>(), It.IsAny<int>()))
				.ThrowsAsync(new Exception("Simulated exception"));

			await Assert.ThrowsAsync<Exception>(async () => await _controller.GetPilots());
		}

		[Fact]
		[Trait("Category", "GetPilots")]
		public async Task GetPilots_ReturnsNoContent_WhenNoData()
		{
			List<Pilot> pilots = null;
			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 0, null));
			_pilotServiceMock.Setup(service => service.GetPilots(page, pageSize))
				.ReturnsAsync(pilots);

			var result = await _controller.GetPilots(page, pageSize);

			var actionResult = Assert.IsType<ActionResult<PagedResponse<PilotDto>>>(result);
			Assert.IsType<NoContentResult>(actionResult.Result);
		}

		[Fact]
		[Trait("Category", "GetPilotById")]
		public async Task GetPilotById_ReturnsOkResult_WhenPilotExists()
		{
			var pilotId = 1;
			_pilotServiceMock.Setup(service => service.PilotExists(pilotId))
				.ReturnsAsync(true);
			_pilotServiceMock.Setup(service => service.GetPilot(pilotId))
				.ReturnsAsync(pilot);
			_mapperMock.Setup(mapper => mapper.Map<PilotDto>(It.IsAny<Pilot>()))
				.Returns(new PilotDto());

			var result = await _controller.GetPilot(pilotId);

			var actionResult = Assert.IsType<ActionResult<PilotDto>>(result);
			var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			Assert.IsType<PilotDto>(okResult.Value);
		}

		[Fact]
		[Trait("Category", "GetPilotById")]
		public async Task GetPilotById_ReturnsNotFound_WhenPilotDoesNotExist()
		{
			var pilotId = 2;
			_pilotServiceMock.Setup(service => service.PilotExists(pilotId))
				.ReturnsAsync(false);

			var result = await _controller.GetPilot(pilotId);

			var actionResult = Assert.IsType<ActionResult<PilotDto>>(result);
			Assert.IsType<NotFoundResult>(actionResult.Result);
		}

		[Fact]
		[Trait("Category", "GetPilotById")]
		public async Task GetPilotById_ThrowsException_WhenServiceThrowsException()
		{
			var pilotId = 1;
			_pilotServiceMock.Setup(service => service.PilotExists(pilotId))
				.Throws(new Exception("Simulated exception"));

			await Assert.ThrowsAsync<Exception>(async () => await _controller.GetPilot(pilotId));
		}

		[Fact]
		[Trait("Category", "GetPilotByName")]
		public async Task GetPilotsByName_ReturnsOkResult_WithValidInput()
		{
			var firstName = "Ognjen";
			var lastName = "Andjelic";
			var expectedPilots = new List<Pilot>
			{
				pilot
			};
			_pilotServiceMock.Setup(service => service.GetPilotsByName(firstName, lastName))
					.ReturnsAsync(expectedPilots);

			var result = await _controller.GetPilotsByName(firstName, lastName);

			Assert.IsType<ActionResult<IEnumerable<PilotDto>>>(result);
		}

		[Fact]
		[Trait("Category", "GetPilotByName")]
		public async Task GetPilotsByName_ReturnsBadRequest_WhenBothNamesAreMissing()
		{
			string firstName = string.Empty;
			string lastName = string.Empty;
			List<Pilot> pilots = null;
			_pilotServiceMock.Setup(service => service.GetPilotsByName(firstName, lastName))
				.ReturnsAsync(pilots);

			var result = await _controller.GetPilotsByName(firstName, lastName);

			var badRequestResult = Assert.IsType<ActionResult<IEnumerable<PilotDto>>>(result);
			Assert.IsType<BadRequestObjectResult>(badRequestResult.Result);
			Assert.True(string.IsNullOrEmpty(badRequestResult.Value?.ToString()));
		}

		[Fact]
		[Trait("Category", "GetPilotByName")]
		public async Task GetPilotByName_ReturnsNotFound_WhenNoData()
		{
			List<Pilot> pilots = null;
			_pilotServiceMock.Setup(service => service.GetPilotsByName(pilot.FirstName, pilot.LastName))
				.ReturnsAsync(pilots);

			var result = await _controller.GetPilotsByName(pilot.FirstName, pilot.LastName);

			var actionResult = Assert.IsType<ActionResult<IEnumerable<PilotDto>>>(result);
			Assert.IsType<NotFoundResult>(actionResult.Result);
		}

		[Fact]
		[Trait("Category", "GetPilotByName")]
		public async Task GetPilotByName_ThrowsException_WhenServiceThrowsException()
		{
			var firstName = "Og";
			var lastName = "An";

			_pilotServiceMock.Setup(service => service.GetPilotsByName(firstName, lastName))
				.ThrowsAsync(new Exception("Simulated exception"));

			await Assert.ThrowsAsync<Exception>(async () => await _controller.GetPilotsByName(firstName, lastName));
		}

		[Fact]
		[Trait("Category", "PostPilot")]
		public async Task PostPilot_ReturnsCreatedAtAction_WhenPilotCreatedSuccessfully()
		{
			_mapperMock.Setup(mapper => mapper.Map<Pilot>(pilotCreateDto))
			.Returns(pilot);
			_pilotServiceMock.Setup(service => service.PostPilot(It.IsAny<Pilot>()))
			.ReturnsAsync(pilot);

			var result = await _controller.PostPilot(pilotCreateDto);

			var actionResult = Assert.IsType<ActionResult<Pilot>>(result);
			var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
			Assert.Equal("GetPilot", createdAtActionResult.ActionName);
			Assert.Equal(pilot.Id, createdAtActionResult.RouteValues["id"]);
			Assert.Equal(pilot, createdAtActionResult.Value);
		}

		[Fact]
		[Trait("Category", "PostPilot")]
		public async Task PostPilot_ReturnsNullReferenceException_WhenPilotCreationFails()
		{
			_pilotServiceMock.Setup(service => service.PostPilot(pilot))
				.ThrowsAsync(new NullReferenceException("Simulated exception"));

			await Assert.ThrowsAsync<NullReferenceException>(async () => await _controller.PostPilot(pilotCreateDto));
		}

		[Fact]
		[Trait("Category", "PutPilot")]
		public async Task PutPilot_ValidUpdate_ReturnsNoContentResult()
		{
			var id = 1;
			_pilotServiceMock.Setup(service => service.PilotExists(id)).ReturnsAsync(true);
			_pilotServiceMock.Setup(service => service.PutPilot(It.IsAny<Pilot>())).Returns(Task.CompletedTask);

			var result = await _controller.PutPilot(id, pilotDto);

			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "PutPilot")]
		public async Task PutPilot_PilotNotFound_ReturnsNotFoundResult()
		{
			var id = 1;
			_pilotServiceMock.Setup(service => service.PilotExists(id)).ReturnsAsync(false);

			var result = await _controller.PutPilot(id, pilotDto);

			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		[Trait("Category", "PutPilot")]
		public async Task PutPilot_IdsMismatch_ReturnsBadRequestResult()
		{
			var id = 2;

			var result = await _controller.PutPilot(id, pilotDto);

			Assert.IsType<BadRequestResult>(result);
		}

		[Fact]
		[Trait("Category", "PutPilot")]
		public async Task PutPilot_ReturnsNullReferenceException_WhenPilotUpdateFails()
		{
			var id = 1;
			_pilotServiceMock.Setup(service => service.PilotExists(id)).ReturnsAsync(true);
			_pilotServiceMock.Setup(service => service.PutPilot(It.IsAny<Pilot>()))
				.ThrowsAsync(new Exception("Simulated error"));

			await Assert.ThrowsAsync<Exception>(async () =>
			{
				await _controller.PutPilot(id, pilotDto);
			});
		}

		[Fact]
		[Trait("Category", "PatchPilot")]
		public async Task PatchPilot_ValidUpdate_ReturnsOkResult()
		{
			var id = 1;
			var pilotDocument = new JsonPatchDocument();
			var updatedPilot = new Pilot { Id = id, FirstName = "OG" };
			_pilotServiceMock.Setup(service => service.PilotExists(id)).ReturnsAsync(true);
			_pilotServiceMock.Setup(service => service.PatchPilot(id, pilotDocument)).ReturnsAsync(updatedPilot);

			var result = await _controller.PatchPilot(id, pilotDocument);

			var okResult = Assert.IsType<OkObjectResult>(result);
			var returnedPilot = Assert.IsType<Pilot>(okResult.Value);
			Assert.Equal(id, returnedPilot.Id);
			Assert.Equal("OG", returnedPilot.FirstName);
		}

		[Fact]
		[Trait("Category", "PatchPilot")]
		public async Task PatchPilot_PilotNotFound_ReturnsNotFoundResult()
		{
			var id = 1;
			var pilotDocument = new JsonPatchDocument();
			_pilotServiceMock.Setup(service => service.PilotExists(id)).ReturnsAsync(false);

			var result = await _controller.PatchPilot(id, pilotDocument);

			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		[Trait("Category", "PatchPilot")]
		public async Task PatchPilot_ReturnsNullReferenceException_WhenPilotPatchFails()
		{
			var id = 1;
			_pilotServiceMock.Setup(service => service.PilotExists(id)).ReturnsAsync(true);
			_pilotServiceMock.Setup(service => service.PatchPilot(id, It.IsAny<JsonPatchDocument>()))
				.ThrowsAsync(new Exception("Simulated error"));

			await Assert.ThrowsAsync<Exception>(async () =>
			{
				await _controller.PatchPilot(id, It.IsAny<JsonPatchDocument>());
			});
		}

		[Fact]
		[Trait("Category", "DeletePilot")]
		public async Task DeletePilot_ValidDelete_ReturnsNoContentResult()
		{
			var id = 1;
			_pilotServiceMock.Setup(service => service.PilotExists(id)).ReturnsAsync(true);
			_pilotServiceMock.Setup(service => service.DeletePilot(id)).ReturnsAsync(true);

			var result = await _controller.DeletePilot(id);

			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "DeletePilot")]
		public async Task DeletePilot_PilotNotFound_ReturnsNotFoundResult()
		{
			var id = 1;
			_pilotServiceMock.Setup(service => service.PilotExists(id)).ReturnsAsync(false);

			var result = await _controller.DeletePilot(id);

			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		[Trait("Category", "DeletePilot")]
		public async Task DeletePilot_PilotFoundButIsReferenced_ReturnsConflictObjectResult()
		{
			var id = 1;
			_pilotServiceMock.Setup(service => service.PilotExists(id)).ReturnsAsync(true);
			_pilotServiceMock.Setup(service => service.DeletePilot(id)).ReturnsAsync(false);

			var result = await _controller.DeletePilot(id);

			Assert.IsType<ConflictObjectResult>(result);
		}

		[Fact]
		[Trait("Category", "DeletePilot")]
		public async Task DeletePilot_ReturnsNullReferenceException_WhenPilotDeleteFails()
		{
			var id = 1;
			_pilotServiceMock.Setup(service => service.PilotExists(id)).ReturnsAsync(true);
			_pilotServiceMock.Setup(service => service.DeletePilot(id))
				.ThrowsAsync(new Exception("Simulated error"));

			await Assert.ThrowsAsync<Exception>(async () =>
			{
				await _controller.DeletePilot(id);
			});
		}

	}
}