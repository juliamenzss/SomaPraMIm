using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SomaPraMim.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MarketName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MarketName",
                table: "ShoppingLists",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "ShoppingItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Unit",
                table: "ShoppingItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MarketName",
                table: "ShoppingLists");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "ShoppingItems");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "ShoppingItems");
        }
    }
}
