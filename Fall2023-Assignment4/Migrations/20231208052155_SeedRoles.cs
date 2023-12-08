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

                @$"INSERT [AspNetUsers] ([Id], [FirstName], [LastName], [UserName], [NormalizedUserName],

[Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp],

[PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount])

VALUES

(N'{User1Id}', N'Test 2', N'Lastname', N'jojo@gmail.com', N'JOJO@GMAIL.COM',

N'jojo@gmail.com', N'JOJO@GMAIL.COM', 0,

N'AQAAAAIAAYagAAAAENuYd7Y+OfVJuzEZVG9vFyhJwwvKs6yo0KkavLYx2ZIxWcq8GpRVHsayy7iGhqGW/w==',

N'5KZ5WMLG4WMOB5NBRPMRO2ZO6JYUJBDE', N'7c7a6af9-b951-41ee-a222-a90492fd5b02', NULL, 0, 0, NULL, 1, 0)");



       

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



        }

    }

}