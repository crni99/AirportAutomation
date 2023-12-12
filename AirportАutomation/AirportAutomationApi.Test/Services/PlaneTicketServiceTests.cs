using AirportAutomationApi.Entities;
using AirportAutomationApi.IRepository;
using AirportAutomationApi.Services;
using Microsoft.AspNetCore.JsonPatch;
using Moq;

namespace AirportAutomationApi.Test.Services
{
	public class PlaneTicketServiceTests
	{
		private readonly Mock<IPlaneTicketRepository> _repositoryMock;
		private readonly PlaneTicketService _service;

		public PlaneTicketServiceTests()
		{
			_repositoryMock = new Mock<IPlaneTicketRepository>();
			_service = new PlaneTicketService(_repositoryMock.Object);
		}

		[Fact]
		public async Task GetPlaneTickets_Should_Call_Repository_GetPlaneTickets()
		{
			await _service.GetPlaneTickets(1, 10);

			_repositoryMock.Verify(repo => repo.GetPlaneTickets(1, 10), Times.Once);
		}

		[Fact]
		public async Task GetPlaneTicket_Should_Call_Repository_GetPlaneTicket()
		{
			await _service.GetPlaneTicket(1);

			_repositoryMock.Verify(repo => repo.GetPlaneTicket(1), Times.Once);
		}

		[Fact]
		public async Task GetPlaneTicketsForPrice_Should_Call_Repository_GetPlaneTicketsForPrice()
		{
			await _service.GetPlaneTicketsForPrice(1, 10);

			_repositoryMock.Verify(repo => repo.GetPlaneTicketsForPrice(1, 10), Times.Once);
		}

		[Fact]
		public async Task PostPlaneTicket_Should_Call_Repository_PostPlaneTicket()
		{
			var pilot = new PlaneTicket();

			await _service.PostPlaneTicket(pilot);

			_repositoryMock.Verify(repo => repo.PostPlaneTicket(pilot), Times.Once);
		}

		[Fact]
		public async Task PutPlaneTicket_Should_Call_Repository_PutPlaneTicket()
		{
			var pilot = new PlaneTicket();

			await _service.PutPlaneTicket(pilot);

			_repositoryMock.Verify(repo => repo.PutPlaneTicket(pilot), Times.Once);
		}

		[Fact]
		public async Task PatchPlaneTicket_Should_Call_Repository_PatchPlaneTicket()
		{
			var pilotDocument = new JsonPatchDocument();

			await _service.PatchPlaneTicket(1, pilotDocument);

			_repositoryMock.Verify(repo => repo.PatchPlaneTicket(1, pilotDocument), Times.Once);
		}

		[Fact]
		public async Task DeletePlaneTicket_Should_Call_Repository_DeletePlaneTicket()
		{
			await _service.DeletePlaneTicket(1);

			_repositoryMock.Verify(repo => repo.DeletePlaneTicket(1), Times.Once);
		}

		[Fact]
		public void PlaneTicketExists_Should_Call_Repository_PlaneTicketExists()
		{
			var result = _service.PlaneTicketExists(1);

			_repositoryMock.Verify(repo => repo.PlaneTicketExists(1), Times.Once);
			Assert.False(result);
		}

		[Fact]
		public void PlaneTicketClassesCount_ShouldReturnCorrectCount()
		{
			var expectedCount = 5;
			_repositoryMock.Setup(repo => repo.PlaneTicketsCount()).Returns(expectedCount);

			int count = _service.PlaneTicketsCount();

			Assert.Equal(expectedCount, count);
		}
	}
}
