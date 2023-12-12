using AirportAutomationApi.Controllers;
using AirportAutomationApi.Dtos.Passenger;
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
	public class PassengersControllerTests
	{
		private readonly PassengersController _controller;
		private readonly Mock<IPassengerService> _passengerServiceMock;
		private readonly Mock<IPaginationValidationService> _paginationValidationServiceMock;
		private readonly Mock<IMapper> _mapperMock;
		private readonly Mock<ILogger<PassengersController>> _loggerMock;
		private readonly Mock<IConfiguration> _configurationMock;

		private readonly int page = 1;
		private readonly int pageSize = 10;

		private readonly Passenger passenger = new()
		{
			Id = 1,
			FirstName = "Ognjen",
			LastName = "Andjelic",
			UPRN = "1234567890123",
			Passport = "1234567",
			Address = "DD 10",
			Phone = "064"
		};
		private readonly Passenger passenger2 = new()
		{
			Id = 1,
			FirstName = "John",
			LastName = "Wick",
			UPRN = "0987654321123",
			Passport = "7654321",
			Address = "BGD 011",
			Phone = "063"
		};

		private readonly PassengerCreateDto passengerCreateDto = new("Ognjen", "Andjelic", "1234567890123", "1234567", "DD 10", "064");
		private readonly PassengerDto passengerDto = new()
		{
			Id = 1,
			FirstName = "Ognjen",
			LastName = "Andjelic",
			UPRN = "1234567890123",
			Passport = "1234567",
			Address = "DD 10",
			Phone = "064"
		};

		public PassengersControllerTests()
		{
			_passengerServiceMock = new Mock<IPassengerService>();
			_paginationValidationServiceMock = new Mock<IPaginationValidationService>();
			_mapperMock = new Mock<IMapper>();
			_loggerMock = new Mock<ILogger<PassengersController>>();
			_configurationMock = new Mock<IConfiguration>();
			var configBuilder = new ConfigurationBuilder();
			configBuilder.AddInMemoryCollection(new Dictionary<string, string>
			{
				{"pageSettings:maxPageSize", "10"}
			});
			_configurationMock.Setup(x => x.GetSection(It.IsAny<string>()))
				.Returns(configBuilder.Build().GetSection(""));

			_controller = new PassengersController(
				_passengerServiceMock.Object,
				_paginationValidationServiceMock.Object,
				_mapperMock.Object,
				_loggerMock.Object,
				_configurationMock.Object
			);
		}

		[Fact]
		[Trait("Category", "GetPassengers")]
		public async Task GetPassengers_InvalidPaginationParameters_ReturnsBadRequest()
		{
			int invalidPage = -1;
			int invalidPageSize = 0;
			var expectedBadRequestResult = new BadRequestObjectResult("Invalid pagination parameters.");

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(invalidPage, invalidPageSize, It.IsAny<int>()))
				.Returns((false, 0, expectedBadRequestResult));

			var result = await _controller.GetPassengers(invalidPage, invalidPageSize);

			Assert.IsType<BadRequestObjectResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetPassengers")]
		public async Task GetPassengers_ReturnsNoContent_WhenNoPassengersFound()
		{
			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 0, null));
			_passengerServiceMock.Setup(service => service.GetPassengers(It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync(new List<Passenger>());

			var result = await _controller.GetPassengers();

			Assert.IsType<NoContentResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetPassengers")]
		public async Task GetPassengers_ReturnsInternalServerError_WhenExceptionThrown()
		{
			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 0, null));
			_passengerServiceMock.Setup(service => service.GetPassengers(It.IsAny<int>(), It.IsAny<int>()))
				.ThrowsAsync(new Exception("Simulated exception"));

			await Assert.ThrowsAsync<Exception>(async () => await _controller.GetPassengers());
		}

		[Fact]
		[Trait("Category", "GetPassengers")]
		public async Task GetPassengers_ReturnsNoContent_WhenNoData()
		{
			List<Passenger> passengers = null;
			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 0, null));
			_passengerServiceMock.Setup(service => service.GetPassengers(page, pageSize))
				.ReturnsAsync(passengers);

			var result = await _controller.GetPassengers(page, pageSize);

			var actionResult = Assert.IsType<ActionResult<PagedResponse<PassengerDto>>>(result);
			Assert.IsType<NoContentResult>(actionResult.Result);
		}

		[Fact]
		[Trait("Category", "GetPassengerById")]
		public async Task GetPassengerById_ReturnsOkResult_WhenPassengerExists()
		{
			var passengerId = 1;
			_passengerServiceMock.Setup(service => service.PassengerExists(passengerId))
				.Returns(true);
			_passengerServiceMock.Setup(service => service.GetPassenger(passengerId))
				.ReturnsAsync(passenger);
			_mapperMock.Setup(mapper => mapper.Map<PassengerDto>(It.IsAny<Passenger>()))
				.Returns(new PassengerDto());

			var result = await _controller.GetPassenger(passengerId);

			var actionResult = Assert.IsType<ActionResult<PassengerDto>>(result);
			var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			Assert.IsType<PassengerDto>(okResult.Value);
		}

		[Fact]
		[Trait("Category", "GetPassengerById")]
		public async Task GetPassengerById_ReturnsNotFound_WhenPassengerDoesNotExist()
		{
			var passengerId = 2;
			_passengerServiceMock.Setup(service => service.PassengerExists(passengerId))
				.Returns(false);

			var result = await _controller.GetPassenger(passengerId);

			var actionResult = Assert.IsType<ActionResult<PassengerDto>>(result);
			Assert.IsType<NotFoundResult>(actionResult.Result);
		}

		[Fact]
		[Trait("Category", "GetPassengerById")]
		public async Task GetPassengerById_ThrowsException_WhenServiceThrowsException()
		{
			var passengerId = 1;
			_passengerServiceMock.Setup(service => service.PassengerExists(passengerId))
				.Throws(new Exception("Simulated exception"));

			await Assert.ThrowsAsync<Exception>(async () => await _controller.GetPassenger(passengerId));
		}

		[Fact]
		[Trait("Category", "GetPassengerByName")]
		public async Task GetPassengersByName_ReturnsOkResult_WithValidInput()
		{
			var firstName = "Ognjen";
			var lastName = "Andjelic";
			var expectedPassengers = new List<Passenger>
			{
				passenger
			};
			_passengerServiceMock.Setup(service => service.GetPassengersByName(firstName, lastName))
					.ReturnsAsync(expectedPassengers);

			var result = await _controller.GetPassengersByName(firstName, lastName);
			Assert.IsType<ActionResult<IEnumerable<PassengerDto>>>(result);
		}

		[Fact]
		[Trait("Category", "GetPassengerByName")]
		public async Task GetPassengersByName_ReturnsBadRequest_WhenBothNamesAreMissing()
		{
			string firstName = string.Empty;
			string lastName = string.Empty;
			List<Passenger> passengers = null;
			_passengerServiceMock.Setup(service => service.GetPassengersByName(firstName, lastName))
				.ReturnsAsync(passengers);

			var result = await _controller.GetPassengersByName(firstName, lastName);

			var badRequestResult = Assert.IsType<ActionResult<IEnumerable<PassengerDto>>>(result);
			Assert.IsType<BadRequestObjectResult>(badRequestResult.Result);
			Assert.True(string.IsNullOrEmpty(badRequestResult.Value?.ToString()));
		}

		[Fact]
		[Trait("Category", "GetPassengerByName")]
		public async Task GetPassengerByName_ReturnsNotFound_WhenNoData()
		{
			List<Passenger> passengers = null;
			_passengerServiceMock.Setup(service => service.GetPassengersByName(passenger.FirstName, passenger.LastName))
				.ReturnsAsync(passengers);

			var result = await _controller.GetPassengersByName(passenger.FirstName, passenger.LastName);

			var actionResult = Assert.IsType<ActionResult<IEnumerable<PassengerDto>>>(result);
			Assert.IsType<NotFoundResult>(actionResult.Result);
		}

		[Fact]
		[Trait("Category", "GetPassengerByName")]
		public async Task GetPassengerByName_ThrowsException_WhenServiceThrowsException()
		{
			var firstName = "Og";
			var lastName = "An";

			_passengerServiceMock.Setup(service => service.GetPassengersByName(firstName, lastName))
				.ThrowsAsync(new Exception("Simulated exception"));

			await Assert.ThrowsAsync<Exception>(async () => await _controller.GetPassengersByName(firstName, lastName));
		}

		[Fact]
		[Trait("Category", "PostPassenger")]
		public async Task PostPassenger_ReturnsCreatedAtAction_WhenPassengerCreatedSuccessfully()
		{
			_mapperMock.Setup(mapper => mapper.Map<Passenger>(passengerCreateDto))
			.Returns(passenger);
			_passengerServiceMock.Setup(service => service.PostPassenger(It.IsAny<Passenger>()))
			.ReturnsAsync(passenger);

			var result = await _controller.PostPassenger(passengerCreateDto);

			var actionResult = Assert.IsType<ActionResult<Passenger>>(result);
			var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
			Assert.Equal("GetPassenger", createdAtActionResult.ActionName);
			Assert.Equal(passenger.Id, createdAtActionResult.RouteValues["id"]);
			Assert.Equal(passenger, createdAtActionResult.Value);
		}

		[Fact]
		[Trait("Category", "PostPassenger")]
		public async Task PostPassenger_ReturnsNullReferenceException_WhenPassengerCreationFails()
		{
			_passengerServiceMock.Setup(service => service.PostPassenger(passenger))
				.ThrowsAsync(new NullReferenceException("Simulated exception"));

			await Assert.ThrowsAsync<NullReferenceException>(async () => await _controller.PostPassenger(passengerCreateDto));
		}

		[Fact]
		[Trait("Category", "PutPassenger")]
		public async Task PutPassenger_ValidUpdate_ReturnsNoContentResult()
		{
			var id = 1;
			_passengerServiceMock.Setup(service => service.PassengerExists(id)).Returns(true);
			_passengerServiceMock.Setup(service => service.PutPassenger(It.IsAny<Passenger>())).Returns(Task.CompletedTask);

			var result = await _controller.PutPassenger(id, passengerDto);

			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "PutPassenger")]
		public async Task PutPassenger_PassengerNotFound_ReturnsNotFoundResult()
		{
			var id = 1;
			_passengerServiceMock.Setup(service => service.PassengerExists(id)).Returns(false);

			var result = await _controller.PutPassenger(id, passengerDto);

			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		[Trait("Category", "PutPassenger")]
		public async Task PutPassenger_IdsMismatch_ReturnsBadRequestResult()
		{
			var id = 2;

			var result = await _controller.PutPassenger(id, passengerDto);

			Assert.IsType<BadRequestResult>(result);
		}

		[Fact]
		[Trait("Category", "PutPassenger")]
		public async Task PutPassenger_ReturnsNullReferenceException_WhenPassengerUpdateFails()
		{
			var id = 1;
			_passengerServiceMock.Setup(service => service.PassengerExists(id)).Returns(true);
			_passengerServiceMock.Setup(service => service.PutPassenger(It.IsAny<Passenger>()))
				.ThrowsAsync(new Exception("Simulated error"));

			await Assert.ThrowsAsync<Exception>(async () =>
			{
				await _controller.PutPassenger(id, passengerDto);
			});
		}

		[Fact]
		[Trait("Category", "PatchPassenger")]
		public async Task PatchPassenger_ValidUpdate_ReturnsOkResult()
		{
			var id = 1;
			var passengerDocument = new JsonPatchDocument();
			var updatedPassenger = new Passenger { Id = id, FirstName = "OG" };
			_passengerServiceMock.Setup(service => service.PassengerExists(id)).Returns(true);
			_passengerServiceMock.Setup(service => service.PatchPassenger(id, passengerDocument)).ReturnsAsync(updatedPassenger);

			var result = await _controller.PatchPassenger(id, passengerDocument);

			var okResult = Assert.IsType<OkObjectResult>(result);
			var returnedPassenger = Assert.IsType<Passenger>(okResult.Value);
			Assert.Equal(id, returnedPassenger.Id);
			Assert.Equal("OG", returnedPassenger.FirstName);
		}

		[Fact]
		[Trait("Category", "PatchPassenger")]
		public async Task PatchPassenger_PassengerNotFound_ReturnsNotFoundResult()
		{
			var id = 1;
			var passengerDocument = new JsonPatchDocument();
			_passengerServiceMock.Setup(service => service.PassengerExists(id)).Returns(false);

			var result = await _controller.PatchPassenger(id, passengerDocument);

			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		[Trait("Category", "PatchPassenger")]
		public async Task PatchPassenger_ReturnsNullReferenceException_WhenPassengerPatchFails()
		{
			var id = 1;
			_passengerServiceMock.Setup(service => service.PassengerExists(id)).Returns(true);
			_passengerServiceMock.Setup(service => service.PatchPassenger(id, It.IsAny<JsonPatchDocument>()))
				.ThrowsAsync(new Exception("Simulated error"));

			await Assert.ThrowsAsync<Exception>(async () =>
			{
				await _controller.PatchPassenger(id, It.IsAny<JsonPatchDocument>());
			});
		}

		[Fact]
		[Trait("Category", "DeletePassenger")]
		public async Task DeletePassenger_ValidDelete_ReturnsNoContentResult()
		{
			var id = 1;
			_passengerServiceMock.Setup(service => service.PassengerExists(id)).Returns(true);
			_passengerServiceMock.Setup(service => service.DeletePassenger(id)).ReturnsAsync(true);

			var result = await _controller.DeletePassenger(id);

			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "DeletePassenger")]
		public async Task DeletePassenger_PassengerNotFound_ReturnsNotFoundResult()
		{
			var id = 1;
			_passengerServiceMock.Setup(service => service.PassengerExists(id)).Returns(false);

			var result = await _controller.DeletePassenger(id);

			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		[Trait("Category", "DeletePassenger")]
		public async Task DeletePassenger_PassengerFoundButIsReferenced_ReturnsConflictObjectResult()
		{
			var id = 1;
			_passengerServiceMock.Setup(service => service.PassengerExists(id)).Returns(true);
			_passengerServiceMock.Setup(service => service.DeletePassenger(id)).ReturnsAsync(false);

			var result = await _controller.DeletePassenger(id);

			Assert.IsType<ConflictObjectResult>(result);
		}

		[Fact]
		[Trait("Category", "DeletePassenger")]
		public async Task DeletePassenger_ReturnsNullReferenceException_WhenPassengerDeleteFails()
		{
			var id = 1;
			_passengerServiceMock.Setup(service => service.PassengerExists(id)).Returns(true);
			_passengerServiceMock.Setup(service => service.DeletePassenger(id))
				.ThrowsAsync(new Exception("Simulated error"));

			await Assert.ThrowsAsync<Exception>(async () =>
			{
				await _controller.DeletePassenger(id);
			});
		}

	}
}