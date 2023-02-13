using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2_PeliculasAPI.Migrations
{
    public partial class AdminData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
    
            BEGIN TRANSACTION;
            GO

            IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
                SET IDENTITY_INSERT [AspNetRoles] ON;
            INSERT INTO [AspNetRoles] ([Id], [ConcurrencyStamp], [Name], [NormalizedName])
            VALUES (N'9aae0b6d-d50c-4d0a-9b90-2a6873e3845d', N'9c1ab91a-c121-4f2b-bce0-03bf1593c600', N'Admin', N'Admin');
            IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
                SET IDENTITY_INSERT [AspNetRoles] OFF;
            GO

            IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AccessFailedCount', N'ConcurrencyStamp', N'Email', N'EmailConfirmed', N'LockoutEnabled', N'LockoutEnd', N'NormalizedEmail', N'NormalizedUserName', N'PasswordHash', N'PhoneNumber', N'PhoneNumberConfirmed', N'SecurityStamp', N'TwoFactorEnabled', N'UserName') AND [object_id] = OBJECT_ID(N'[AspNetUsers]'))
                SET IDENTITY_INSERT [AspNetUsers] ON;
            INSERT INTO [AspNetUsers] ([Id], [AccessFailedCount], [ConcurrencyStamp], [Email], [EmailConfirmed], [LockoutEnabled], [LockoutEnd], [NormalizedEmail], [NormalizedUserName], [PasswordHash], [PhoneNumber], [PhoneNumberConfirmed], [SecurityStamp], [TwoFactorEnabled], [UserName])
            VALUES (N'5673b8cf-12de-44f6-92ad-fae4a77932ad', 0, N'3e23fcfe-9eac-48c8-adfa-c6a0460cf841', N'felipe@hotmail.com', CAST(0 AS bit), CAST(0 AS bit), NULL, N'felipe@hotmail.com', N'felipe@hotmail.com', N'AQAAAAEAACcQAAAAEFkeOhM5Uoz3E9HBhTBgJZcB593Qps80/1j6eUCBIZRxkcBxADOsgGgLGSIevQA9ew==', NULL, CAST(0 AS bit), N'3e505c5d-52e1-4649-9fcd-c083ef84a833', CAST(0 AS bit), N'felipe@hotmail.com');
            IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AccessFailedCount', N'ConcurrencyStamp', N'Email', N'EmailConfirmed', N'LockoutEnabled', N'LockoutEnd', N'NormalizedEmail', N'NormalizedUserName', N'PasswordHash', N'PhoneNumber', N'PhoneNumberConfirmed', N'SecurityStamp', N'TwoFactorEnabled', N'UserName') AND [object_id] = OBJECT_ID(N'[AspNetUsers]'))
                SET IDENTITY_INSERT [AspNetUsers] OFF;
            GO

            IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ClaimType', N'ClaimValue', N'UserId') AND [object_id] = OBJECT_ID(N'[AspNetUserClaims]'))
                SET IDENTITY_INSERT [AspNetUserClaims] ON;
            INSERT INTO [AspNetUserClaims] ([Id], [ClaimType], [ClaimValue], [UserId])
            VALUES (1, N'http://schemas.microsoft.com/ws/2008/06/identity/claims/role', N'Admin', N'5673b8cf-12de-44f6-92ad-fae4a77932ad');
            IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ClaimType', N'ClaimValue', N'UserId') AND [object_id] = OBJECT_ID(N'[AspNetUserClaims]'))
                SET IDENTITY_INSERT [AspNetUserClaims] OFF;

            GO

            COMMIT;

            GO

            ");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DeleteData(
              table: "AspNetRoles",
              keyColumn: "Id",
              keyValue: "9aae0b6d-d50c-4d0a-9b90-2a6873e3845d");

            migrationBuilder.DeleteData(
                table: "AspNetUserClaims",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "5673b8cf-12de-44f6-92ad-fae4a77932ad");

        }
    }
}
