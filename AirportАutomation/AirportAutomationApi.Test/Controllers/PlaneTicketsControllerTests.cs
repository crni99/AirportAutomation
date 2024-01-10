using AirportAutomationApi.Dtos.PlaneTicket;
using AirportAutomationApi.Entities;
using AirportAutomationApi.IService;
using AirportАutomationApi.Controllers;
using AirportАutomationApi.Dtos.PlaneTicket;
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
	public class PlaneTicketsControllerTests
	{
		private readonly PlaneTicketsController _controller;
		private readonly Mock<IPlaneTicketService> _planeTicketServiceMock;
		private readonly Mock<IPaginationValidationService> _paginationValidationServiceMock;
		private readonly Mock<IMapper> _mapperMock;
		private readonly Mock<ILogger<PlaneTicketsController>> _loggerMock;
		private readonly Mock<IConfiguration> _configurationMock;

		private readonly int page = 1;
		private readonly int pageSize = 10;

		private readonly PlaneTicket planeTicket = new()
		{
			Id = 1,
			Price = 200,
			PurchaseDate = new DateOnly(2023, 09, 20),
			SeatNumber = 1,
			PassengerId = 1,
			TravelClassId = 1,
			FlightId = 1
		};

		private readonly PlaneTicket planeTicket2 = new()
		{
			Id = 2,
			Price = 400,
			PurchaseDate = new DateOnly(2023, 09, 20),
			SeatNumber = 2,
			PassengerId = 2,
			TravelClassId = 2,
			FlightId = 2
		};

		private readonly PlaneTicketCreateDto planeTicketCreateDto = new()
		{
			Price = 200,
			PurchaseDate = new DateOnly(2023, 09, 20),
			SeatNumber = 1,
			PassengerId = 1,
			TravelClassId = 1,
			FlightId = 1
		};

		private readonly PlaneTicketDto planeTicketDto = new()
		{
			Id = 1,
			Price = 200,
			PurchaseDate = new DateOnly(2023, 09, 20),
			SeatNumber = 1,
			PassengerId = 1,
			TravelClassId = 1,
			FlightId = 1
		};

		private readonly PlaneTicketUpdateDto planeTicketUpdateDto = new()
		{
			Id = 1,
			Price = 200,
			PurchaseDate = new DateOnly(2023, 09, 20),
			SeatNumber = 1,
			PassengerId = 1,
			TravelClassId = 1,
			FlightId = 1
		};

		public PlaneTicketsControllerTests()
		{
			_planeTicketServiceMock = new Mock<IPlaneTicketService>();
			_paginationValidationServiceMock = new Mock<IPaginationValidationService>();
			_mapperMock = new Mock<IMapper>();
			_loggerMock = new Mock<ILogger<PlaneTicketsController>>();
			_configurationMock = new Mock<IConfiguration>();
			var configBuilder = new ConfigurationBuilder();
			configBuilder.AddInMemoryCollection(new Dictionary<string, string>
			{
				{"pageSettings:maxPageSize", "10"}
			});
			_configurationMock.Setup(x => x.GetSection(It.IsAny<string>()))
				.Returns(configBuilder.Build().GetSection(""));

			_controller = new PlaneTicketsController(
				_planeTicketServiceMock.Object,
				_paginationValidationServiceMock.Object,
				_mapperMock.Object,
				_loggerMock.Object,
				_configurationMock.Object
			);
		}

		[Fact]
		[Trait("Category", "GetPlaneTickets")]
		public async Task GetPlaneTickets_InvalidPaginationParameters_ReturnsBadRequest()
		{
			int invalidPage = -1;
			int invalidPageSize = 0;
			var expectedBadRequestResult = new BadRequestObjectResult("Invalid pagination parameters.");

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(invalidPage, invalidPageSize, It.IsAny<int>()))
				.Returns((false, 0, expectedBadRequestResult));

			var result = await _controller.GetPlaneTickets(invalidPage, invalidPageSize);

			Assert.IsType<BadRequestObjectResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetPlaneTickets")]
		public async Task GetPlaneTickets_ReturnsNoContent_WhenNoPlaneTicketsFound()
		{
			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 0, null));
			_planeTicketServiceMock.Setup(service => service.GetPlaneTickets(It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync(new List<PlaneTicket>());

			var result = await _controller.GetPlaneTickets();

			Assert.IsType<NoContentResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetPlaneTickets")]
		public async Task GetPlaneTickets_ReturnsInternalServerError_WhenExceptionThrown()
		{
			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 0, null));
			_planeTicketServiceMock.Setup(service => service.GetPlaneTickets(It.IsAny<int>(), It.IsAny<int>()))
				.ThrowsAsync(new Exception("Simulated exception"));

			await Assert.ThrowsAsync<Exception>(async () => await _controller.GetPlaneTickets());
		}

		[Fact]
		[Trait("Category", "GetPlaneTickets")]
		public async Task GetPlaneTickets_ReturnsNoContent_WhenNoData()
		{
			List<PlaneTicket> planeTickets = null;
			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 0, null));
			_planeTicketServiceMock.Setup(service => service.GetPlaneTickets(page, pageSize))
				.ReturnsAsync(planeTickets);

			var result = await _controller.GetPlaneTickets(page, pageSize);

			var actionResult = Assert.IsType<ActionResult<PagedResponse<PlaneTicketDto>>>(result);
			Assert.IsType<NoContentResult>(actionResult.Result);
		}

		[Fact]
		[Trait("Category", "GetPlaneTicketById")]
		public async Task GetPlaneTicketById_ReturnsOkResult_WhenPlaneTicketExists()
		{
			var planeTicketId = 1;
			_planeTicketServiceMock.Setup(service => service.PlaneTicketExists(planeTicketId))
				.ReturnsAsync(true);
			_planeTicketServiceMock.Setup(service => service.GetPlaneTicket(planeTicketId))
				.ReturnsAsync(planeTicket);
			_mapperMock.Setup(mapper => mapper.Map<PlaneTicketDto>(It.IsAny<PlaneTicket>()))
				.Returns(new PlaneTicketDto());

			var result = await _controller.GetPlaneTicket(planeTicketId);

			var actionResult = Assert.IsType<ActionResult<PlaneTicketDto>>(result);
			var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			Assert.IsType<PlaneTicketDto>(okResult.Value);
		}

		[Fact]
		[Trait("Category", "GetPlaneTicketById")]
		public async Task GetPlaneTicketById_ReturnsNotFound_WhenPlaneTicketDoesNotExist()
		{
			var planeTicketId = 2;
			_planeTicketServiceMock.Setup(service => service.PlaneTicketExists(planeTicketId))
				.ReturnsAsync(false);

			var result = await _controller.GetPlaneTicket(planeTicketId);

			var actionResult = Assert.IsType<ActionResult<PlaneTicketDto>>(result);
			Assert.IsType<NotFoundResult>(actionResult.Result);
		}

		[Fact]
		[Trait("Category", "GetPlaneTicketById")]
		public async Task GetPlaneTicketById_ThrowsException_WhenServiceThrowsException()
		{
			var planeTicketId = 1;
			_planeTicketServiceMock.Setup(service => service.PlaneTicketExists(planeTicketId))
				.Throws(new Exception("Simulated exception"));

			await Assert.ThrowsAsync<Exception>(async () => await _controller.GetPlaneTicket(planeTicketId));
		}

		[Fact]
		[Trait("Category", "GetPlaneTicketsForPrice")]
		public async Task GetPlaneTicketsForPrice_ReturnsOkResult_WithValidInput()
		{
			var minPrice = 1;
			var maxPrice = 1000;
			var expectedPlaneTickets = new List<PlaneTicket>
			{
				planeTicket
			};
			var expectedPlaneTicketDto = expectedPlaneTickets.Select(a => new PlaneTicketDto
			{
				Id = a.Id,
			});

			_planeTicketServiceMock.Setup(service => service.GetPlaneTicketsForPrice(minPrice, maxPrice))
					.ReturnsAsync(expectedPlaneTickets);

			var result = await _controller.GetPlaneTicketsForPrice(minPrice, maxPrice);

			Assert.IsType<ActionResult<IEnumerable<PlaneTicketDto>>>(result);
		}

		[Fact]
		[Trait("Category", "GetPlaneTicketsForPrice")]
		public async Task GetPlaneTicketsForPrice_ReturnsBadRequest_WhenBothPricesAreMissing()
		{
			int? minPrice = null;
			int? maxPrice = null;
			List<PlaneTicket> planeTickets = null;
			_planeTicketServiceMock.Setup(service => service.GetPlaneTicketsForPrice(minPrice, maxPrice))
				.ReturnsAsync(planeTickets);

			var result = await _controller.GetPlaneTicketsForPrice(minPrice, maxPrice);

			var badRequestObjectResult = Assert.IsType<ActionResult<IEnumerable<PlaneTicketDto>>>(result);
			Assert.IsType<BadRequestObjectResult>(badRequestObjectResult.Result);
			Assert.True(string.IsNullOrEmpty(badRequestObjectResult.Value?.ToString()));
		}

		[Fact]
		[Trait("Category", "GetPlaneTicketsForPrice")]
		public async Task GetPlaneTicketsForPrice_ReturnsNoContent_WhenNoData()
		{
			List<PlaneTicket> planeTickets = null;
			_planeTicketServiceMock.Setup(service => service.GetPlaneTicketsForPrice(page, pageSize))
				.ReturnsAsync(planeTickets);

			var result = await _controller.GetPlaneTicketsForPrice(page, pageSize);

			var actionResult = Assert.IsType<ActionResult<IEnumerable<PlaneTicketDto>>>(result);
			Assert.IsType<NoContentResult>(actionResult.Result);
		}

		[Fact]
		[Trait("Category", "GetPlaneTicketsForPrice")]
		public async Task GetPlaneTicketsForPrice_ThrowsException_WhenServiceThrowsException()
		{
			int minPrice = 1;
			int maxPrice = 2;

			_planeTicketServiceMock.Setup(service => service.GetPlaneTicketsForPrice(minPrice, maxPrice))
				.ThrowsAsync(new Exception("Simulated exception"));

			await Assert.ThrowsAsync<Exception>(async () => await _controller.GetPlaneTicketsForPrice(minPrice, maxPrice));
		}

		[Fact]
		[Trait("Category", "PostPlaneTicket")]
		public async Task PostPlaneTicket_ReturnsCreatedAtAction_WhenPlaneTicketCreatedSuccessfully()
		{
			_mapperMock.Setup(mapper => mapper.Map<PlaneTicket>(planeTicketCreateDto))
			.Returns(planeTicket);
			_planeTicketServiceMock.Setup(service => service.PostPlaneTicket(It.IsAny<PlaneTicket>()))
			.ReturnsAsync(planeTicket);

			var result = await _controller.PostPlaneTicket(planeTicketCreateDto);

			var actionResult = Assert.IsType<ActionResult<PlaneTicket>>(result);
			var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
			Assert.Equal("GetPlaneTicket", createdAtActionResult.ActionName);
			Assert.Equal(planeTicket.Id, createdAtActionResult.RouteValues["id"]);
			Assert.Equal(planeTicket, createdAtActionResult.Value);
		}

		[Fact]
		[Trait("Category", "PostPlaneTicket")]
		public async Task PostPlaneTicket_ReturnsNullReferenceException_WhenPlaneTicketCreationFails()
		{
			_planeTicketServiceMock.Setup(service => service.PostPlaneTicket(planeTicket))
				.ThrowsAsync(new NullReferenceException("Simulated exception"));

			await Assert.ThrowsAsync<NullReferenceException>(async () => await _controller.PostPlaneTicket(planeTicketCreateDto));
		}

		[Fact]
		[Trait("Category", "PutPlaneTicket")]
		public async Task PutPlaneTicket_ValidUpdate_ReturnsNoContentResult()
		{
			var id = 1;
			_planeTicketServiceMock.Setup(service => service.PlaneTicketExists(id)).ReturnsAsync(true);
			_planeTicketServiceMock.Setup(service => service.PutPlaneTicket(It.IsAny<PlaneTicket>())).Returns(Task.CompletedTask);

			var result = await _controller.PutPlaneTicket(id, planeTicketUpdateDto);

			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "PutPlaneTicket")]
		public async Task PutPlaneTicket_PlaneTicketNotFound_ReturnsNotFoundResult()
		{
			var id = 1;
			_planeTicketServiceMock.Setup(service => service.PlaneTicketExists(id)).ReturnsAsync(false);

			var result = await _controller.PutPlaneTicket(id, planeTicketUpdateDto);

			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		[Trait("Category", "PutPlaneTicket")]
		public async Task PutPlaneTicket_IdsMismatch_ReturnsBadRequestResult()
		{
			var id = 2;

			var result = await _controller.PutPlaneTicket(id, planeTicketUpdateDto);

			Assert.IsType<BadRequestResult>(result);
		}

		[Fact]
		[Trait("Category", "PutPlaneTicket")]
		public async Task PutPlaneTicket_ReturnsNullReferenceException_WhenPlaneTicketUpdateFails()
		{
			var id = 1;
			_planeTicketServiceMock.Setup(service => service.PlaneTicketExists(id)).ReturnsAsync(true);
			_planeTicketServiceMock.Setup(service => service.PutPlaneTicket(It.IsAny<PlaneTicket>()))
				.ThrowsAsync(new Exception("Simulated error"));

			await Assert.ThrowsAsync<Exception>(async () =>
			{
				await _controller.PutPlaneTicket(id, planeTicketUpdateDto);
			});
		}

		[Fact]
		[Trait("Category", "PatchPlaneTicket")]
		public async Task PatchPlaneTicket_ValidUpdate_ReturnsOkResult()
		{
			var id = 1;
			var planeTicketDocument = new JsonPatchDocument();
			var updatedPlaneTicket = new PlaneTicket { Id = id, Price = 600 };
			_planeTicketServiceMock.Setup(service => service.PlaneTicketExists(id)).ReturnsAsync(true);
			_planeTicketServiceMock.Setup(service => service.PatchPlaneTicket(id, planeTicketDocument)).ReturnsAsync(updatedPlaneTicket);

			var result = await _controller.PatchPlaneTicket(id, planeTicketDocument);

			var okResult = Assert.IsType<OkObjectResult>(result);
			var returnedPlaneTicket = Assert.IsType<PlaneTicket>(okResult.Value);
			Assert.Equal(id, returnedPlaneTicket.Id);
			Assert.Equal(600, returnedPlaneTicket.Price);
		}

		[Fact]
		[Trait("Category", "PatchPlaneTicket")]
		public async Task PatchPlaneTicket_PlaneTicketNotFound_ReturnsNotFoundResult()
		{
			var id = 1;
			var planeTicketDocument = new JsonPatchDocument();
			_planeTicketServiceMock.Setup(service => service.PlaneTicketExists(id)).ReturnsAsync(false);

			var result = await _controller.PatchPlaneTicket(id, planeTicketDocument);

			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		[Trait("Category", "PatchPlaneTicket")]
		public async Task PatchPlaneTicket_ReturnsNullReferenceException_WhenPlaneTicketPatchFails()
		{
			var id = 1;
			_planeTicketServiceMock.Setup(service => service.PlaneTicketExists(id)).ReturnsAsync(true);
			_planeTicketServiceMock.Setup(service => service.PatchPlaneTicket(id, It.IsAny<JsonPatchDocument>()))
				.ThrowsAsync(new Exception("Simulated error"));

			await Assert.ThrowsAsync<Exception>(async () =>
			{
				await _controller.PatchPlaneTicket(id, It.IsAny<JsonPatchDocument>());
			});
		}

		[Fact]
		[Trait("Category", "DeletePlaneTicket")]
		public async Task DeletePlaneTicket_ValidDelete_ReturnsNoContentResult()
		{
			var id = 1;
			_planeTicketServiceMock.Setup(service => service.PlaneTicketExists(id)).ReturnsAsync(true);
			_planeTicketServiceMock.Setup(service => service.DeletePlaneTicket(id)).Returns(Task.CompletedTask);

			var result = await _controller.DeletePlaneTicket(id);

			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "DeletePlaneTicket")]
		public async Task DeletePlaneTicket_PlaneTicketNotFound_ReturnsNotFoundResult()
		{
			var id = 1;
			_planeTicketServiceMock.Setup(service => service.PlaneTicketExists(id)).ReturnsAsync(false);

			var result = await _controller.DeletePlaneTicket(id);

			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		[Trait("Category", "DeletePlaneTicket")]
		public async Task DeletePlaneTicket_ReturnsNullReferenceException_WhenPlaneTicketDeleteFails()
		{
			var id = 1;
			_planeTicketServiceMock.Setup(service => service.PlaneTicketExists(id)).ReturnsAsync(true);
			_planeTicketServiceMock.Setup(service => service.DeletePlaneTicket(id))
				.ThrowsAsync(new Exception("Simulated error"));

			await Assert.ThrowsAsync<Exception>(async () =>
			{
				await _controller.DeletePlaneTicket(id);
			});
		}

	}
}