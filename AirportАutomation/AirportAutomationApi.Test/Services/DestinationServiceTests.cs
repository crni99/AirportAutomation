using AirportAutomationApi.Entities;
using AirportAutomationApi.IRepository;
using AirportAutomationApi.Services;
using Microsoft.AspNetCore.JsonPatch;
using Moq;

namespace AirportAutomationApi.Test.Services
{
	public class DestinationServiceTests
	{
		private readonly Mock<IDestinationRepository> _repositoryMock;
		private readonly DestinationService _service;

		public DestinationServiceTests()
		{
			_repositoryMock = new Mock<IDestinationRepository>();
			_service = new DestinationService(_repositoryMock.Object);
		}

		[Fact]
		public async Task GetDestinations_Should_Call_Repository_GetDestinations()
		{
			await _service.GetDestinations(1, 10);

			_repositoryMock.Verify(repo => repo.GetDestinations(1, 10), Times.Once);
		}

		[Fact]
		public async Task GetDestination_Should_Call_Repository_GetDestination()
		{
			await _service.GetDestination(1);

			_repositoryMock.Verify(repo => repo.GetDestination(1), Times.Once);
		}

		[Fact]
		public async Task PostDestination_Should_Call_Repository_PostDestination()
		{
			var destination = new Destination();

			await _service.PostDestination(destination);

			_repositoryMock.Verify(repo => repo.PostDestination(destination), Times.Once);
		}

		[Fact]
		public async Task PutDestination_Should_Call_Repository_PutDestination()
		{
			var destination = new Destination();

			await _service.PutDestination(destination);

			_repositoryMock.Verify(repo => repo.PutDestination(destination), Times.Once);
		}

		[Fact]
		public async Task PatchDestination_Should_Call_Repository_PatchDestination()
		{
			var destinationDocument = new JsonPatchDocument();

			await _service.PatchDestination(1, destinationDocument);

			_repositoryMock.Verify(repo => repo.PatchDestination(1, destinationDocument), Times.Once);
		}

		[Fact]
		public async Task DeleteDestination_Should_Call_Repository_DeleteDestination()
		{
			await _service.DeleteDestination(1);

			_repositoryMock.Verify(repo => repo.DeleteDestination(1), Times.Once);
		}

		[Fact]
		public async Task DestinationExists_Should_Call_Repository_DestinationExistsAsync()
		{
			var result = await _service.DestinationExists(1);

			_repositoryMock.Verify(repo => repo.DestinationExists(1), Times.Once);
			Assert.False(result);
		}

		[Fact]
		public void DestinationsCount_ShouldReturnCorrectCount()
		{
			var expectedCount = 5;
			_repositoryMock.Setup(repo => repo.DestinationsCount()).Returns(expectedCount);

			int count = _service.DestinationsCount();

			Assert.Equal(expectedCount, count);
		}
	}
}
