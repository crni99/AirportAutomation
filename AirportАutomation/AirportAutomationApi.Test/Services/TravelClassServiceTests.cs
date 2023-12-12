using AirportAutomationApi.IRepository;
using AirportAutomationApi.Services;
using Moq;

namespace AirportAutomationApi.Test.Services
{
	public class TravelClassServiceTests
	{
		private readonly Mock<ITravelClassRepository> _repositoryMock;
		private readonly TravelClassService _service;

		public TravelClassServiceTests()
		{
			_repositoryMock = new Mock<ITravelClassRepository>();
			_service = new TravelClassService(_repositoryMock.Object);
		}

		[Fact]
		public async Task GetTravelClasses_Should_Call_Repository_GetTravelClasses()
		{
			await _service.GetTravelClasses(1, 10);

			_repositoryMock.Verify(repo => repo.GetTravelClasses(1, 10), Times.Once);
		}

		[Fact]
		public async Task GetTravelClass_Should_Call_Repository_GetTravelClass()
		{
			await _service.GetTravelClass(1);

			_repositoryMock.Verify(repo => repo.GetTravelClass(1), Times.Once);
		}

		[Fact]
		public void TravelClassExists_Should_Call_Repository_TravelClassExists()
		{
			var result = _service.TravelClassExists(1);

			_repositoryMock.Verify(repo => repo.TravelClassExists(1), Times.Once);
			Assert.False(result);
		}

		[Fact]
		public void TravelClassesCount_ShouldReturnCorrectCount()
		{
			var expectedCount = 5;
			_repositoryMock.Setup(repo => repo.TravelClassesCount()).Returns(expectedCount);

			int count = _service.TravelClassesCount();

			Assert.Equal(expectedCount, count);
		}
	}
}
