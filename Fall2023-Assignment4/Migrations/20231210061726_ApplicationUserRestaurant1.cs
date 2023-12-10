using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fall2023_Assignment4.Migrations
{
    /// <inheritdoc />
    public partial class ApplicationUserRestaurant1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FavoriteCount",
                table: "Restaurant",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ApplicationUserRestaurants",
                columns: table => new
                {
                    ApplicationUserId = table.Column<string>(type: "TEXT", nullable: false),
                    RestaurantId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserRestaurants", x => new { x.ApplicationUserId, x.RestaurantId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserRestaurants_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserRestaurants_Restaurant_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

          
            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserRestaurants_RestaurantId",
                table: "ApplicationUserRestaurants",
                column: "RestaurantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserRestaurants");

  
            migrationBuilder.DropColumn(
                name: "FavoriteCount",
                table: "Restaurant");
        }
    }
}
