using AirportAutomationApi.Entities;
using AirportAutomationApi.IRepository;
using AirportAutomationApi.Services;
using Microsoft.AspNetCore.JsonPatch;
using Moq;

namespace AirportAutomationApi.Test.Services
{
	public class AirlineServiceTests
	{
		private readonly Mock<IAirlineRepository> _repositoryMock;
		private readonly AirlineService _service;

		public AirlineServiceTests()
		{
			_repositoryMock = new Mock<IAirlineRepository>();
			_service = new AirlineService(_repositoryMock.Object);
		}

		[Fact]
		public async Task GetAirlines_Should_Call_Repository_GetAirlines()
		{
			await _service.GetAirlines(1, 10);

			_repositoryMock.Verify(repo => repo.GetAirlines(1, 10), Times.Once);
		}

		[Fact]
		public async Task GetAirline_Should_Call_Repository_GetAirline()
		{
			await _service.GetAirline(1);

			_repositoryMock.Verify(repo => repo.GetAirline(1), Times.Once);
		}

		[Fact]
		public async Task GetAirlinesByName_Should_Call_Repository_GetAirlinesByName()
		{
			await _service.GetAirlinesByName("Sample Airline");

			_repositoryMock.Verify(repo => repo.GetAirlinesByName("Sample Airline"), Times.Once);
		}

		[Fact]
		public async Task PostAirline_Should_Call_Repository_PostAirline()
		{
			var airline = new Airline();

			await _service.PostAirline(airline);

			_repositoryMock.Verify(repo => repo.PostAirline(airline), Times.Once);
		}

		[Fact]
		public async Task PutAirline_Should_Call_Repository_PutAirline()
		{
			var airline = new Airline();

			await _service.PutAirline(airline);

			_repositoryMock.Verify(repo => repo.PutAirline(airline), Times.Once);
		}

		[Fact]
		public async Task PatchAirline_Should_Call_Repository_PatchAirline()
		{
			var airlineDocument = new JsonPatchDocument();

			await _service.PatchAirline(1, airlineDocument);

			_repositoryMock.Verify(repo => repo.PatchAirline(1, airlineDocument), Times.Once);
		}

		[Fact]
		public async Task DeleteAirline_Should_Call_Repository_DeleteAirline()
		{
			await _service.DeleteAirline(1);

			_repositoryMock.Verify(repo => repo.DeleteAirline(1), Times.Once);
		}

		[Fact]
		public async Task AirlineExists_Should_Call_Repository_AirlineExistsAsync()
		{
			var result = await _service.AirlineExists(1);

			_repositoryMock.Verify(repo => repo.AirlineExists(1), Times.Once);
			Assert.False(result);
		}

		[Fact]
		public void AirlinesCount_ShouldReturnCorrectCount()
		{
			var expectedCount = 5;
			_repositoryMock.Setup(repo => repo.AirlinesCount()).Returns(expectedCount);

			int count = _service.AirlinesCount();

			Assert.Equal(expectedCount, count);
		}
	}
}

