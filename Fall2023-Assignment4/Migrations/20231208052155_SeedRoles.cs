using Microsoft.EntityFrameworkCore.Migrations;

namespace Fall2023_Assignment4.Migrations
{
    public partial class SeedRoles : Migration
    {
        private string ManagerRoleId = Guid.NewGuid().ToString();

        private string UserRoleId = Guid.NewGuid().ToString();

        private string AdminRoleId = Guid.NewGuid().ToString();



        private string User1Id = Guid.NewGuid().ToString();

        private string User2Id = Guid.NewGuid().ToString();



        protected override void Up(MigrationBuilder migrationBuilder)

        {

            SeedRolesSQL(migrationBuilder);



            SeedUser(migrationBuilder);



            SeedUserRoles(migrationBuilder);

            migrationBuilder.CreateTable(
                name: "Review",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    RestaurantId = table.Column<string>(type: "TEXT", nullable: false),
                    Url = table.Column<string>(type: "TEXT", nullable: true),
                    Text = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: true),
                    UserName = table.Column<string>(type: "TEXT", nullable: true),
                    TimeCreated = table.Column<string>(type: "TEXT", nullable: true),
                    Rating = table.Column<double>(type: "REAL", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Review", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Review_Restaurant_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Review_RestaurantId",
                table: "Review",
                column: "RestaurantId");

        }



        private void SeedRolesSQL(MigrationBuilder migrationBuilder)

        {

            migrationBuilder.Sql(@$"INSERT INTO [AspNetRoles] ([Id],[Name],[NormalizedName],[ConcurrencyStamp])

            VALUES ('{AdminRoleId}', 'Administrator', 'ADMINISTRATOR', null);");

            migrationBuilder.Sql(@$"INSERT INTO [AspNetRoles] ([Id],[Name],[NormalizedName],[ConcurrencyStamp])

            VALUES ('{ManagerRoleId}', 'Manager', 'MANAGER', null);");

            migrationBuilder.Sql(@$"INSERT INTO [AspNetRoles] ([Id],[Name],[NormalizedName],[ConcurrencyStamp])

            VALUES ('{UserRoleId}', 'User', 'USER', null);");

        }



        private void SeedUser(MigrationBuilder migrationBuilder)

        {

            migrationBuilder.Sql(
    $@"INSERT INTO AspNetUsers (Id, FirstName, LastName, UserName, NormalizedUserName,
        Email, NormalizedEmail, EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp,
        PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnd, LockoutEnabled, AccessFailedCount)
    VALUES
        ('{User1Id}', 'J', 'K', 'jojo@gmail.com', 'JOJO@GMAIL.COM',
        'jojo@gmail.com', 'JOJO@GMAIL.COM', 0,
        'AQAAAAIAAYagAAAAENuYd7Y+OfVJuzEZVG9vFyhJwwvKs6yo0KkavLYx2ZIxWcq8GpRVHsayy7iGhqGW/w==',
        '5KZ5WMLG4WMOB5NBRPMRO2ZO6JYUJBDE', '7c7a6af9-b951-41ee-a222-a90492fd5b02', NULL, 0, 0, NULL, 1, 0)");

            migrationBuilder.Sql(
    $@"INSERT INTO AspNetUsers (Id, FirstName, LastName, UserName, NormalizedUserName,
        Email, NormalizedEmail, EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp,
        PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnd, LockoutEnabled, AccessFailedCount)
    VALUES
        ('{User2Id}', 'N', 'U', 'normalUser@gmail.com', 'NORMALUSER@GMAIL.COM',
        'normalUser@gmail.com', 'NORMALUSER@GMAIL.COM', 0,
        'AQAAAAIAAYagAAAAEPv/ovLjuwV8/AukwA0HZAvK1LQOVz8bgdx8sR2Apmhg33i5Brq6tN5GJDmXYnw4xw==',
        'OKF7J2H7OTVB3CV5KRNZKZIOXZAQ4MA2', 'fc556592-61c7-42f5-92cd-b2d0926cc3b6', NULL, 0, 0, NULL, 1, 0)");


        }



        private void SeedUserRoles(MigrationBuilder migrationBuilder)

        {

            migrationBuilder.Sql(@$"

        INSERT INTO [AspNetUserRoles]

           ([UserId]

           ,[RoleId])

        VALUES

           ('{User1Id}', '{ManagerRoleId}');

        INSERT INTO [AspNetUserRoles]

           ([UserId]

           ,[RoleId])

        VALUES

           ('{User1Id}', '{UserRoleId}');");



            migrationBuilder.Sql(@$"

        INSERT INTO [AspNetUserRoles]

           ([UserId]

           ,[RoleId])

        VALUES

           ('{User2Id}', '{AdminRoleId}');

        INSERT INTO [AspNetUserRoles]

           ([UserId]

           ,[RoleId])

        VALUES

           ('{User2Id}', '{ManagerRoleId}');");



        }



        protected override void Down(MigrationBuilder migrationBuilder)

        {
            migrationBuilder.DropTable(
                name: "Review");
        }

    }

}