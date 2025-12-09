using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AVritmica.BD.Migrations
{
    /// <inheritdoc />
    public partial class AddVariantesCarritoProducto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CarritoProductos_CarritoProducto",
                table: "CarritoProductos");

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "CarritoProductos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tamaño",
                table: "CarritoProductos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                  name: "IX_CarritoProductos_CarritoProductoVariante",
                  table: "CarritoProductos",
                  columns: new[] { "CarritoId", "ProductoId", "Color", "Tamaño" },
                  unique: true);  //  SIN el filter
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CarritoProductos_CarritoProductoVariante",
                table: "CarritoProductos");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "CarritoProductos");

            migrationBuilder.DropColumn(
                name: "Tamaño",
                table: "CarritoProductos");

            migrationBuilder.CreateIndex(
                name: "IX_CarritoProductos_CarritoProducto",
                table: "CarritoProductos",
                columns: new[] { "CarritoId", "ProductoId" },
                unique: true);
        }
    }
}
