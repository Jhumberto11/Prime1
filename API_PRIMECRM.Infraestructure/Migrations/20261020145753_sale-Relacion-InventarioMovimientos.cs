using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_PRIMECRM.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class saleRelacionInventarioMovimientos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShippingCost",
                table: "Sales",
                newName: "TotalShippingCost");

            migrationBuilder.AlterColumn<int>(
                name: "SalesChannelId",
                table: "Sales",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "InventoryMovementId",
                table: "Sales",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Profit",
                table: "Sales",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ShippingCostRate",
                table: "Sales",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<int>(
                name: "RestockOrderId",
                table: "InventoryMovements",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "OrderStatus",
                table: "InventoryMovements",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SaleId",
                table: "InventoryMovements",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sales_InventoryMovementId",
                table: "Sales",
                column: "InventoryMovementId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_InventoryMovements_InventoryMovementId",
                table: "Sales",
                column: "InventoryMovementId",
                principalTable: "InventoryMovements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sales_InventoryMovements_InventoryMovementId",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_InventoryMovementId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "InventoryMovementId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "Profit",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "ShippingCostRate",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "OrderStatus",
                table: "InventoryMovements");

            migrationBuilder.DropColumn(
                name: "SaleId",
                table: "InventoryMovements");

            migrationBuilder.RenameColumn(
                name: "TotalShippingCost",
                table: "Sales",
                newName: "ShippingCost");

            migrationBuilder.AlterColumn<int>(
                name: "SalesChannelId",
                table: "Sales",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RestockOrderId",
                table: "InventoryMovements",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
