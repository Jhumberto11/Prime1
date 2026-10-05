using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_PRIMECRM.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class _2decimalesmasCourier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "CashHandlingValue",
                table: "CourierCompanies",
                type: "decimal(18,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "CashHandlingValue",
                table: "CourierCompanies",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)");


        }
    }
}
