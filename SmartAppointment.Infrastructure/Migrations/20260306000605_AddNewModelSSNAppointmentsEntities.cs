using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartAppointment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNewModelSSNAppointmentsEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SSN",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SSN",
                table: "Appointments");
        }
    }
}
