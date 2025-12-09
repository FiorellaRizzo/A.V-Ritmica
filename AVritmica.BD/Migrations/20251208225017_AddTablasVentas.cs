using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AVritmica.BD.Migrations
{
    /// <inheritdoc />
    public partial class AddTablasVentas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            // Comento los cambios a CarritoProductos para no afectar tablas existentes
          

            // migrationBuilder.DropIndex(
            //     name: "IX_CarritoProductos_CarritoProductoVariante",
            //     table: "CarritoProductos");
            //
            // migrationBuilder.AlterColumn<string>(
            //     name: "Tamaño",
            //     table: "CarritoProductos",
            //     type: "nvarchar(50)",
            //     maxLength: 50,
            //     nullable: true,
            //     oldClrType: typeof(string),
            //     oldType: "nvarchar(100)",
            //     oldMaxLength: 100,
            //     oldNullable: true);
            //
            // migrationBuilder.AlterColumn<string>(
            //     name: "Color",
            //     table: "CarritoProductos",
            //     type: "nvarchar(50)",
            //     maxLength: 50,
            //     nullable: true,
            //     oldClrType: typeof(string),
            //     oldType: "nvarchar(100)",
            //     oldMaxLength: 100,
            //     oldNullable: true);

            
            // CREACIÓN DE TABLAS NUEVAS (VENTAS)
            

            migrationBuilder.CreateTable(
                name: "Ventas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroVenta = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Estado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Pendiente"),
                    NombreCliente = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TipoEnvio = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "RetiroLocal"),
                    DireccionEnvio = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CiudadEnvio = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CodigoPostalEnvio = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    ProvinciaEnvio = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CostoEnvio = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MetodoPago = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Pendiente"),
                    NumeroComprobante = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FechaPago = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BancoOrigen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TitularCuenta = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notas = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NumeroTracking = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UrlTracking = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ventas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VentaDetalles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VentaId = table.Column<int>(type: "int", nullable: false),
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Tamanio = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VentaDetalles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VentaDetalles_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VentaDetalles_Ventas_VentaId",
                        column: x => x.VentaId,
                        principalTable: "Ventas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade); // cambio Restrict A Cascade para poder hacer el cambio en la BD
                });

            migrationBuilder.CreateIndex(
                name: "IX_VentaDetalles_ProductoId",
                table: "VentaDetalles",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_VentaDetalles_VentaId",
                table: "VentaDetalles",
                column: "VentaId");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_NumeroVenta",
                table: "Ventas",
                column: "NumeroVenta",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VentaDetalles");

            migrationBuilder.DropTable(
                name: "Ventas");

            
            // comento la restauracion de CarritoProductos
            // porque no quiero modificar esas columnas
            

            // migrationBuilder.AlterColumn<string>(
            //     name: "Tamaño",
            //     table: "CarritoProductos",
            //     type: "nvarchar(100)",
            //     maxLength: 100,
            //     nullable: true,
            //     oldClrType: typeof(string),
            //     oldType: "nvarchar(50)",
            //     oldMaxLength: 50,
            //     oldNullable: true);
            //
            // migrationBuilder.AlterColumn<string>(
            //     name: "Color",
            //     table: "CarritoProductos",
            //     type: "nvarchar(100)",
            //     maxLength: 100,
            //     nullable: true,
            //     oldClrType: typeof(string),
            //     oldType: "nvarchar(50)",
            //     oldMaxLength: 50,
            //     oldNullable: true);
            //
            // migrationBuilder.CreateIndex(
            //     name: "IX_CarritoProductos_CarritoProductoVariante",
            //     table: "CarritoProductos",
            //     columns: new[] { "CarritoId", "ProductoId", "Color", "Tamaño" },
            //     unique: true,
            //     filter: "[Color] IS NOT NULL AND [Tamaño] IS NOT NULL");
        }
    }
}