﻿using AirportAutomationApi.Entities;
using AirportAutomationApi.IRepository;
using AirportAutomationApi.Services;
using Microsoft.AspNetCore.JsonPatch;
using Moq;

namespace AirportAutomationApi.Test.Services
{
	public class FlightServiceTests
	{
		private readonly Mock<IFlightRepository> _repositoryMock;
		private readonly FlightService _service;

		public FlightServiceTests()
		{
			_repositoryMock = new Mock<IFlightRepository>();
			_service = new FlightService(_repositoryMock.Object);
		}


		[Fact]
		public async Task GetFlights_Should_Call_Repository_GetFlights()
		{
			await _service.GetFlights(1, 10);

			_repositoryMock.Verify(repo => repo.GetFlights(1, 10), Times.Once);
		}

		[Fact]
		public async Task GetFlight_Should_Call_Repository_GetFlight()
		{
			await _service.GetFlight(1);

			_repositoryMock.Verify(repo => repo.GetFlight(1), Times.Once);
		}

		[Fact]
		public async Task GetFlightsByName_Should_Call_Repository_GetFlightsByName()
		{
			var startDate = new DateOnly(1999, 12, 01);
			var endDate = new DateOnly(2023, 9, 20);

			await _service.GetFlightsBetweenDates(startDate, endDate);

			_repositoryMock.Verify(repo => repo.GetFlightsBetweenDates(startDate, endDate), Times.Once);
		}

		[Fact]
		public async Task PostFlight_Should_Call_Repository_PostFlight()
		{
			var fLight = new Flight();

			await _service.PostFlight(fLight);

			_repositoryMock.Verify(repo => repo.PostFlight(fLight), Times.Once);
		}

		[Fact]
		public async Task PutFlight_Should_Call_Repository_PutFlight()
		{
			var fLight = new Flight();

			await _service.PutFlight(fLight);

			_repositoryMock.Verify(repo => repo.PutFlight(fLight), Times.Once);
		}

		[Fact]
		public async Task DeleteFlight_Should_Call_Repository_DeleteFlight()
		{
			await _service.DeleteFlight(1);

			_repositoryMock.Verify(repo => repo.DeleteFlight(1), Times.Once);
		}

		[Fact]
		public async Task FlightExists_Should_Call_Repository_FlightExistsAsync()
		{
			var result = await _service.FlightExists(1);

			_repositoryMock.Verify(repo => repo.FlightExists(1), Times.Once);
			Assert.False(result);
		}

		[Fact]
		public async Task PatchFlight_Should_Call_Repository_PatchFlight()
		{
			var fLightDocument = new JsonPatchDocument();

			await _service.PatchFlight(1, fLightDocument);

			_repositoryMock.Verify(repo => repo.PatchFlight(1, fLightDocument), Times.Once);
		}

		[Fact]
		public void FlightsCount_ShouldReturnCorrectCount()
		{
			var expectedCount = 5;
			_repositoryMock.Setup(repo => repo.FlightsCount()).Returns(expectedCount);

			int count = _service.FlightsCount();

			Assert.Equal(expectedCount, count);
		}
	}
}