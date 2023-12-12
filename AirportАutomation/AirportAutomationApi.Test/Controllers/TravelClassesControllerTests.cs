using AirportAutomationApi.Controllers;
using AirportAutomationApi.Dtos.TravelClass;
using AirportAutomationApi.Entities;
using AirportAutomationApi.IService;
using AirportАutomationApi.Dtos.Response;
using AirportАutomationApi.IServices;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace AirportAutomationApi.Test.Controllers
{
	public class TravelClassesControllerTests
	{
		private readonly TravelClassesController _controller;
		private readonly Mock<ITravelClassService> _travelClassServiceMock;
		private readonly Mock<IPaginationValidationService> _paginationValidationServiceMock;
		private readonly Mock<IMapper> _mapperMock;
		private readonly Mock<ILogger<TravelClassesController>> _loggerMock;
		private readonly Mock<IConfiguration> _configurationMock;

		private readonly int page = 1;
		private readonly int pageSize = 10;

		private readonly TravelClass travelClass = new()
		{
			Id = 1,
			Type = "Business"
		};

		private readonly TravelClass travelClass2 = new()
		{
			Id = 2,
			Type = "First"
		};

		public TravelClassesControllerTests()
		{
			_travelClassServiceMock = new Mock<ITravelClassService>();
			_paginationValidationServiceMock = new Mock<IPaginationValidationService>();
			_mapperMock = new Mock<IMapper>();
			_loggerMock = new Mock<ILogger<TravelClassesController>>();
			_configurationMock = new Mock<IConfiguration>();
			var configBuilder = new ConfigurationBuilder();
			configBuilder.AddInMemoryCollection(new Dictionary<string, string>
			{
				{"pageSettings:maxPageSize", "10"}
			});
			_configurationMock.Setup(x => x.GetSection(It.IsAny<string>()))
				.Returns(configBuilder.Build().GetSection(""));

			_controller = new TravelClassesController(
				_travelClassServiceMock.Object,
				_paginationValidationServiceMock.Object,
				_mapperMock.Object,
				_loggerMock.Object,
				_configurationMock.Object
			);
		}

		[Fact]
		[Trait("Category", "GetTravelClasses")]
		public async Task GetTravelClasses_InvalidPaginationParameters_ReturnsBadRequest()
		{
			int invalidPage = -1;
			int invalidPageSize = 0;
			var expectedBadRequestResult = new BadRequestObjectResult("Invalid pagination parameters.");

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(invalidPage, invalidPageSize, It.IsAny<int>()))
				.Returns((false, 0, expectedBadRequestResult));

			var result = await _controller.GetTravelClasses(invalidPage, invalidPageSize);

			Assert.IsType<BadRequestObjectResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetTravelClasses")]
		public async Task GetTravelClasses_ReturnsNoContent_WhenNoTravelClassesFound()
		{
			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 0, null));
			_travelClassServiceMock.Setup(service => service.GetTravelClasses(It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync(new List<TravelClass>());

			var result = await _controller.GetTravelClasses();

			Assert.IsType<NoContentResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetTravelClasses")]
		public async Task GetTravelClasses_ReturnsInternalServerError_WhenExceptionThrown()
		{
			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 0, null));
			_travelClassServiceMock.Setup(service => service.GetTravelClasses(It.IsAny<int>(), It.IsAny<int>()))
				.ThrowsAsync(new Exception("Simulated exception"));

			await Assert.ThrowsAsync<Exception>(async () => await _controller.GetTravelClasses());
		}

		[Fact]
		[Trait("Category", "GetTravelClasses")]
		public async Task GetTravelClasses_ReturnsNoContent_WhenNoData()
		{
			List<TravelClass> travelClasses = null;
			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 0, null));
			_travelClassServiceMock.Setup(service => service.GetTravelClasses(page, pageSize))
				.ReturnsAsync(travelClasses);

			var result = await _controller.GetTravelClasses(page, pageSize);

			var actionResult = Assert.IsType<ActionResult<PagedResponse<TravelClassDto>>>(result);
			Assert.IsType<NoContentResult>(actionResult.Result);
		}

		[Fact]
		[Trait("Category", "GetTravelClassById")]
		public async Task GetTravelClassById_ReturnsOkResult_WhenTravelClassExists()
		{
			var travelClassId = 1;
			_travelClassServiceMock.Setup(service => service.TravelClassExists(travelClassId))
				.Returns(true);
			_travelClassServiceMock.Setup(service => service.GetTravelClass(travelClassId))
				.ReturnsAsync(travelClass);
			_mapperMock.Setup(mapper => mapper.Map<TravelClassDto>(It.IsAny<TravelClass>()))
				.Returns(new TravelClassDto());

			var result = await _controller.GetTravelClass(travelClassId);

			var actionResult = Assert.IsType<ActionResult<TravelClassDto>>(result);
			var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			Assert.IsType<TravelClassDto>(okResult.Value);
		}

		[Fact]
		[Trait("Category", "GetTravelClassById")]
		public async Task GetTravelClassById_ReturnsNotFound_WhenTravelClassDoesNotExist()
		{
			var travelClassId = 2;
			_travelClassServiceMock.Setup(service => service.TravelClassExists(travelClassId))
				.Returns(false);

			var result = await _controller.GetTravelClass(travelClassId);

			var actionResult = Assert.IsType<ActionResult<TravelClassDto>>(result);
			Assert.IsType<NotFoundResult>(actionResult.Result);
		}

		[Fact]
		[Trait("Category", "GetTravelClassById")]
		public async Task GetTravelClassById_ThrowsException_WhenServiceThrowsException()
		{
			var travelClassId = 1;
			_travelClassServiceMock.Setup(service => service.TravelClassExists(travelClassId))
				.Throws(new Exception("Simulated exception"));

			await Assert.ThrowsAsync<Exception>(async () => await _controller.GetTravelClass(travelClassId));
		}
	}
}