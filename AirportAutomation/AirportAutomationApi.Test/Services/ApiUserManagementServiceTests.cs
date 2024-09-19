using AirportAutomation.Application.Services;
using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Interfaces.IRepositories;
using Microsoft.AspNetCore.JsonPatch;
using Moq;

namespace AirportAutomationApi.Test.Services
{
	public class ApiUserManagementServiceTests
	{
		private readonly Mock<IApiUserManagementRepository> _repositoryMock;
		private readonly ApiUserManagementService _service;

		public ApiUserManagementServiceTests()
		{
			_repositoryMock = new Mock<IApiUserManagementRepository>();
			_service = new ApiUserManagementService(_repositoryMock.Object);
		}

		[Fact]
		public async Task GetApiUsers_Should_Call_Repository_GetApiUsers()
		{
			await _service.GetApiUsers(1, 10);

			_repositoryMock.Verify(repo => repo.GetApiUsers(1, 10), Times.Once);
		}

		[Fact]
		public async Task GetApiUser_Should_Call_Repository_GetApiUser()
		{
			await _service.GetApiUser(1);

			_repositoryMock.Verify(repo => repo.GetApiUser(1), Times.Once);
		}

		[Fact]
		public async Task GetApiUsersByRole_Should_Call_Repository_GetApiUsersByRole()
		{
			await _service.GetApiUsersByRole(1, 10, "user");

			_repositoryMock.Verify(repo => repo.GetApiUsersByRole(1, 10, "user"), Times.Once);
		}

		[Fact]
		public async Task PutApiUser_Should_Call_Repository_PutApiUser()
		{
			var airline = new ApiUserEntity();

			await _service.PutApiUser(airline);

			_repositoryMock.Verify(repo => repo.PutApiUser(airline), Times.Once);
		}

		[Fact]
		public async Task DeleteApiUser_Should_Call_Repository_DeleteApiUser()
		{
			await _service.DeleteApiUser(1);

			_repositoryMock.Verify(repo => repo.DeleteApiUser(1), Times.Once);
		}

		[Fact]
		public async Task ApiUserExists_Should_Call_Repository_ApiUserExistsAsync()
		{
			var result = await _service.ApiUserExists(1);

			_repositoryMock.Verify(repo => repo.ApiUserExists(1), Times.Once);
			Assert.False(result);
		}

		[Fact]
		public async Task ApiUsersCount_ShouldReturnCorrectCount()
		{
			var expectedCount = 5;
			_repositoryMock.Setup(repo => repo.ApiUsersCount(null)).ReturnsAsync(expectedCount);

			var count = await _service.ApiUsersCount();

			Assert.Equal(expectedCount, count);
		}
	}
}

