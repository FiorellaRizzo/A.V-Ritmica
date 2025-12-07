using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AVritmica.BD.Migrations
{
    /// <inheritdoc />
    public partial class AgregarStockPorColor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockMovimientos_Productos_ProductoId1",
                table: "StockMovimientos");

            migrationBuilder.DropIndex(
                name: "IX_StockMovimientos_ProductoId1",
                table: "StockMovimientos");

            migrationBuilder.DropColumn(
                name: "ProductoId1",
                table: "StockMovimientos");

            migrationBuilder.AddColumn<string>(
                name: "StockPorColor",
                table: "Productos",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StockPorColor",
                table: "Productos");

            migrationBuilder.AddColumn<int>(
                name: "ProductoId1",
                table: "StockMovimientos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockMovimientos_ProductoId1",
                table: "StockMovimientos",
                column: "ProductoId1");

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovimientos_Productos_ProductoId1",
                table: "StockMovimientos",
                column: "ProductoId1",
                principalTable: "Productos",
                principalColumn: "Id");
        }
    }
}
