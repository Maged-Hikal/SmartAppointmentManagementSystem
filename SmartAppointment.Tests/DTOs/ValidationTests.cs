using FluentAssertions;
using SmartAppointment.Application.DTOs;
using System.ComponentModel.DataAnnotations;

namespace SmartAppointment.Tests.DTOs
{
    public class CreateAppointmentDtoTests
    {
        [Fact]
        public void CreateAppointmentDto_With_Valid_Data_Should_Pass_Validation()
        {
            // Arrange
            var dto = new CreateAppointmentDto
            {
                Date = DateTime.UtcNow.AddDays(1),
                CustomerName = "John Doe",
                UserId = "user123",
                Email = "john@example.com",
                PhoneNumber = "+1234567890",
                SSN = "123456789012"
            };

            // Act
            var validationResults = ValidateModel(dto);

            // Assert
            validationResults.Should().BeEmpty();
        }

        [Fact]
        public void CreateAppointmentDto_With_Default_Date_Should_Pass_Validation()
        {
            var dto = new CreateAppointmentDto
            {
                Date = default(DateTime), //// dateTime cannot be null, so this is the minimum value
                CustomerName = "John Doe",
                UserId = "user123",
                Email = "john@example.com",
                PhoneNumber = "+1234567890",
                SSN = "123456789012"
            };

            var validationResults = ValidateModel(dto);

            validationResults.Should().BeEmpty();
        }

        [Fact]
        public void CreateAppointmentDto_With_Past_Date_Should_Pass_Validation()
        {
            var dto = new CreateAppointmentDto
            {
                Date = DateTime.UtcNow.AddDays(-1),
                CustomerName = "John Doe",
                UserId = "user123",
                Email = "john@example.com",
                PhoneNumber = "+1234567890",
                SSN = "123456789012"
            };

            var validationResults = ValidateModel(dto);

            validationResults.Should().BeEmpty();
        }

        [Fact]
        public void CreateAppointmentDto_With_Missing_CustomerName_Should_Fail_Validation()
        {
            var dto = new CreateAppointmentDto
            {
                Date = DateTime.UtcNow.AddDays(1),
                UserId = "user123",
                Email = "john@example.com",
                PhoneNumber = "+1234567890",
                SSN = "123456789012"
            };

            var validationResults = ValidateModel(dto);

            validationResults.Should().ContainSingle();
            validationResults[0].ErrorMessage.Should().Contain("required");
        }

        [Fact]
        public void CreateAppointmentDto_With_Too_Long_CustomerName_Should_Fail_Validation()
        {
            var dto = new CreateAppointmentDto
            {
                Date = DateTime.UtcNow.AddDays(1),
                CustomerName = new string('A', 101), // Exceeds 100 character limit
                UserId = "user123",
                Email = "john@example.com",
                PhoneNumber = "+1234567890",
                SSN = "123456789012"
            };
            var validationResults = ValidateModel(dto);

            validationResults.Should().ContainSingle();
            validationResults[0].ErrorMessage.Should().Contain("100");
        }

        [Fact]
        public void CreateAppointmentDto_With_Missing_PhoneNumber_Should_Fail_Validation()
        {
            var dto = new CreateAppointmentDto
            {
                Date = DateTime.UtcNow.AddDays(1),
                CustomerName = "John Doe",
                UserId = "user123",
                Email = "john@example.com",
                SSN = "123456789012"
            };

            var validationResults = ValidateModel(dto);

            validationResults.Should().ContainSingle();
            validationResults[0].ErrorMessage.Should().Contain("Phone number is required");
        }

        [Fact]
        public void CreateAppointmentDto_With_Invalid_PhoneNumber_Should_Fail_Validation()
        {
            var dto = new CreateAppointmentDto
            {
                Date = DateTime.UtcNow.AddDays(1),
                CustomerName = "John Doe",
                UserId = "user123",
                Email = "john@example.com",
                PhoneNumber = "invalid-phone",
                SSN = "123456789012"
            };

            var validationResults = ValidateModel(dto);

            validationResults.Should().ContainSingle();
            validationResults[0].ErrorMessage.Should().Contain("Invalid phone number format");
        }

        [Fact]
        public void CreateAppointmentDto_With_Missing_SSN_Should_Fail_Validation()
        {
            var dto = new CreateAppointmentDto
            {
                Date = DateTime.UtcNow.AddDays(1),
                CustomerName = "John Doe",
                UserId = "user123",
                Email = "john@example.com",
                PhoneNumber = "+1234567890"
            };

            var validationResults = ValidateModel(dto);


            validationResults.Should().ContainSingle();
            validationResults[0].ErrorMessage.Should().Contain("required");
        }

