using KeepItUp.MagJob.Identity.Core.UserAggregate;
using KeepItUp.MagJob.Identity.IntegrationTests.Infrastructure;
using KeepItUp.MagJob.Identity.UseCases.Users.Commands.UpdateUserProfilePicture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace KeepItUp.MagJob.Identity.IntegrationTests.UseCases.Users.Commands;

/// <summary>
/// Integration tests for UpdateUserProfilePictureCommandHandler.
/// Tests the complete flow from command to database update of user profile picture.
/// </summary>
public class UpdateUserProfilePictureCommandHandlerTests : BaseIntegrationTest
{
    public UpdateUserProfilePictureCommandHandlerTests(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
    }

    public class Handle : UpdateUserProfilePictureCommandHandlerTests
    {
        public Handle(DatabaseFixture databaseFixture) : base(databaseFixture)
        {
        }

        [Fact]
        public async Task Should_UpdateProfilePicture_When_ValidImageProvided()
        {
            // Arrange
            var user = User.Create(
                "John",
                "Doe",
                "john.doe@example.com",
                "johndoe",
                Guid.NewGuid());

            await DbContext.Users.AddAsync(user);
            await SaveAndClearAsync();

            var imageFile = CreateTestImageFile("test.jpg", "image/jpeg");

            var command = new UpdateUserProfilePictureCommand
            {
                UserId = user.Id,
                ProfilePictureFile = imageFile,
                CurrentUserId = user.Id
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNullOrEmpty();
            result.Value.Should().Contain("test");

            // Verify user profile picture was updated
            var updatedUser = await DbContext.Users.FindAsync(user.Id);
            updatedUser.Should().NotBeNull();
            updatedUser!.Profile?.ProfileImage.Should().NotBeNullOrEmpty();
            updatedUser.Profile?.ProfileImage.Should().Contain("test");
        }

        [Fact]
        public async Task Should_ReturnNotFound_When_UserDoesNotExist()
        {
            // Arrange
            var nonExistentUserId = Guid.NewGuid();
            var imageFile = CreateTestImageFile("test.jpg", "image/jpeg");

            var command = new UpdateUserProfilePictureCommand
            {
                UserId = nonExistentUserId,
                ProfilePictureFile = imageFile,
                CurrentUserId = nonExistentUserId
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain($"User with ID {nonExistentUserId} not found.");
        }

        [Fact]
        public async Task Should_ReturnUnauthorized_When_UserTriesToUpdateAnotherUsersProfile()
        {
            // Arrange
            var user1 = User.Create(
                "John",
                "Doe",
                "john.doe@example.com",
                "johndoe",
                Guid.NewGuid());

            var user2 = User.Create(
                "Jane",
                "Smith",
                "jane.smith@example.com",
                "janesmith",
                Guid.NewGuid());

            await DbContext.Users.AddRangeAsync(user1, user2);
            await SaveAndClearAsync();

            var imageFile = CreateTestImageFile("test.jpg", "image/jpeg");

            var command = new UpdateUserProfilePictureCommand
            {
                UserId = user1.Id,
                ProfilePictureFile = imageFile,
                CurrentUserId = user2.Id // Different user
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain("User can only update their own profile picture.");
        }

        [Fact]
        public async Task Should_ReturnError_When_InvalidFileTypeProvided()
        {
            // Arrange
            var user = User.Create(
                "John",
                "Doe",
                "john.doe@example.com",
                "johndoe",
                Guid.NewGuid());

            await DbContext.Users.AddAsync(user);
            await SaveAndClearAsync();

            var invalidFile = CreateTestImageFile("test.txt", "text/plain");

            var command = new UpdateUserProfilePictureCommand
            {
                UserId = user.Id,
                ProfilePictureFile = invalidFile,
                CurrentUserId = user.Id
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain("Only image/jpeg, image/jpg, image/png, image/gif, image/webp files are allowed for profile picture.");
        }

        [Fact]
        public async Task Should_ReturnError_When_FileTooLarge()
        {
            // Arrange
            var user = User.Create(
                "John",
                "Doe",
                "john.doe@example.com",
                "johndoe",
                Guid.NewGuid());

            await DbContext.Users.AddAsync(user);
            await SaveAndClearAsync();

            var largeFile = CreateTestImageFile("large.jpg", "image/jpeg", 10 * 1024 * 1024); // 10MB

            var command = new UpdateUserProfilePictureCommand
            {
                UserId = user.Id,
                ProfilePictureFile = largeFile,
                CurrentUserId = user.Id
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain("profile picture file size cannot exceed 5MB.");
        }

        [Fact]
        public async Task Should_ReplaceExistingProfilePicture_When_UserAlreadyHasOne()
        {
            // Arrange
            var user = User.Create(
                "John",
                "Doe",
                "john.doe@example.com",
                "johndoe",
                Guid.NewGuid());

            user.UpdateProfile(null, null, "old-picture.jpg");

            await DbContext.Users.AddAsync(user);
            await SaveAndClearAsync();

            var newImageFile = CreateTestImageFile("new-picture.jpg", "image/jpeg");

            var command = new UpdateUserProfilePictureCommand
            {
                UserId = user.Id,
                ProfilePictureFile = newImageFile,
                CurrentUserId = user.Id
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNullOrEmpty();
            result.Value.Should().Contain("new-picture");

            // Verify user profile picture was updated
            var updatedUser = await DbContext.Users.FindAsync(user.Id);
            updatedUser.Should().NotBeNull();
            updatedUser!.Profile?.ProfileImage.Should().NotBeNullOrEmpty();
            updatedUser.Profile?.ProfileImage.Should().Contain("new-picture");
            updatedUser.Profile?.ProfileImage.Should().NotContain("old-picture");
        }

        [Fact]
        public async Task Should_ReturnError_When_FileIsEmpty()
        {
            // Arrange
            var user = User.Create(
                "John",
                "Doe",
                "john.doe@example.com",
                "johndoe",
                Guid.NewGuid());

            await DbContext.Users.AddAsync(user);
            await SaveAndClearAsync();

            var emptyFile = CreateTestImageFile("empty.jpg", "image/jpeg", 0);

            var command = new UpdateUserProfilePictureCommand
            {
                UserId = user.Id,
                ProfilePictureFile = emptyFile,
                CurrentUserId = user.Id
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain("profile picture file is required.");
        }

        private static IFormFile CreateTestImageFile(string fileName, string contentType, int sizeInBytes = 1024)
        {
            var content = new byte[sizeInBytes];
            if (sizeInBytes > 0)
            {
                // Fill with random data
                new Random().NextBytes(content);
            }

            var stream = new MemoryStream(content);
            return new FormFile(stream, 0, content.Length, "ProfilePictureFile", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };
        }
    }
}