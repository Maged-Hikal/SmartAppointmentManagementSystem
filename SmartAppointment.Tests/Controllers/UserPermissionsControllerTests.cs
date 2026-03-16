using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SmartAppointment.API.Controllers;
using SmartAppointment.Infrastructure.Identity;

namespace SmartAppointment.Tests.Controllers
{
    public class UserPermissionsControllerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly UserPermissionsController _controller;

        public UserPermissionsControllerTests()
        {
            _mockUserManager = CreateUserManagerMock();
            _controller = new UserPermissionsController(null, _mockUserManager.Object);
        }

        [Fact]
        public async Task Register_With_Valid_Model_Should_Return_Ok()
        {
            // Arrange
            var model = new RegisterModel
            {
                UserName = "John Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "+1234567890",
                SSN = "123456789012",
                Password = "Password123"
            };

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                SSN = model.SSN
            };

            _mockUserManager.Setup(x => x.Users.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<ApplicationUser, bool>>>(), default))
                          .ReturnsAsync(false);

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), model.Password))
                          .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.Register(model);

            // Assert
            result.Should().BeOfType<OkResult>();
            _mockUserManager.Verify(x => x.Users.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<ApplicationUser, bool>>>(), default), Times.Once);
            _mockUserManager.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>(), model.Password), Times.Once);
        }

        [Fact]
        public async Task Register_With_Duplicate_SSN_Should_Return_BadRequest()
        {
            // Arrange
            var model = new RegisterModel
            {
                UserName = "John Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "+1234567890",
                SSN = "123456789012",
                Password = "Password123"
            };

            _mockUserManager.Setup(x => x.Users.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<ApplicationUser, bool>>>(), default))
                          .ReturnsAsync(true);

            // Act
            var result = await _controller.Register(model);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            var errorMessages = badRequestResult.Value.Should().BeOfType<string[]>().Subject;
            errorMessages.Should().ContainSingle();
            errorMessages[0].Should().Be("SSN already registered, please choose another SSN");
            _mockUserManager.Verify(x => x.Users.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<ApplicationUser, bool>>>(), default), Times.Once);
            _mockUserManager.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Register_With_Identity_Failure_Should_Return_BadRequest_With_Errors()
        {
            // Arrange
            var model = new RegisterModel
            {
                UserName = "John Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "+1234567890",
                SSN = "123456789012",
                Password = "Password123"
            };

            var identityErrors = new List<IdentityError>
            {
                new IdentityError { Description = "Password too weak" },
                new IdentityError { Description = "Email already exists" }
            };

            _mockUserManager.Setup(x => x.Users.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<ApplicationUser, bool>>>(), default))
                          .ReturnsAsync(false);

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), model.Password))
                          .ReturnsAsync(IdentityResult.Failed(identityErrors.ToArray()));

            // Act
            var result = await _controller.Register(model);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            var errorMessages = badRequestResult.Value.Should().BeOfType<List<string>>().Subject;
            errorMessages.Should().HaveCount(2);
            errorMessages.Should().Contain("Password too weak");
            errorMessages.Should().Contain("Email already exists");
            _mockUserManager.Verify(x => x.Users.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<ApplicationUser, bool>>>(), default), Times.Once);
            _mockUserManager.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>(), model.Password), Times.Once);
        }

        [Fact]
        public async Task Register_With_Minimal_Valid_Data_Should_Return_Ok()
        {
            // Arrange
            var model = new RegisterModel
            {
                UserName = "Jane Smith",
                Email = null, // Optional
                PhoneNumber = null, // Optional
                SSN = "987654321098",
                Password = "SecurePass123"
            };

            _mockUserManager.Setup(x => x.Users.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<ApplicationUser, bool>>>(), default))
                          .ReturnsAsync(false);

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), model.Password))
                          .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.Register(model);

            // Assert
            result.Should().BeOfType<OkResult>();
            _mockUserManager.Verify(x => x.Users.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<ApplicationUser, bool>>>(), default), Times.Once);
            _mockUserManager.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>(), model.Password), Times.Once);
        }

        [Fact]
        public async Task Register_With_Maximum_Length_Values_Should_Return_Ok()
        {
            // Arrange
            var model = new RegisterModel
            {
                UserName = new string('A', 50), // Maximum allowed length
                Email = "very.long.email.address@example.com",
                PhoneNumber = "+12345678901234567890",
                SSN = "123456789012",
                Password = new string('A', 100) // Maximum allowed length
            };

            _mockUserManager.Setup(x => x.Users.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<ApplicationUser, bool>>>(), default))
                          .ReturnsAsync(false);

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), model.Password))
                          .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.Register(model);

            // Assert
            result.Should().BeOfType<OkResult>();
            _mockUserManager.Verify(x => x.Users.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<ApplicationUser, bool>>>(), default), Times.Once);
            _mockUserManager.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>(), model.Password), Times.Once);
        }

        [Fact]
        public async Task Register_With_Special_Characters_In_UserName_Should_Return_Ok()
        {
            // Arrange
            var model = new RegisterModel
            {
                UserName = "John Doe-Smith O'Connor",
                Email = "john.doe@example.com",
                PhoneNumber = "+1234567890",
                SSN = "123456789012",
                Password = "Password123"
            };

            _mockUserManager.Setup(x => x.Users.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<ApplicationUser, bool>>>(), default))
                          .ReturnsAsync(false);

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), model.Password))
                          .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.Register(model);

            // Assert
            result.Should().BeOfType<OkResult>();
            _mockUserManager.Verify(x => x.Users.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<ApplicationUser, bool>>>(), default), Times.Once);
            _mockUserManager.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>(), model.Password), Times.Once);
        }

        private Mock<UserManager<ApplicationUser>> CreateUserManagerMock()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var passwordHasher = new Mock<IPasswordHasher<ApplicationUser>>();
            var userValidators = new List<IUserValidator<ApplicationUser>>();
            var passwordValidators = new List<IPasswordValidator<ApplicationUser>>();
            var keyNormalizer = new Mock<ILookupNormalizer>();
            var errors = new Mock<IdentityErrorDescriber>();
            var services = new Mock<IServiceProvider>();
            var logger = new Mock<ILogger<UserManager<ApplicationUser>>>();

            return new Mock<UserManager<ApplicationUser>>(
                store.Object,
                null,
                passwordHasher.Object,
                userValidators,
                passwordValidators,
                keyNormalizer.Object,
                errors.Object,
                services.Object,
                logger.Object);
        }
    }
}
