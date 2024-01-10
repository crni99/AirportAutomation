using AirportAutomationApi.Dtos.Flight;
using AirportAutomationApi.Entities;
using AirportAutomationApi.IService;
using AirportАutomationApi.Controllers;
using AirportАutomationApi.Dtos.Flight;
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
	public class FlightsControllerTests
	{
		private readonly FlightsController _controller;
		private readonly Mock<IFlightService> _flightServiceMock;
		private readonly Mock<IPaginationValidationService> _paginationValidationServiceMock;
		private readonly Mock<IMapper> _mapperMock;
		private readonly Mock<ILogger<FlightsController>> _loggerMock;
		private readonly Mock<IConfiguration> _configurationMock;

		private readonly int page = 1;
		private readonly int pageSize = 10;

		private readonly Flight flight = new()
		{
			Id = 1,
			DepartureDate = new DateOnly(2023, 09, 20),
			DepartureTime = new TimeOnly(09, 51, 00),
			AirlineId = 1,
			DestinationId = 1,
			PilotId = 1
		};

		private readonly Flight flight2 = new()
		{
			Id = 2,
			DepartureDate = new DateOnly(2023, 09, 20),
			DepartureTime = new TimeOnly(09, 52, 00),
			AirlineId = 2,
			DestinationId = 2,
			PilotId = 2
		};

		private readonly FlightCreateDto flightCreateDto = new()
		{
			DepartureDate = new DateOnly(2023, 09, 20),
			DepartureTime = new TimeOnly(09, 51, 00),
			AirlineId = 1,
			DestinationId = 1,
			PilotId = 1
		};

		private readonly FlightUpdateDto flightUpdateDto = new()
		{
			Id = 1,
			DepartureDate = new DateOnly(2023, 09, 20),
			DepartureTime = new TimeOnly(09, 51, 00),
			AirlineId = 1,
			DestinationId = 1,
			PilotId = 1
		};

		public FlightsControllerTests()
		{
			_flightServiceMock = new Mock<IFlightService>();
			_paginationValidationServiceMock = new Mock<IPaginationValidationService>();
			_mapperMock = new Mock<IMapper>();
			_loggerMock = new Mock<ILogger<FlightsController>>();
			_configurationMock = new Mock<IConfiguration>();
			var configBuilder = new ConfigurationBuilder();
			configBuilder.AddInMemoryCollection(new Dictionary<string, string>
			{
				{"pageSettings:maxPageSize", "10"}
			});
			_configurationMock.Setup(x => x.GetSection(It.IsAny<string>()))
				.Returns(configBuilder.Build().GetSection(""));

			_controller = new FlightsController(
				_flightServiceMock.Object,
				_paginationValidationServiceMock.Object,
				_mapperMock.Object,
				_loggerMock.Object,
				_configurationMock.Object
			);
		}

		[Fact]
		[Trait("Category", "GetFlights")]
		public async Task GetFlights_InvalidPaginationParameters_ReturnsBadRequest()
		{
			int invalidPage = -1;
			int invalidPageSize = 0;
			var expectedBadRequestResult = new BadRequestObjectResult("Invalid pagination parameters.");

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(invalidPage, invalidPageSize, It.IsAny<int>()))
				.Returns((false, 0, expectedBadRequestResult));

			var result = await _controller.GetFlights(invalidPage, invalidPageSize);

			Assert.IsType<BadRequestObjectResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetFlights")]
		public async Task GetFlights_ReturnsNoContent_WhenNoFlightsFound()
		{
			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 0, null));
			_flightServiceMock.Setup(service => service.GetFlights(It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync(new List<Flight>());

			var result = await _controller.GetFlights();

			Assert.IsType<NoContentResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetFlights")]
		public async Task GetFlights_ReturnsInternalServerError_WhenExceptionThrown()
		{
			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 0, null));
			_flightServiceMock.Setup(service => service.GetFlights(It.IsAny<int>(), It.IsAny<int>()))
				.ThrowsAsync(new Exception("Simulated exception"));

			await Assert.ThrowsAsync<Exception>(async () => await _controller.GetFlights());
		}

		[Fact]
		[Trait("Category", "GetFlights")]
		public async Task GetFlights_ReturnsNoContent_WhenNoData()
		{
			List<Flight> flights = null;
			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 0, null));
			_flightServiceMock.Setup(service => service.GetFlights(page, pageSize))
				.ReturnsAsync(flights);

			var result = await _controller.GetFlights(page, pageSize);

			var actionResult = Assert.IsType<ActionResult<PagedResponse<FlightDto>>>(result);
			Assert.IsType<NoContentResult>(actionResult.Result);
		}

		[Fact]
		[Trait("Category", "GetFlightById")]
		public async Task GetFlightById_ReturnsOkResult_WhenFlightExists()
		{
			var flightId = 1;
			_flightServiceMock.Setup(service => service.FlightExists(flightId))
				.ReturnsAsync(true);
			_flightServiceMock.Setup(service => service.GetFlight(flightId))
				.ReturnsAsync(flight);
			_mapperMock.Setup(mapper => mapper.Map<FlightDto>(It.IsAny<Flight>()))
				.Returns(new FlightDto());

			var result = await _controller.GetFlight(flightId);

			var actionResult = Assert.IsType<ActionResult<FlightDto>>(result);
			var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			Assert.IsType<FlightDto>(okResult.Value);
		}

		[Fact]
		[Trait("Category", "GetFlightById")]
		public async Task GetFlightById_ReturnsNotFound_WhenFlightDoesNotExist()
		{
			var flightId = 2;
			_flightServiceMock.Setup(service => service.FlightExists(flightId))
				.ReturnsAsync(false);

			var result = await _controller.GetFlight(flightId);

			var actionResult = Assert.IsType<ActionResult<FlightDto>>(result);
			Assert.IsType<NotFoundResult>(actionResult.Result);
		}

		[Fact]
		[Trait("Category", "GetFlightById")]
		public async Task GetFlightById_ThrowsException_WhenServiceThrowsException()
		{
			var flightId = 1;
			_flightServiceMock.Setup(service => service.FlightExists(flightId))
				.Throws(new Exception("Simulated exception"));

			await Assert.ThrowsAsync<Exception>(async () => await _controller.GetFlight(flightId));
		}

		[Fact]
		[Trait("Category", "GetFlightsBetweenDates")]
		public async Task GetFlightsBetweenDates_ReturnsOkResult_WithValidInput()
		{
			var startDate = new DateOnly(1999, 12, 01);
			var endDate = new DateOnly(2024, 12, 01);
			var expectedFlights = new List<Flight>
			{
				flight
			};
			_flightServiceMock.Setup(service => service.GetFlightsBetweenDates(startDate, endDate))
					.ReturnsAsync(expectedFlights);

			var result = await _controller.GetFlightsBetweenDates(startDate, endDate);
			Assert.IsType<ActionResult<IEnumerable<FlightDto>>>(result);
		}

		[Fact]
		[Trait("Category", "GetFlightsBetweenDates")]
		public async Task GetFlightsBetweenDates_ReturnsBadRequest_WhenBothDatesAreMissing()
		{
			DateOnly? startDate = null;
			DateOnly? endDate = null;
			List<Flight> flights = null;
			_flightServiceMock.Setup(service => service.GetFlightsBetweenDates(startDate, endDate))
					.ReturnsAsync(flights);

			var result = await _controller.GetFlightsBetweenDates(startDate, endDate);

			var badRequestResult = Assert.IsType<ActionResult<IEnumerable<FlightDto>>>(result);
			Assert.IsType<BadRequestObjectResult>(badRequestResult.Result);
			Assert.True(string.IsNullOrEmpty(badRequestResult.Value?.ToString()));
		}

		[Fact]
		[Trait("Category", "GetFlightsBetweenDates")]
		public async Task GetFlightsBetweenDates_ReturnsNotFound_WhenNoData()
		{
			List<Flight> flights = null;
			_flightServiceMock.Setup(service => service.GetFlightsBetweenDates(flight.DepartureDate, flight2.DepartureDate))
				.ReturnsAsync(flights);

			var result = await _controller.GetFlightsBetweenDates(flight.DepartureDate, flight2.DepartureDate);

			var actionResult = Assert.IsType<ActionResult<IEnumerable<FlightDto>>>(result);
			Assert.IsType<NotFoundResult>(actionResult.Result);
		}

		[Fact]
		[Trait("Category", "GetFlightsBetweenDates")]
		public async Task GetFlightsBetweenDates_ThrowsException_WhenServiceThrowsException()
		{
			var startDate = DateOnly.MinValue;
			var endDate = DateOnly.MinValue;

			_flightServiceMock.Setup(service => service.GetFlightsBetweenDates(startDate, endDate))
				.ThrowsAsync(new Exception("Simulated exception"));

			await Assert.ThrowsAsync<Exception>(async () => await _controller.GetFlightsBetweenDates(startDate, endDate));
		}

		[Fact]
		[Trait("Category", "PostFlight")]
		public async Task PostFlight_ReturnsCreatedAtAction_WhenFlightCreatedSuccessfully()
		{
			_mapperMock.Setup(mapper => mapper.Map<Flight>(flightCreateDto))
			.Returns(flight);
			_flightServiceMock.Setup(service => service.PostFlight(It.IsAny<Flight>()))
			.ReturnsAsync(flight);

			var result = await _controller.PostFlight(flightCreateDto);

			var actionResult = Assert.IsType<ActionResult<Flight>>(result);
			var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
			Assert.Equal("GetFlight", createdAtActionResult.ActionName);
			Assert.Equal(flight.Id, createdAtActionResult.RouteValues["id"]);
			Assert.Equal(flight, createdAtActionResult.Value);
		}

		[Fact]
		[Trait("Category", "PostFlight")]
		public async Task PostFlight_ReturnsNullReferenceException_WhenFlightCreationFails()
		{
			_flightServiceMock.Setup(service => service.PostFlight(flight))
				.ThrowsAsync(new NullReferenceException("Simulated exception"));

			await Assert.ThrowsAsync<NullReferenceException>(async () => await _controller.PostFlight(flightCreateDto));
		}

		[Fact]
		[Trait("Category", "PutFlight")]
		public async Task PutFlight_ValidUpdate_ReturnsNoContentResult()
		{
			var id = 1;
			_flightServiceMock.Setup(service => service.FlightExists(id)).ReturnsAsync(true);
			_flightServiceMock.Setup(service => service.PutFlight(It.IsAny<Flight>())).Returns(Task.CompletedTask);

			var result = await _controller.PutFlight(id, flightUpdateDto);

			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "PutFlight")]
		public async Task PutFlight_FlightNotFound_ReturnsNotFoundResult()
		{
			var id = 1;
			_flightServiceMock.Setup(service => service.FlightExists(id)).ReturnsAsync(false);

			var result = await _controller.PutFlight(id, flightUpdateDto);

			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		[Trait("Category", "PutFlight")]
		public async Task PutFlight_IdsMismatch_ReturnsBadRequestResult()
		{
			var id = 2;

			var result = await _controller.PutFlight(id, flightUpdateDto);

			Assert.IsType<BadRequestResult>(result);
		}

		[Fact]
		[Trait("Category", "PutFlight")]
		public async Task PutFlight_ReturnsNullReferenceException_WhenFlightUpdateFails()
		{
			var id = 1;
			_flightServiceMock.Setup(service => service.FlightExists(id)).ReturnsAsync(true);
			_flightServiceMock.Setup(service => service.PutFlight(It.IsAny<Flight>()))
				.ThrowsAsync(new Exception("Simulated error"));

			await Assert.ThrowsAsync<Exception>(async () =>
			{
				await _controller.PutFlight(id, flightUpdateDto);
			});
		}

		[Fact]
		[Trait("Category", "PatchFlight")]
		public async Task PatchFlight_ValidUpdate_ReturnsOkResult()
		{
			var id = 1;
			var flightDocument = new JsonPatchDocument();
			var updatedFlight = new Flight { Id = id, DepartureDate = new DateOnly(2022, 12, 01) };
			_flightServiceMock.Setup(service => service.FlightExists(id)).ReturnsAsync(true);
			_flightServiceMock.Setup(service => service.PatchFlight(id, flightDocument)).ReturnsAsync(updatedFlight);

			var result = await _controller.PatchFlight(id, flightDocument);

			var okResult = Assert.IsType<OkObjectResult>(result);
			var returnedFlight = Assert.IsType<Flight>(okResult.Value);
			Assert.Equal(id, returnedFlight.Id);
			Assert.Equal(new DateOnly(2022, 12, 01), returnedFlight.DepartureDate);
		}

		[Fact]
		[Trait("Category", "PatchFlight")]
		public async Task PatchFlight_FlightNotFound_ReturnsNotFoundResult()
		{
			var id = 1;
			var flightDocument = new JsonPatchDocument();
			_flightServiceMock.Setup(service => service.FlightExists(id)).ReturnsAsync(false);

			var result = await _controller.PatchFlight(id, flightDocument);

			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		[Trait("Category", "PatchFlight")]
		public async Task PatchFlight_ReturnsNullReferenceException_WhenFlightPatchFails()
		{
			var id = 1;
			_flightServiceMock.Setup(service => service.FlightExists(id)).ReturnsAsync(true);
			_flightServiceMock.Setup(service => service.PatchFlight(id, It.IsAny<JsonPatchDocument>()))
				.ThrowsAsync(new Exception("Simulated error"));

			await Assert.ThrowsAsync<Exception>(async () =>
			{
				await _controller.PatchFlight(id, It.IsAny<JsonPatchDocument>());
			});
		}

		[Fact]
		[Trait("Category", "DeleteFlight")]
		public async Task DeleteFlight_ValidDelete_ReturnsNoContentResult()
		{
			var id = 1;
			_flightServiceMock.Setup(service => service.FlightExists(id)).ReturnsAsync(true);
			_flightServiceMock.Setup(service => service.DeleteFlight(id)).ReturnsAsync(true);

			var result = await _controller.DeleteFlight(id);

			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "DeleteFlight")]
		public async Task DeleteFlight_FlightNotFound_ReturnsNotFoundResult()
		{
			var id = 1;
			_flightServiceMock.Setup(service => service.FlightExists(id)).ReturnsAsync(false);

			var result = await _controller.DeleteFlight(id);

			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		[Trait("Category", "DeleteFlight")]
		public async Task DeleteFlight_FlightFoundButIsReferenced_ReturnsConflictObjectResult()
		{
			var id = 1;
			_flightServiceMock.Setup(service => service.FlightExists(id)).ReturnsAsync(true);
			_flightServiceMock.Setup(service => service.DeleteFlight(id)).ReturnsAsync(false);

			var result = await _controller.DeleteFlight(id);

			Assert.IsType<ConflictObjectResult>(result);
		}

		[Fact]
		[Trait("Category", "DeleteFlight")]
		public async Task DeleteFlight_ReturnsNullReferenceException_WhenFlightDeleteFails()
		{
			var id = 1;
			_flightServiceMock.Setup(service => service.FlightExists(id)).ReturnsAsync(true);
			_flightServiceMock.Setup(service => service.DeleteFlight(id))
				.ThrowsAsync(new Exception("Simulated error"));

			await Assert.ThrowsAsync<Exception>(async () =>
			{
				await _controller.DeleteFlight(id);
			});
		}

	}
}