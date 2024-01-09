using AirportAutomationApi.Entities;
using AirportAutomationApi.IRepository;
using AirportAutomationApi.Services;
using Microsoft.AspNetCore.JsonPatch;
using Moq;

namespace AirportAutomationApi.Test.Services
{
	public class PilotServiceTests
	{
		private readonly Mock<IPilotRepository> _repositoryMock;
		private readonly PilotService _service;

		public PilotServiceTests()
		{
			_repositoryMock = new Mock<IPilotRepository>();
			_service = new PilotService(_repositoryMock.Object);
		}

		[Fact]
		public async Task GetPilots_Should_Call_Repository_GetPilots()
		{
			await _service.GetPilots(1, 10);

			_repositoryMock.Verify(repo => repo.GetPilots(1, 10), Times.Once);
		}

		[Fact]
		public async Task GetPilot_Should_Call_Repository_GetPilot()
		{
			await _service.GetPilot(1);

			_repositoryMock.Verify(repo => repo.GetPilot(1), Times.Once);
		}

		[Fact]
		public async Task GetPilotsByName_Should_Call_Repository_GetPilotsByName()
		{
			await _service.GetPilotsByName("John", "Doe");

			_repositoryMock.Verify(repo => repo.GetPilotsByName("John", "Doe"), Times.Once);
		}

		[Fact]
		public async Task PostPilot_Should_Call_Repository_PostPilot()
		{
			var pilot = new Pilot();

			await _service.PostPilot(pilot);

			_repositoryMock.Verify(repo => repo.PostPilot(pilot), Times.Once);
		}

		[Fact]
		public async Task PutPilot_Should_Call_Repository_PutPilot()
		{
			var pilot = new Pilot();

			await _service.PutPilot(pilot);

			_repositoryMock.Verify(repo => repo.PutPilot(pilot), Times.Once);
		}

		[Fact]
		public async Task PatchPilot_Should_Call_Repository_PatchPilot()
		{
			var pilotDocument = new JsonPatchDocument();

			await _service.PatchPilot(1, pilotDocument);

			_repositoryMock.Verify(repo => repo.PatchPilot(1, pilotDocument), Times.Once);
		}

		[Fact]
		public async Task DeletePilot_Should_Call_Repository_DeletePilot()
		{
			await _service.DeletePilot(1);

			_repositoryMock.Verify(repo => repo.DeletePilot(1), Times.Once);
		}

		[Fact]
		public async Task PilotExists_Should_Call_Repository_PilotExists()
		{
			var result = await _service.PilotExists(1);

			_repositoryMock.Verify(repo => repo.PilotExists(1), Times.Once);
			Assert.False(result);
		}

		[Fact]
		public void PilotsCount_ShouldReturnCorrectCount()
		{
			var expectedCount = 5;
			_repositoryMock.Setup(repo => repo.PilotsCount()).Returns(expectedCount);

			int count = _service.PilotsCount();

			Assert.Equal(expectedCount, count);
		}
	}
}
