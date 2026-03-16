using FluentAssertions;
using System.ComponentModel.DataAnnotations;

namespace SmartAppointment.Tests.Models
{
    public class RegisterModelTests
    {
        [Fact]
        public void RegisterModel_With_Valid_Data_Should_Pass_Validation()
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

            // Act
            var validationResults = ValidateModel(model);

            // Assert
            validationResults.Should().BeEmpty();
        }

        [Fact]
        public void RegisterModel_With_Missing_UserName_Should_Fail_Validation()
        {
            // Arrange
            var model = new RegisterModel
            {
                UserName = "",
                Email = "john.doe@example.com",
                PhoneNumber = "+1234567890",
                SSN = "123456789012",
                Password = "Password123"
            };

            // Act
            var validationResults = ValidateModel(model);

            // Assert
            validationResults.Should().ContainSingle();
            validationResults[0].ErrorMessage.Should().Contain("Full Name is required");
        }

        [Fact]
        public void RegisterModel_With_Too_Long_UserName_Should_Fail_Validation()
        {
            // Arrange
            var model = new RegisterModel
            {
                UserName = new string('A', 51), // Exceeds 50 character limit
                Email = "john.doe@example.com",
                PhoneNumber = "+1234567890",
                SSN = "123456789012",
                Password = "Password123"
            };

            // Act
            var validationResults = ValidateModel(model);

            // Assert
            validationResults.Should().ContainSingle();
            validationResults[0].ErrorMessage.Should().Contain("Name is too long");
        }

        [Theory]
        [InlineData("John123")]
        [InlineData("John@Doe")]
        [InlineData("John#Doe")]
        public void RegisterModel_With_Invalid_UserName_Characters_Should_Fail_Validation(string userName)
        {
            // Arrange
            var model = new RegisterModel
            {
                UserName = userName,
                Email = "john.doe@example.com",
                PhoneNumber = "+1234567890",
                SSN = "123456789012",
                Password = "Password123"
            };

            // Act
            var validationResults = ValidateModel(model);

            // Assert
            validationResults.Should().ContainSingle();
            validationResults[0].ErrorMessage.Should().Contain("Only letters and name symbols");
        }

        [Fact]
        public void RegisterModel_With_Valid_UserName_Characters_Should_Pass_Validation()
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

            // Act
            var validationResults = ValidateModel(model);

