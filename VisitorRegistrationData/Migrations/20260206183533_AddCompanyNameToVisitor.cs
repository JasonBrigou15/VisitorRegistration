using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VisitorRegistrationData.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyNameToVisitor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "Visitors",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "Visitors");
        }
    }
}
