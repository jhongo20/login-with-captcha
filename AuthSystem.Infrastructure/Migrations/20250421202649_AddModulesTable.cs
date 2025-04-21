using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AuthSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddModulesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("7220c88a-1ee4-45fe-91b1-34c73a2c57ad"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("7b396271-d5eb-428d-827d-f840488a1301"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("8ba1f416-11ef-4ad5-9db6-82c068f6b2e0"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("cfd610f0-e324-4a69-b726-e5c84560e4e0"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("eeac088f-34f9-499a-b5f7-114b72e22eb5"));

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("2e94db85-9918-4b3f-a09b-cb886f8c2867"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("39dbc70f-5115-4999-b26b-4476115fe09e"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("72138c65-e260-419d-b83d-9bf2476a32b8"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("86155368-e34b-484a-b717-72ef484b6087"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("c59219dd-ecab-40ce-8565-36377c641575"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("0b565c8b-393c-4365-9817-c59b2c8d869d"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("376587dc-283d-492c-a80b-49cf8c6cb11a"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("f63828d9-87a8-434f-b460-291a63b2732f"));

            migrationBuilder.CreateTable(
                name: "Modules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Route = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Modules_Modules_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Modules",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Description", "IsActive", "LastModifiedAt", "LastModifiedBy", "Name" },
                values: new object[,]
                {
                    { new Guid("034726f0-f521-40ae-8208-a15628794e7d"), new DateTime(2025, 4, 21, 20, 26, 49, 130, DateTimeKind.Utc).AddTicks(4052), "System", "Eliminar usuarios", true, new DateTime(2025, 4, 21, 20, 26, 49, 130, DateTimeKind.Utc).AddTicks(4052), "System", "users.delete" },
                    { new Guid("607d2005-63f7-4cfc-b9ab-b16077bf041c"), new DateTime(2025, 4, 21, 20, 26, 49, 130, DateTimeKind.Utc).AddTicks(4045), "System", "Ver usuarios", true, new DateTime(2025, 4, 21, 20, 26, 49, 130, DateTimeKind.Utc).AddTicks(4046), "System", "users.view" },
                    { new Guid("a5c08ec3-eeef-4910-8b15-3f9529c39a70"), new DateTime(2025, 4, 21, 20, 26, 49, 130, DateTimeKind.Utc).AddTicks(4048), "System", "Crear usuarios", true, new DateTime(2025, 4, 21, 20, 26, 49, 130, DateTimeKind.Utc).AddTicks(4049), "System", "users.create" },
                    { new Guid("ffdaccc5-f1ed-446e-8166-878ee297d743"), new DateTime(2025, 4, 21, 20, 26, 49, 130, DateTimeKind.Utc).AddTicks(4050), "System", "Editar usuarios", true, new DateTime(2025, 4, 21, 20, 26, 49, 130, DateTimeKind.Utc).AddTicks(4050), "System", "users.edit" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Description", "IsActive", "LastModifiedAt", "LastModifiedBy", "Name" },
                values: new object[,]
                {
                    { new Guid("d7e350e8-5fb7-4517-b8da-6f602d66a3a9"), new DateTime(2025, 4, 21, 20, 26, 49, 130, DateTimeKind.Utc).AddTicks(3877), "System", "Administrador del sistema", true, new DateTime(2025, 4, 21, 20, 26, 49, 130, DateTimeKind.Utc).AddTicks(3879), "System", "Admin" },
                    { new Guid("e1a8268c-d086-4098-aaf4-e326b3cd1dee"), new DateTime(2025, 4, 21, 20, 26, 49, 130, DateTimeKind.Utc).AddTicks(3886), "System", "Usuario estándar", true, new DateTime(2025, 4, 21, 20, 26, 49, 130, DateTimeKind.Utc).AddTicks(3886), "System", "User" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedAt", "CreatedBy", "Email", "EmailConfirmed", "FullName", "IsActive", "LastModifiedAt", "LastModifiedBy", "LastPasswordChangeAt", "LockoutEnabled", "LockoutEnd", "PasswordHash", "PasswordResetToken", "PasswordResetTokenExpiry", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "TwoFactorRecoveryCode", "TwoFactorSecretKey", "UserType", "Username" },
                values: new object[] { new Guid("bcab4262-01ff-410f-9948-179b1cf9154b"), 0, "2f24947e-db09-4b53-a116-03d93f03044f", new DateTime(2025, 4, 21, 20, 26, 49, 238, DateTimeKind.Utc).AddTicks(3169), "System", "admin@example.com", true, "Administrator", true, new DateTime(2025, 4, 21, 20, 26, 49, 238, DateTimeKind.Utc).AddTicks(3182), "System", null, true, null, "$2a$11$Hw4nzSysTUJHwhtGCo2CqeItVFtM0xCFUhZ5UUMdWtW7EABImD5A2", null, null, null, true, "667c1a71-e35f-494a-8782-d69cee647e58", false, null, null, 1, "admin" });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "IsActive", "LastModifiedAt", "LastModifiedBy", "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("51aa030a-2ce2-400d-b6a5-d9d6cbb419d3"), new DateTime(2025, 4, 21, 20, 26, 49, 130, DateTimeKind.Utc).AddTicks(4081), "System", true, new DateTime(2025, 4, 21, 20, 26, 49, 130, DateTimeKind.Utc).AddTicks(4082), "System", new Guid("ffdaccc5-f1ed-446e-8166-878ee297d743"), new Guid("d7e350e8-5fb7-4517-b8da-6f602d66a3a9") },
                    { new Guid("6cf33b76-46cd-4420-9c51-f0ffac6486a4"), new DateTime(2025, 4, 21, 20, 26, 49, 130, DateTimeKind.Utc).AddTicks(4083), "System", true, new DateTime(2025, 4, 21, 20, 26, 49, 130, DateTimeKind.Utc).AddTicks(4084), "System", new Guid("034726f0-f521-40ae-8208-a15628794e7d"), new Guid("d7e350e8-5fb7-4517-b8da-6f602d66a3a9") },
                    { new Guid("bcaddc1f-c5da-48d0-8408-eccc202b9295"), new DateTime(2025, 4, 21, 20, 26, 49, 130, DateTimeKind.Utc).AddTicks(4077), "System", true, new DateTime(2025, 4, 21, 20, 26, 49, 130, DateTimeKind.Utc).AddTicks(4078), "System", new Guid("607d2005-63f7-4cfc-b9ab-b16077bf041c"), new Guid("d7e350e8-5fb7-4517-b8da-6f602d66a3a9") },
                    { new Guid("d723e258-3efb-4a82-958a-524c44f98ea2"), new DateTime(2025, 4, 21, 20, 26, 49, 130, DateTimeKind.Utc).AddTicks(4085), "System", true, new DateTime(2025, 4, 21, 20, 26, 49, 130, DateTimeKind.Utc).AddTicks(4086), "System", new Guid("607d2005-63f7-4cfc-b9ab-b16077bf041c"), new Guid("e1a8268c-d086-4098-aaf4-e326b3cd1dee") },
                    { new Guid("f3714d9b-9721-447c-a6e7-33de247c806f"), new DateTime(2025, 4, 21, 20, 26, 49, 130, DateTimeKind.Utc).AddTicks(4079), "System", true, new DateTime(2025, 4, 21, 20, 26, 49, 130, DateTimeKind.Utc).AddTicks(4080), "System", new Guid("a5c08ec3-eeef-4910-8b15-3f9529c39a70"), new Guid("d7e350e8-5fb7-4517-b8da-6f602d66a3a9") }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "IsActive", "LastModifiedAt", "LastModifiedBy", "RoleId", "UserId" },
                values: new object[] { new Guid("b071fca7-4389-4cc7-bed5-b8c695a1f4b6"), new DateTime(2025, 4, 21, 20, 26, 49, 238, DateTimeKind.Utc).AddTicks(3769), "System", true, new DateTime(2025, 4, 21, 20, 26, 49, 238, DateTimeKind.Utc).AddTicks(3770), "System", new Guid("d7e350e8-5fb7-4517-b8da-6f602d66a3a9"), new Guid("bcab4262-01ff-410f-9948-179b1cf9154b") });

            migrationBuilder.CreateIndex(
                name: "IX_Modules_Name",
                table: "Modules",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Modules_ParentId",
                table: "Modules",
                column: "ParentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Modules");

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("51aa030a-2ce2-400d-b6a5-d9d6cbb419d3"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("6cf33b76-46cd-4420-9c51-f0ffac6486a4"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("bcaddc1f-c5da-48d0-8408-eccc202b9295"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("d723e258-3efb-4a82-958a-524c44f98ea2"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("f3714d9b-9721-447c-a6e7-33de247c806f"));

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("b071fca7-4389-4cc7-bed5-b8c695a1f4b6"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("034726f0-f521-40ae-8208-a15628794e7d"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("607d2005-63f7-4cfc-b9ab-b16077bf041c"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("a5c08ec3-eeef-4910-8b15-3f9529c39a70"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("ffdaccc5-f1ed-446e-8166-878ee297d743"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("d7e350e8-5fb7-4517-b8da-6f602d66a3a9"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("e1a8268c-d086-4098-aaf4-e326b3cd1dee"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("bcab4262-01ff-410f-9948-179b1cf9154b"));

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Description", "IsActive", "LastModifiedAt", "LastModifiedBy", "Name" },
                values: new object[,]
                {
                    { new Guid("39dbc70f-5115-4999-b26b-4476115fe09e"), new DateTime(2025, 4, 19, 5, 18, 56, 895, DateTimeKind.Utc).AddTicks(163), "System", "Ver usuarios", true, new DateTime(2025, 4, 19, 5, 18, 56, 895, DateTimeKind.Utc).AddTicks(164), "System", "users.view" },
                    { new Guid("72138c65-e260-419d-b83d-9bf2476a32b8"), new DateTime(2025, 4, 19, 5, 18, 56, 895, DateTimeKind.Utc).AddTicks(165), "System", "Crear usuarios", true, new DateTime(2025, 4, 19, 5, 18, 56, 895, DateTimeKind.Utc).AddTicks(166), "System", "users.create" },
                    { new Guid("86155368-e34b-484a-b717-72ef484b6087"), new DateTime(2025, 4, 19, 5, 18, 56, 895, DateTimeKind.Utc).AddTicks(201), "System", "Editar usuarios", true, new DateTime(2025, 4, 19, 5, 18, 56, 895, DateTimeKind.Utc).AddTicks(202), "System", "users.edit" },
                    { new Guid("c59219dd-ecab-40ce-8565-36377c641575"), new DateTime(2025, 4, 19, 5, 18, 56, 895, DateTimeKind.Utc).AddTicks(203), "System", "Eliminar usuarios", true, new DateTime(2025, 4, 19, 5, 18, 56, 895, DateTimeKind.Utc).AddTicks(204), "System", "users.delete" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Description", "IsActive", "LastModifiedAt", "LastModifiedBy", "Name" },
                values: new object[,]
                {
                    { new Guid("0b565c8b-393c-4365-9817-c59b2c8d869d"), new DateTime(2025, 4, 19, 5, 18, 56, 895, DateTimeKind.Utc).AddTicks(13), "System", "Administrador del sistema", true, new DateTime(2025, 4, 19, 5, 18, 56, 895, DateTimeKind.Utc).AddTicks(16), "System", "Admin" },
                    { new Guid("376587dc-283d-492c-a80b-49cf8c6cb11a"), new DateTime(2025, 4, 19, 5, 18, 56, 895, DateTimeKind.Utc).AddTicks(23), "System", "Usuario estándar", true, new DateTime(2025, 4, 19, 5, 18, 56, 895, DateTimeKind.Utc).AddTicks(23), "System", "User" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedAt", "CreatedBy", "Email", "EmailConfirmed", "FullName", "IsActive", "LastModifiedAt", "LastModifiedBy", "LastPasswordChangeAt", "LockoutEnabled", "LockoutEnd", "PasswordHash", "PasswordResetToken", "PasswordResetTokenExpiry", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "TwoFactorRecoveryCode", "TwoFactorSecretKey", "UserType", "Username" },
                values: new object[] { new Guid("f63828d9-87a8-434f-b460-291a63b2732f"), 0, "7a384959-39de-4486-b7fe-78e743541f9b", new DateTime(2025, 4, 19, 5, 18, 56, 999, DateTimeKind.Utc).AddTicks(9306), "System", "admin@example.com", true, "Administrator", true, new DateTime(2025, 4, 19, 5, 18, 56, 999, DateTimeKind.Utc).AddTicks(9315), "System", null, true, null, "$2a$11$wzI009jBP3C/YSxPS9jwQuFBy/bvGy52t.jg/dN8kf3nbThDIARbS", null, null, null, true, "ed279eb9-0705-4055-bb1a-056b4cc77515", false, null, null, 1, "admin" });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "IsActive", "LastModifiedAt", "LastModifiedBy", "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("7220c88a-1ee4-45fe-91b1-34c73a2c57ad"), new DateTime(2025, 4, 19, 5, 18, 56, 895, DateTimeKind.Utc).AddTicks(243), "System", true, new DateTime(2025, 4, 19, 5, 18, 56, 895, DateTimeKind.Utc).AddTicks(244), "System", new Guid("39dbc70f-5115-4999-b26b-4476115fe09e"), new Guid("376587dc-283d-492c-a80b-49cf8c6cb11a") },
                    { new Guid("7b396271-d5eb-428d-827d-f840488a1301"), new DateTime(2025, 4, 19, 5, 18, 56, 895, DateTimeKind.Utc).AddTicks(235), "System", true, new DateTime(2025, 4, 19, 5, 18, 56, 895, DateTimeKind.Utc).AddTicks(236), "System", new Guid("72138c65-e260-419d-b83d-9bf2476a32b8"), new Guid("0b565c8b-393c-4365-9817-c59b2c8d869d") },
                    { new Guid("8ba1f416-11ef-4ad5-9db6-82c068f6b2e0"), new DateTime(2025, 4, 19, 5, 18, 56, 895, DateTimeKind.Utc).AddTicks(233), "System", true, new DateTime(2025, 4, 19, 5, 18, 56, 895, DateTimeKind.Utc).AddTicks(234), "System", new Guid("39dbc70f-5115-4999-b26b-4476115fe09e"), new Guid("0b565c8b-393c-4365-9817-c59b2c8d869d") },
                    { new Guid("cfd610f0-e324-4a69-b726-e5c84560e4e0"), new DateTime(2025, 4, 19, 5, 18, 56, 895, DateTimeKind.Utc).AddTicks(241), "System", true, new DateTime(2025, 4, 19, 5, 18, 56, 895, DateTimeKind.Utc).AddTicks(242), "System", new Guid("c59219dd-ecab-40ce-8565-36377c641575"), new Guid("0b565c8b-393c-4365-9817-c59b2c8d869d") },
                    { new Guid("eeac088f-34f9-499a-b5f7-114b72e22eb5"), new DateTime(2025, 4, 19, 5, 18, 56, 895, DateTimeKind.Utc).AddTicks(239), "System", true, new DateTime(2025, 4, 19, 5, 18, 56, 895, DateTimeKind.Utc).AddTicks(240), "System", new Guid("86155368-e34b-484a-b717-72ef484b6087"), new Guid("0b565c8b-393c-4365-9817-c59b2c8d869d") }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "IsActive", "LastModifiedAt", "LastModifiedBy", "RoleId", "UserId" },
                values: new object[] { new Guid("2e94db85-9918-4b3f-a09b-cb886f8c2867"), new DateTime(2025, 4, 19, 5, 18, 57, 0, DateTimeKind.Utc).AddTicks(272), "System", true, new DateTime(2025, 4, 19, 5, 18, 57, 0, DateTimeKind.Utc).AddTicks(273), "System", new Guid("0b565c8b-393c-4365-9817-c59b2c8d869d"), new Guid("f63828d9-87a8-434f-b460-291a63b2732f") });
        }
    }
}