            // Assert
            validationResults.Should().BeEmpty();
        }

        [Fact]
        public void RegisterModel_With_Invalid_Email_Should_Fail_Validation()
        {
            // Arrange
            var model = new RegisterModel
            {
                UserName = "John Doe",
                Email = "invalid-email",
                PhoneNumber = "+1234567890",
                SSN = "123456789012",
                Password = "Password123"
            };

            // Act
            var validationResults = ValidateModel(model);

            // Assert
            validationResults.Should().ContainSingle();
            validationResults[0].ErrorMessage.Should().Contain("valid email");
        }

        [Fact]
        public void RegisterModel_With_Null_Email_Should_Pass_Validation()
        {
            // Arrange
            var model = new RegisterModel
            {
                UserName = "John Doe",
                Email = null,
                PhoneNumber = "+1234567890",
                SSN = "123456789012",
                Password = "Password123"
            };

            // Act
            var validationResults = ValidateModel(model);

            // Assert
            validationResults.Should().BeEmpty();
        }

        [Fact]
        public void RegisterModel_With_Invalid_PhoneNumber_Should_Fail_Validation()
        {
            // Arrange
            var model = new RegisterModel
            {
                UserName = "John Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "invalid-phone",
                SSN = "123456789012",
                Password = "Password123"
            };

            // Act
            var validationResults = ValidateModel(model);

            // Assert
            validationResults.Should().ContainSingle();
            validationResults[0].ErrorMessage.Should().Contain("Invalid phone number");
        }

        [Fact]
        public void RegisterModel_With_Null_PhoneNumber_Should_Pass_Validation()
        {
            // Arrange
            var model = new RegisterModel
            {
                UserName = "John Doe",
                Email = "john.doe@example.com",
                PhoneNumber = null,
                SSN = "123456789012",
                Password = "Password123"
            };

            // Act
            var validationResults = ValidateModel(model);

            // Assert
            validationResults.Should().BeEmpty();
        }

        [Fact]
        public void RegisterModel_With_Missing_SSN_Should_Fail_Validation()
        {
            // Arrange
            var model = new RegisterModel
            {
                UserName = "John Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "+1234567890",
                SSN = "",
                Password = "Password123"
            };

            // Act
            var validationResults = ValidateModel(model);

            // Assert
            validationResults.Should().ContainSingle();
            validationResults[0].ErrorMessage.Should().Contain("SSN is required");
        }

        [Theory]
        [InlineData("12345678901")] // 11 digits
        [InlineData("1234567890123")] // 13 digits
        [InlineData("1234567890a")] // Contains letter
        [InlineData("123-456-789012")] // Contains dashes
        [InlineData("123 456 789012")] // Contains spaces
        public void RegisterModel_With_Invalid_SSN_Should_Fail_Validation(string ssn)
        {
            // Arrange
            var model = new RegisterModel
            {
                UserName = "John Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "+1234567890",
                SSN = ssn,
                Password = "Password123"
            };

            // Act
            var validationResults = ValidateModel(model);

            // Assert
            validationResults.Should().NotBeEmpty();
            validationResults.Should().Contain(v => v.ErrorMessage.Contains("SSN must be exactly 12 digits"));
        }

        [Fact]
        public void RegisterModel_With_Valid_SSN_Should_Pass_Validation()
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

            // Act
            var validationResults = ValidateModel(model);

            // Assert
            validationResults.Should().BeEmpty();
        }

        [Fact]
        public void RegisterModel_With_Short_Password_Should_Fail_Validation()
        {
            // Arrange
            var model = new RegisterModel
            {
                UserName = "John Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "+1234567890",
                SSN = "123456789012",
                Password = "12345" // Only 5 characters
            };

            // Act
            var validationResults = ValidateModel(model);

            // Assert
            validationResults.Should().ContainSingle();
            validationResults[0].ErrorMessage.Should().Contain("at least 6");
        }

        [Fact]
        public void RegisterModel_With_Too_Long_Password_Should_Fail_Validation()
        {
            // Arrange
            var model = new RegisterModel
            {
                UserName = "John Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "+1234567890",
                SSN = "123456789012",
                Password = new string('A', 101) // Exceeds 100 character limit
            };

            // Act
            var validationResults = ValidateModel(model);

            // Assert
            validationResults.Should().ContainSingle();
            validationResults[0].ErrorMessage.Should().Contain("at most 100");
        }

        [Fact]
        public void RegisterModel_With_Valid_Password_Length_Should_Pass_Validation()
        {
            // Arrange
            var model = new RegisterModel
            {
                UserName = "John Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "+1234567890",
                SSN = "123456789012",
                Password = "ValidPassword123" // 16 characters
            };

            // Act
            var validationResults = ValidateModel(model);

            // Assert
            validationResults.Should().BeEmpty();
        }

        [Fact]
        public void RegisterModel_With_Multiple_Validation_Errors_Should_Return_All_Errors()
        {
            // Arrange
            var model = new RegisterModel
            {
                UserName = "", // Missing
                Email = "invalid-email", // Invalid format
                PhoneNumber = "invalid-phone", // Invalid format
                SSN = "123", // Too short
                Password = "123" // Too short
            };

            // Act
            var validationResults = ValidateModel(model);

            // Assert
            validationResults.Should().HaveCountGreaterOrEqualTo(4);
            validationResults.Should().Contain(v => v.ErrorMessage.Contains("Full Name is required"));
            validationResults.Should().Contain(v => v.ErrorMessage.Contains("valid email"));
            validationResults.Should().Contain(v => v.ErrorMessage.Contains("Invalid phone number"));
            validationResults.Should().Contain(v => v.ErrorMessage.Contains("SSN must be exactly 12 digits"));
        }

        private static IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            return validationResults;
        }
    }
}