        [Theory]
        [InlineData("12345678901")] // 11 digits
        [InlineData("1234567890123")] // 13 digits
        [InlineData("1234567890a")] // Contains letter
        [InlineData("123-456-7890")] // Contains dashes
        public void CreateAppointmentDto_With_Invalid_SSN_Should_Fail_Validation(string ssn)
        {

            var dto = new CreateAppointmentDto
            {
                Date = DateTime.UtcNow.AddDays(1),
                CustomerName = "John Doe",
                UserId = "user123",
                Email = "john@example.com",
                PhoneNumber = "+1234567890",
                SSN = ssn
            };


            var validationResults = ValidateModel(dto);


            validationResults.Should().NotBeEmpty();
            validationResults.Should().Contain(v => v.ErrorMessage.Contains("SSN must be exactly 12 digits"));
        }

        [Fact]
        public void CreateAppointmentDto_With_Valid_SSN_Should_Pass_Validation()
        {

            var dto = new CreateAppointmentDto
            {
                Date = DateTime.UtcNow.AddDays(1),
                CustomerName = "John Doe",
                UserId = "user123",
                Email = "john@example.com",
                PhoneNumber = "+1234567890",
                SSN = "123456789012"
            };


            var validationResults = ValidateModel(dto);


            validationResults.Should().BeEmpty();
        }

        [Fact]
        public void CreateAppointmentDto_With_Null_Optional_Fields_Should_Pass_Validation()
        {

            var dto = new CreateAppointmentDto
            {
                Date = DateTime.UtcNow.AddDays(1),
                CustomerName = "John Doe",
                UserId = "user123",
                Email = null,
                PhoneNumber = "+1234567890",
                SSN = "123456789012"
            };


            var validationResults = ValidateModel(dto);


            validationResults.Should().BeEmpty();
        }

        [Fact]
        public void CreateAppointmentDto_With_Empty_Optional_Fields_Should_Pass_Validation()
        {

            var dto = new CreateAppointmentDto
            {
                Date = DateTime.UtcNow.AddDays(1),
                CustomerName = "John Doe",
                UserId = "user123",
                Email = "",
                PhoneNumber = "+1234567890",
                SSN = "123456789012"
            };


            var validationResults = ValidateModel(dto);


            validationResults.Should().BeEmpty();
        }

        private static IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            return validationResults;
        }
    }

    public class AppointmentDtoTests
    {
        [Fact]
        public void AppointmentDto_With_Valid_SSN_Should_Be_Valid()
        {

            var dto = new AppointmentDto
            {
                Id = Guid.NewGuid(),
                Date = DateTime.UtcNow.AddDays(1),
                CustomerName = "John Doe",
                Status = "Pending",
                UserId = "user123",
                Email = "john@example.com",
                PhoneNumber = "+1234567890",
                SSN = "123456789012"
            };


            var validationResults = ValidateModel(dto);


            validationResults.Should().BeEmpty();
        }

        [Theory]
        [InlineData("12345678901")] // 11 digits
        [InlineData("1234567890123")] // 13 digits
        [InlineData("1234567890a")] // Contains letter
        [InlineData("123-456-7890")] // Contains dashes
        public void AppointmentDto_With_Invalid_SSN_Should_Fail_Validation(string ssn)
        {

            var dto = new AppointmentDto
            {
                Id = Guid.NewGuid(),
                Date = DateTime.UtcNow.AddDays(1),
                CustomerName = "John Doe",
                Status = "Pending",
                UserId = "user123",
                Email = "john@example.com",
                PhoneNumber = "+1234567890",
                SSN = ssn
            };


            var validationResults = ValidateModel(dto);


            validationResults.Should().NotBeEmpty();
            validationResults.Should().Contain(v => v.ErrorMessage.Contains("SSN must be exactly 12 digits"));
        }

        [Fact]
        public void AppointmentDto_With_Null_SSN_Should_Be_Valid()
        {

            var dto = new AppointmentDto
            {
                Id = Guid.NewGuid(),
                Date = DateTime.UtcNow.AddDays(1),
                CustomerName = "John Doe",
                Status = "Pending",
                UserId = "user123",
                Email = "john@example.com",
                PhoneNumber = "+1234567890",
                SSN = null
            };


            var validationResults = ValidateModel(dto);


            validationResults.Should().BeEmpty();
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
