using AirportAutomationApi.Entities;
using AirportAutomationApi.IRepository;
using AirportAutomationApi.Services;
using Microsoft.AspNetCore.JsonPatch;
using Moq;

namespace AirportAutomationApi.Test.Services
{
	public class PassengerServiceTests
	{
		private readonly Mock<IPassengerRepository> _repositoryMock;
		private readonly PassengerService _service;

		public PassengerServiceTests()
		{
			_repositoryMock = new Mock<IPassengerRepository>();
			_service = new PassengerService(_repositoryMock.Object);
		}

		[Fact]
		public async Task GetPassengers_Should_Call_Repository_GetPassengers()
		{
			await _service.GetPassengers(1, 10);

			_repositoryMock.Verify(repo => repo.GetPassengers(1, 10), Times.Once);
		}

		[Fact]
		public async Task GetPassenger_Should_Call_Repository_GetPassenger()
		{
			await _service.GetPassenger(1);

			_repositoryMock.Verify(repo => repo.GetPassenger(1), Times.Once);
		}

		[Fact]
		public async Task GetPassengersByName_Should_Call_Repository_GetPassengersByName()
		{
			await _service.GetPassengersByName("John", "Doe");

			_repositoryMock.Verify(repo => repo.GetPassengersByName("John", "Doe"), Times.Once);
		}

		[Fact]
		public async Task PostPassenger_Should_Call_Repository_PostPassenger()
		{
			var passenger = new Passenger();

			await _service.PostPassenger(passenger);

			_repositoryMock.Verify(repo => repo.PostPassenger(passenger), Times.Once);
		}

		[Fact]
		public async Task PutPassenger_Should_Call_Repository_PutPassenger()
		{
			var passenger = new Passenger();

			await _service.PutPassenger(passenger);

			_repositoryMock.Verify(repo => repo.PutPassenger(passenger), Times.Once);
		}

		[Fact]
		public async Task DeletePassenger_Should_Call_Repository_DeletePassenger()
		{
			await _service.DeletePassenger(1);

			_repositoryMock.Verify(repo => repo.DeletePassenger(1), Times.Once);
		}

		[Fact]
		public async Task PassengerExists_Should_Call_Repository_PassengerExistsAsync()
		{
			var result = await _service.PassengerExists(1);

			_repositoryMock.Verify(repo => repo.PassengerExists(1), Times.Once);
			Assert.False(result);
		}

		[Fact]
		public async Task PatchPassenger_Should_Call_Repository_PatchPassenger()
		{
			var passengerDocument = new JsonPatchDocument();

			await _service.PatchPassenger(1, passengerDocument);

			_repositoryMock.Verify(repo => repo.PatchPassenger(1, passengerDocument), Times.Once);
		}

		[Fact]
		public void PassengersCount_ShouldReturnCorrectCount()
		{
			var expectedCount = 5;
			_repositoryMock.Setup(repo => repo.PassengersCount()).Returns(expectedCount);

			int count = _service.PassengersCount();

			Assert.Equal(expectedCount, count);
		}
	}
}

