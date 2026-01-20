using FluentAssertions;
using Moq;
using Users.API.Application.Features.Users.Queries.GetUserById;
using Users.API.Application.Interfaces;
using Users.API.Models;
using Xunit;

namespace Users.API.Tests.Unit.Handlers;

public class GetUserByIdQueryHandlerTests
{
    private readonly Mock<IUserRepository> _mockRepository;
    private readonly GetUserByIdQueryHandler _handler;

    public GetUserByIdQueryHandlerTests()
    {
        _mockRepository = new Mock<IUserRepository>();
        _handler = new GetUserByIdQueryHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_UserExists_ReturnsUser()
    {
        // Arrange
        var userId = 1;
        var query = new GetUserByIdQuery(userId);
        var expectedUser = new User
        {
            Id = userId,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com"
        };

        _mockRepository
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(userId);
        result.FirstName.Should().Be(expectedUser.FirstName);
        _mockRepository.Verify(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UserNotExists_ReturnsNull()
    {
        // Arrange
        var userId = 999;
        var query = new GetUserByIdQuery(userId);

        _mockRepository
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _mockRepository.Verify(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
