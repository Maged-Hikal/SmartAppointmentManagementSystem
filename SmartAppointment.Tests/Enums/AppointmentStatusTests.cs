using FluentAssertions;
using SmartAppointment.Domain.Enums;

namespace SmartAppointment.Tests.Enums
{
    public class AppointmentStatusTests
    {
        [Fact]
        public void AppointmentStatus_Should_Have_Correct_Values()
        {
            // Assert
            ((int)AppointmentStatus.Pending).Should().Be(0);
            ((int)AppointmentStatus.Approved).Should().Be(1);
            ((int)AppointmentStatus.Rejected).Should().Be(2);
            ((int)AppointmentStatus.Cancelled).Should().Be(3);
            ((int)AppointmentStatus.Rescheduled).Should().Be(4);
        }

        [Fact]
        public void AppointmentStatus_Should_Have_Five_Values()
        {
            // Arrange & Act
            var values = Enum.GetValues(typeof(AppointmentStatus));

            // Assert
            values.Length.Should().Be(5);
        }

        [Theory]
        [InlineData(AppointmentStatus.Pending, "Pending")]
        [InlineData(AppointmentStatus.Approved, "Approved")]
        [InlineData(AppointmentStatus.Rejected, "Rejected")]
        [InlineData(AppointmentStatus.Cancelled, "Cancelled")]
        [InlineData(AppointmentStatus.Rescheduled, "Rescheduled")]
        public void AppointmentStatus_Should_Have_Correct_String_Representations(AppointmentStatus status, string expectedString)
        {
            // Act
            var statusString = status.ToString();

            // Assert
            statusString.Should().Be(expectedString);
        }

        [Fact]
        public void AppointmentStatus_Default_Should_Be_Pending()
        {
            // Arrange & Act
            var defaultStatus = default(AppointmentStatus);

            // Assert
            defaultStatus.Should().Be(AppointmentStatus.Pending);
        }

        [Theory]
        [InlineData(0, AppointmentStatus.Pending)]
        [InlineData(1, AppointmentStatus.Approved)]
        [InlineData(2, AppointmentStatus.Rejected)]
        [InlineData(3, AppointmentStatus.Cancelled)]
        [InlineData(4, AppointmentStatus.Rescheduled)]
        public void AppointmentStatus_Should_Parse_From_Integer_Correctly(int intValue, AppointmentStatus expectedStatus)
        {
            // Arrange & Act
            var parsedStatus = (AppointmentStatus)intValue;

            // Assert
            parsedStatus.Should().Be(expectedStatus);
        }

        [Theory]
        [InlineData("Pending", AppointmentStatus.Pending)]
        [InlineData("Approved", AppointmentStatus.Approved)]
        [InlineData("Rejected", AppointmentStatus.Rejected)]
        [InlineData("Cancelled", AppointmentStatus.Cancelled)]
        [InlineData("Rescheduled", AppointmentStatus.Rescheduled)]
        public void AppointmentStatus_Should_Parse_From_String_Correctly(string stringValue, AppointmentStatus expectedStatus)
        {
            // Arrange & Act
            var parsedStatus = Enum.Parse<AppointmentStatus>(stringValue);

            // Assert
            parsedStatus.Should().Be(expectedStatus);
        }

        [Fact]
        public void AppointmentStatus_Should_Support_TryParse()
        {
            // Arrange
            const string validStatusString = "Approved";
            const string invalidStatusString = "InvalidStatus";

            // Act
            var validParseResult = Enum.TryParse(validStatusString, out AppointmentStatus validStatusValue);
            var invalidParseResult = Enum.TryParse(invalidStatusString, out AppointmentStatus invalidStatusValue);

            // Assert
            validParseResult.Should().BeTrue();
            validStatusValue.Should().Be(AppointmentStatus.Approved);

            invalidParseResult.Should().BeFalse();
            invalidStatusValue.Should().Be(default(AppointmentStatus));
        }

        [Fact]
        public void AppointmentStatus_Should_Support_Comparison_Operators()
        {
            // Arrange
            var pending = AppointmentStatus.Pending;
            var approved = AppointmentStatus.Approved;
            var rejected = AppointmentStatus.Rejected;
            var cancelled = AppointmentStatus.Cancelled;
            var rescheduled = AppointmentStatus.Rescheduled;

            // Act & Assert
            (pending < approved).Should().BeTrue();
            (approved > pending).Should().BeTrue();
            (pending <= AppointmentStatus.Pending).Should().BeTrue();
            (approved >= AppointmentStatus.Approved).Should().BeTrue();
            (rejected == rejected).Should().BeTrue();
            (pending != approved).Should().BeTrue();
            (cancelled < rescheduled).Should().BeTrue();
        }
    }
}
