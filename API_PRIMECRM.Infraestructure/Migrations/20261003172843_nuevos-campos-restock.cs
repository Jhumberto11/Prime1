using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_PRIMECRM.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class nuevoscamposrestock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LandedUnitCost",
                table: "RestockOrders",
                newName: "ActualTotalSV");

            migrationBuilder.AlterColumn<decimal>(
                name: "OtherCharges",
                table: "RestockOrders",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<decimal>(
                name: "ActualLandedUnitCost",
                table: "RestockOrders",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "EstimatedLandedUnitCost",
                table: "RestockOrders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "EstimatedTotalSV",
                table: "RestockOrders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualLandedUnitCost",
                table: "RestockOrders");

            migrationBuilder.DropColumn(
                name: "EstimatedLandedUnitCost",
                table: "RestockOrders");

            migrationBuilder.DropColumn(
                name: "EstimatedTotalSV",
                table: "RestockOrders");

            migrationBuilder.RenameColumn(
                name: "ActualTotalSV",
                table: "RestockOrders",
                newName: "LandedUnitCost");

            migrationBuilder.AlterColumn<decimal>(
                name: "OtherCharges",
                table: "RestockOrders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);
        }
    }
}
