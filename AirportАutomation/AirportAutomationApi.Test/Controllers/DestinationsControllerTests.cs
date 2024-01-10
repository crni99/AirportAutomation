using AirportAutomationApi.Dtos.Destination;
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
	public class DestinationsControllerTests
	{
		private readonly DestinationsController _controller;
		private readonly Mock<IDestinationService> _destinationServiceMock;
		private readonly Mock<IPaginationValidationService> _paginationValidationServiceMock;
		private readonly Mock<IMapper> _mapperMock;
		private readonly Mock<ILogger<DestinationsController>> _loggerMock;
		private readonly Mock<IConfiguration> _configurationMock;

		private readonly int page = 1;
		private readonly int pageSize = 10;

		private readonly Destination destination = new()
		{
			Id = 1,
			City = "Belgrade",
			Airport = "Belgrade Nikola Tesla Airport"
		};

		private readonly Destination destination2 = new()
		{
			Id = 1,
			City = "Paris",
			Airport = "Charles de Gaulle Airport"
		};

		private readonly DestinationCreateDto destinationCreateDto = new()
		{
			City = "Belgrade",
			Airport = "Belgrade Nikola Tesla Airport"
		};

		private readonly DestinationDto destinationDto = new()
		{
			Id = 1,
			City = "Belgrade",
			Airport = "Belgrade Nikola Tesla Airport"
		};

		public DestinationsControllerTests()
		{
			_destinationServiceMock = new Mock<IDestinationService>();
			_paginationValidationServiceMock = new Mock<IPaginationValidationService>();
			_mapperMock = new Mock<IMapper>();
			_loggerMock = new Mock<ILogger<DestinationsController>>();
			_configurationMock = new Mock<IConfiguration>();
			var configBuilder = new ConfigurationBuilder();
			configBuilder.AddInMemoryCollection(new Dictionary<string, string>
			{
				{"pageSettings:maxPageSize", "10"}
			});
			_configurationMock.Setup(x => x.GetSection(It.IsAny<string>()))
				.Returns(configBuilder.Build().GetSection(""));

			_controller = new DestinationsController(
				_destinationServiceMock.Object,
				_paginationValidationServiceMock.Object,
				_mapperMock.Object,
				_loggerMock.Object,
				_configurationMock.Object
			);
		}

		[Fact]
		[Trait("Category", "GetDestinations")]
		public async Task GetDestinations_InvalidPaginationParameters_ReturnsBadRequest()
		{
			int invalidPage = -1;
			int invalidPageSize = 0;
			var expectedBadRequestResult = new BadRequestObjectResult("Invalid pagination parameters.");

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(invalidPage, invalidPageSize, It.IsAny<int>()))
				.Returns((false, 0, expectedBadRequestResult));

			var result = await _controller.GetDestinations(invalidPage, invalidPageSize);

			Assert.IsType<BadRequestObjectResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetDestinations")]
		public async Task GetDestinations_ReturnsNoContent_WhenNoDestinationsFound()
		{
			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 0, null));
			_destinationServiceMock.Setup(service => service.GetDestinations(It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync(new List<Destination>());

			var result = await _controller.GetDestinations();

			Assert.IsType<NoContentResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetDestinations")]
		public async Task GetDestinations_ReturnsInternalServerError_WhenExceptionThrown()
		{
			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 0, null));
			_destinationServiceMock.Setup(service => service.GetDestinations(It.IsAny<int>(), It.IsAny<int>()))
				.ThrowsAsync(new Exception("Simulated exception"));

			await Assert.ThrowsAsync<Exception>(async () => await _controller.GetDestinations());
		}

		[Fact]
		[Trait("Category", "GetDestinations")]
		public async Task GetDestinations_ReturnsNoContent_WhenNoData()
		{
			List<Destination> destinations = null;
			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 0, null));
			_destinationServiceMock.Setup(service => service.GetDestinations(page, pageSize))
				.ReturnsAsync(destinations);

			var result = await _controller.GetDestinations(page, pageSize);

			var actionResult = Assert.IsType<ActionResult<PagedResponse<DestinationDto>>>(result);
			Assert.IsType<NoContentResult>(actionResult.Result);
		}

		[Fact]
		[Trait("Category", "GetDestinationById")]
		public async Task GetDestinationById_ReturnsOkResult_WhenDestinationExists()
		{
			var destinationId = 1;
			_destinationServiceMock.Setup(service => service.DestinationExists(destinationId))
				.ReturnsAsync(true);
			_destinationServiceMock.Setup(service => service.GetDestination(destinationId))
				.ReturnsAsync(destination);
			_mapperMock.Setup(mapper => mapper.Map<DestinationDto>(It.IsAny<Destination>()))
				.Returns(new DestinationDto());

			var result = await _controller.GetDestination(destinationId);

			var actionResult = Assert.IsType<ActionResult<DestinationDto>>(result);
			var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			Assert.IsType<DestinationDto>(okResult.Value);
		}

		[Fact]
		[Trait("Category", "GetDestinationById")]
		public async Task GetDestinationById_ReturnsNotFound_WhenDestinationDoesNotExist()
		{
			var destinationId = 2;
			_destinationServiceMock.Setup(service => service.DestinationExists(destinationId))
				.ReturnsAsync(false);

			var result = await _controller.GetDestination(destinationId);

			var actionResult = Assert.IsType<ActionResult<DestinationDto>>(result);
			Assert.IsType<NotFoundResult>(actionResult.Result);
		}

		[Fact]
		[Trait("Category", "GetDestinationById")]
		public async Task GetDestinationById_ThrowsException_WhenServiceThrowsException()
		{
			var destinationId = 1;
			_destinationServiceMock.Setup(service => service.DestinationExists(destinationId))
				.Throws(new Exception("Simulated exception"));

			await Assert.ThrowsAsync<Exception>(async () => await _controller.GetDestination(destinationId));
		}

		[Fact]
		[Trait("Category", "PostDestination")]
		public async Task PostDestination_ReturnsCreatedAtAction_WhenDestinationCreatedSuccessfully()
		{
			_mapperMock.Setup(mapper => mapper.Map<Destination>(destinationCreateDto))
			.Returns(destination);
			_destinationServiceMock.Setup(service => service.PostDestination(It.IsAny<Destination>()))
			.ReturnsAsync(destination);

			var result = await _controller.PostDestination(destinationCreateDto);

			var actionResult = Assert.IsType<ActionResult<DestinationDto>>(result);
			var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
			Assert.Equal("GetDestination", createdAtActionResult.ActionName);
			Assert.Equal(destination.Id, createdAtActionResult.RouteValues["id"]);
			Assert.Equal(destination, createdAtActionResult.Value);
		}

		[Fact]
		[Trait("Category", "PostDestination")]
		public async Task PostDestination_ReturnsNullReferenceException_WhenDestinationCreationFails()
		{
			_destinationServiceMock.Setup(service => service.PostDestination(destination))
				.ThrowsAsync(new NullReferenceException("Simulated exception"));

			await Assert.ThrowsAsync<NullReferenceException>(async () => await _controller.PostDestination(destinationCreateDto));
		}

		[Fact]
		[Trait("Category", "PutDestination")]
		public async Task PutDestination_ValidUpdate_ReturnsNoContentResult()
		{
			var id = 1;
			_destinationServiceMock.Setup(service => service.DestinationExists(id)).ReturnsAsync(true);
			_destinationServiceMock.Setup(service => service.PutDestination(It.IsAny<Destination>())).Returns(Task.CompletedTask);

			var result = await _controller.PutDestination(id, destinationDto);

			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "PutDestination")]
		public async Task PutDestination_DestinationNotFound_ReturnsNotFoundResult()
		{
			var id = 1;
			_destinationServiceMock.Setup(service => service.DestinationExists(id)).ReturnsAsync(false);

			var result = await _controller.PutDestination(id, destinationDto);

			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		[Trait("Category", "PutDestination")]
		public async Task PutDestination_IdsMismatch_ReturnsBadRequestResult()
		{
			var id = 2;

			var result = await _controller.PutDestination(id, destinationDto);

			Assert.IsType<BadRequestResult>(result);
		}

		[Fact]
		[Trait("Category", "PutDestination")]
		public async Task PutDestination_ReturnsNullReferenceException_WhenDestinationUpdateFails()
		{
			var id = 1;
			_destinationServiceMock.Setup(service => service.DestinationExists(id)).ReturnsAsync(true);
			_destinationServiceMock.Setup(service => service.PutDestination(It.IsAny<Destination>()))
				.ThrowsAsync(new Exception("Simulated error"));

			await Assert.ThrowsAsync<Exception>(async () =>
			{
				await _controller.PutDestination(id, destinationDto);
			});
		}

		[Fact]
		[Trait("Category", "PatchDestination")]
		public async Task PatchDestination_ValidUpdate_ReturnsOkResult()
		{
			var id = 1;
			var destinationDocument = new JsonPatchDocument();
			var updatedDestination = new Destination { Id = id, City = "Ljig" };
			_destinationServiceMock.Setup(service => service.DestinationExists(id)).ReturnsAsync(true);
			_destinationServiceMock.Setup(service => service.PatchDestination(id, destinationDocument)).ReturnsAsync(updatedDestination);

			var result = await _controller.PatchDestination(id, destinationDocument);

			var okResult = Assert.IsType<OkObjectResult>(result);
			var returnedDestination = Assert.IsType<Destination>(okResult.Value);
			Assert.Equal(id, returnedDestination.Id);
			Assert.Equal("Ljig", returnedDestination.City);
		}

		[Fact]
		[Trait("Category", "PatchDestination")]
		public async Task PatchDestination_DestinationNotFound_ReturnsNotFoundResult()
		{
			var id = 1;
			var destinationDocument = new JsonPatchDocument();
			_destinationServiceMock.Setup(service => service.DestinationExists(id)).ReturnsAsync(false);

			var result = await _controller.PatchDestination(id, destinationDocument);

			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		[Trait("Category", "PatchDestination")]
		public async Task PatchDestination_ReturnsNullReferenceException_WhenDestinationPatchFails()
		{
			var id = 1;
			_destinationServiceMock.Setup(service => service.DestinationExists(id)).ReturnsAsync(true);
			_destinationServiceMock.Setup(service => service.PatchDestination(id, It.IsAny<JsonPatchDocument>()))
				.ThrowsAsync(new Exception("Simulated error"));

			await Assert.ThrowsAsync<Exception>(async () =>
			{
				await _controller.PatchDestination(id, It.IsAny<JsonPatchDocument>());
			});
		}

		[Fact]
		[Trait("Category", "DeleteDestination")]
		public async Task DeleteDestination_ValidDelete_ReturnsNoContentResult()
		{
			var id = 1;
			_destinationServiceMock.Setup(service => service.DestinationExists(id)).ReturnsAsync(true);
			_destinationServiceMock.Setup(service => service.DeleteDestination(id)).ReturnsAsync(true);

			var result = await _controller.DeleteDestination(id);

			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "DeleteDestination")]
		public async Task DeleteDestination_DestinationNotFound_ReturnsNotFoundResult()
		{
			var id = 1;
			_destinationServiceMock.Setup(service => service.DestinationExists(id)).ReturnsAsync(false);

			var result = await _controller.DeleteDestination(id);

			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		[Trait("Category", "DeleteDestination")]
		public async Task DeleteDestination_DestinationFoundButIsReferenced_ReturnsConflictObjectResult()
		{
			var id = 1;
			_destinationServiceMock.Setup(service => service.DestinationExists(id)).ReturnsAsync(true);
			_destinationServiceMock.Setup(service => service.DeleteDestination(id)).ReturnsAsync(false);

			var result = await _controller.DeleteDestination(id);

			Assert.IsType<ConflictObjectResult>(result);
		}

		[Fact]
		[Trait("Category", "DeleteDestination")]
		public async Task DeleteDestination_ReturnsNullReferenceException_WhenDestinationDeleteFails()
		{
			var id = 1;
			_destinationServiceMock.Setup(service => service.DestinationExists(id)).ReturnsAsync(true);
			_destinationServiceMock.Setup(service => service.DeleteDestination(id))
				.ThrowsAsync(new Exception("Simulated error"));

			await Assert.ThrowsAsync<Exception>(async () =>
			{
				await _controller.DeleteDestination(id);
			});
		}

	}
}