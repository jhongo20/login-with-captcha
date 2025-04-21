using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AuthSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedModulesData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Modules_Modules_ParentId",
                table: "Modules");

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
                keyValue: new Guid("e1a8268c-d086-4098-aaf4-e326b3cd1dee"));

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "Modules",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Route",
                table: "Modules",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Icon",
                table: "Modules",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "ModuleId",
                table: "Modules",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Modules",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Description", "DisplayOrder", "Icon", "IsActive", "IsEnabled", "LastModifiedAt", "LastModifiedBy", "ModuleId", "Name", "ParentId", "Route", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("090e0db3-8ee2-4729-9095-fc358fbee9bf"), new DateTime(2025, 4, 21, 20, 44, 25, 957, DateTimeKind.Utc).AddTicks(7269), "System", "Módulo de reportes y estadísticas", 3, "fa-chart-bar", true, true, new DateTime(2025, 4, 21, 20, 44, 25, 957, DateTimeKind.Utc).AddTicks(7270), "System", null, "Reportes", null, "/reports", null, null },
                    { new Guid("70d4253b-8b9f-4c90-871b-98c4073050fd"), new DateTime(2025, 4, 21, 20, 44, 25, 957, DateTimeKind.Utc).AddTicks(7108), "System", "Panel principal del sistema", 1, "fa-tachometer-alt", true, true, new DateTime(2025, 4, 21, 20, 44, 25, 957, DateTimeKind.Utc).AddTicks(7108), "System", null, "Dashboard", null, "/dashboard", null, null },
                    { new Guid("da9d9c11-b242-4c9b-8611-8eb008765bec"), new DateTime(2025, 4, 21, 20, 44, 25, 957, DateTimeKind.Utc).AddTicks(7143), "System", "Módulo de administración del sistema", 2, "fa-cogs", true, true, new DateTime(2025, 4, 21, 20, 44, 25, 957, DateTimeKind.Utc).AddTicks(7143), "System", null, "Administración", null, "/admin", null, null }
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Description", "IsActive", "LastModifiedAt", "LastModifiedBy", "Name" },
                values: new object[,]
                {
                    { new Guid("2a1ccb43-fa4f-48ce-b148-32d3bd6dae19"), new DateTime(2025, 4, 21, 20, 44, 25, 819, DateTimeKind.Utc).AddTicks(8053), "System", "Ver usuarios", true, new DateTime(2025, 4, 21, 20, 44, 25, 819, DateTimeKind.Utc).AddTicks(8054), "System", "users.view" },
                    { new Guid("5c3a4a58-2c25-4a9d-b641-a7a35f9d3c95"), new DateTime(2025, 4, 21, 20, 44, 25, 819, DateTimeKind.Utc).AddTicks(8055), "System", "Crear usuarios", true, new DateTime(2025, 4, 21, 20, 44, 25, 819, DateTimeKind.Utc).AddTicks(8056), "System", "users.create" },
                    { new Guid("7b073c81-8bcd-4a93-96e3-8ef64b87960f"), new DateTime(2025, 4, 21, 20, 44, 25, 819, DateTimeKind.Utc).AddTicks(8099), "System", "Editar usuarios", true, new DateTime(2025, 4, 21, 20, 44, 25, 819, DateTimeKind.Utc).AddTicks(8100), "System", "users.edit" },
                    { new Guid("a9bb2c4d-4c46-4eba-b27a-4b2127a0df5f"), new DateTime(2025, 4, 21, 20, 44, 25, 819, DateTimeKind.Utc).AddTicks(8101), "System", "Eliminar usuarios", true, new DateTime(2025, 4, 21, 20, 44, 25, 819, DateTimeKind.Utc).AddTicks(8102), "System", "users.delete" }
                });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("d7e350e8-5fb7-4517-b8da-6f602d66a3a9"),
                columns: new[] { "CreatedAt", "LastModifiedAt" },
                values: new object[] { new DateTime(2025, 4, 21, 20, 44, 25, 819, DateTimeKind.Utc).AddTicks(7835), new DateTime(2025, 4, 21, 20, 44, 25, 819, DateTimeKind.Utc).AddTicks(7838) });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Description", "IsActive", "LastModifiedAt", "LastModifiedBy", "Name" },
                values: new object[] { new Guid("f7d36113-51ea-4448-a9d2-d9151d5ac28b"), new DateTime(2025, 4, 21, 20, 44, 25, 819, DateTimeKind.Utc).AddTicks(7853), "System", "Usuario estándar", true, new DateTime(2025, 4, 21, 20, 44, 25, 819, DateTimeKind.Utc).AddTicks(7853), "System", "User" });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "IsActive", "LastModifiedAt", "LastModifiedBy", "RoleId", "UserId" },
                values: new object[] { new Guid("e2475aa1-4ac9-44b3-bcb1-c50f08594af4"), new DateTime(2025, 4, 21, 20, 44, 25, 957, DateTimeKind.Utc).AddTicks(6704), "System", true, new DateTime(2025, 4, 21, 20, 44, 25, 957, DateTimeKind.Utc).AddTicks(6707), "System", new Guid("d7e350e8-5fb7-4517-b8da-6f602d66a3a9"), new Guid("bcab4262-01ff-410f-9948-179b1cf9154b") });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("bcab4262-01ff-410f-9948-179b1cf9154b"),
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "LastModifiedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "6e0e824a-9dc9-434d-bf76-9fd59f24ebd5", new DateTime(2025, 4, 21, 20, 44, 25, 957, DateTimeKind.Utc).AddTicks(5781), new DateTime(2025, 4, 21, 20, 44, 25, 957, DateTimeKind.Utc).AddTicks(5791), "$2a$11$LWuztS0wDjyf/g8vMaCS2.rsrRmLKD94nprEnpZr3VJvYlcWiqsk2", "39b570eb-fb1a-4a48-bf8c-44b73bd3c9ee" });

            migrationBuilder.InsertData(
                table: "Modules",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Description", "DisplayOrder", "Icon", "IsActive", "IsEnabled", "LastModifiedAt", "LastModifiedBy", "ModuleId", "Name", "ParentId", "Route", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("69a19da5-b2c6-4a75-91ca-843f00caa2e9"), new DateTime(2025, 4, 21, 20, 44, 25, 957, DateTimeKind.Utc).AddTicks(7185), "System", "Gestión de usuarios del sistema", 1, "fa-users", true, true, new DateTime(2025, 4, 21, 20, 44, 25, 957, DateTimeKind.Utc).AddTicks(7186), "System", null, "Usuarios", new Guid("da9d9c11-b242-4c9b-8611-8eb008765bec"), "/admin/users", null, null },
                    { new Guid("876d50d8-c17a-41b9-8317-0de67c6ceba9"), new DateTime(2025, 4, 21, 20, 44, 25, 957, DateTimeKind.Utc).AddTicks(7216), "System", "Gestión de roles y permisos", 2, "fa-user-shield", true, true, new DateTime(2025, 4, 21, 20, 44, 25, 957, DateTimeKind.Utc).AddTicks(7216), "System", null, "Roles", new Guid("da9d9c11-b242-4c9b-8611-8eb008765bec"), "/admin/roles", null, null }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "IsActive", "LastModifiedAt", "LastModifiedBy", "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("4ff9de14-94a3-4726-9181-aff62299d437"), new DateTime(2025, 4, 21, 20, 44, 25, 957, DateTimeKind.Utc).AddTicks(6898), "System", true, new DateTime(2025, 4, 21, 20, 44, 25, 957, DateTimeKind.Utc).AddTicks(6899), "System", new Guid("2a1ccb43-fa4f-48ce-b148-32d3bd6dae19"), new Guid("f7d36113-51ea-4448-a9d2-d9151d5ac28b") },
                    { new Guid("91bc8cbb-8378-49b1-808a-c9ccc75a02b8"), new DateTime(2025, 4, 21, 20, 44, 25, 957, DateTimeKind.Utc).AddTicks(6813), "System", true, new DateTime(2025, 4, 21, 20, 44, 25, 957, DateTimeKind.Utc).AddTicks(6814), "System", new Guid("7b073c81-8bcd-4a93-96e3-8ef64b87960f"), new Guid("d7e350e8-5fb7-4517-b8da-6f602d66a3a9") },
                    { new Guid("a3d9d72b-353d-4c3b-871c-c620c4d92247"), new DateTime(2025, 4, 21, 20, 44, 25, 957, DateTimeKind.Utc).AddTicks(6799), "System", true, new DateTime(2025, 4, 21, 20, 44, 25, 957, DateTimeKind.Utc).AddTicks(6800), "System", new Guid("2a1ccb43-fa4f-48ce-b148-32d3bd6dae19"), new Guid("d7e350e8-5fb7-4517-b8da-6f602d66a3a9") },
                    { new Guid("e9ab943c-80fb-4335-ae12-102b8a669a40"), new DateTime(2025, 4, 21, 20, 44, 25, 957, DateTimeKind.Utc).AddTicks(6803), "System", true, new DateTime(2025, 4, 21, 20, 44, 25, 957, DateTimeKind.Utc).AddTicks(6804), "System", new Guid("5c3a4a58-2c25-4a9d-b641-a7a35f9d3c95"), new Guid("d7e350e8-5fb7-4517-b8da-6f602d66a3a9") },
                    { new Guid("f7f06804-7088-4e7b-ad1a-f6b7251df0f2"), new DateTime(2025, 4, 21, 20, 44, 25, 957, DateTimeKind.Utc).AddTicks(6819), "System", true, new DateTime(2025, 4, 21, 20, 44, 25, 957, DateTimeKind.Utc).AddTicks(6819), "System", new Guid("a9bb2c4d-4c46-4eba-b27a-4b2127a0df5f"), new Guid("d7e350e8-5fb7-4517-b8da-6f602d66a3a9") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Modules_ModuleId",
                table: "Modules",
                column: "ModuleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Modules_Modules_ModuleId",
                table: "Modules",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Modules_Modules_ParentId",
                table: "Modules",
                column: "ParentId",
                principalTable: "Modules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Modules_Modules_ModuleId",
                table: "Modules");

            migrationBuilder.DropForeignKey(
                name: "FK_Modules_Modules_ParentId",
                table: "Modules");

            migrationBuilder.DropIndex(
                name: "IX_Modules_ModuleId",
                table: "Modules");

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: new Guid("090e0db3-8ee2-4729-9095-fc358fbee9bf"));

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: new Guid("69a19da5-b2c6-4a75-91ca-843f00caa2e9"));

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: new Guid("70d4253b-8b9f-4c90-871b-98c4073050fd"));

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: new Guid("876d50d8-c17a-41b9-8317-0de67c6ceba9"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("4ff9de14-94a3-4726-9181-aff62299d437"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("91bc8cbb-8378-49b1-808a-c9ccc75a02b8"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("a3d9d72b-353d-4c3b-871c-c620c4d92247"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("e9ab943c-80fb-4335-ae12-102b8a669a40"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("f7f06804-7088-4e7b-ad1a-f6b7251df0f2"));

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("e2475aa1-4ac9-44b3-bcb1-c50f08594af4"));

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: new Guid("da9d9c11-b242-4c9b-8611-8eb008765bec"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("2a1ccb43-fa4f-48ce-b148-32d3bd6dae19"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("5c3a4a58-2c25-4a9d-b641-a7a35f9d3c95"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("7b073c81-8bcd-4a93-96e3-8ef64b87960f"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("a9bb2c4d-4c46-4eba-b27a-4b2127a0df5f"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("f7d36113-51ea-4448-a9d2-d9151d5ac28b"));

            migrationBuilder.DropColumn(
                name: "ModuleId",
                table: "Modules");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "Modules",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Route",
                table: "Modules",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Icon",
                table: "Modules",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

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

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("d7e350e8-5fb7-4517-b8da-6f602d66a3a9"),
                columns: new[] { "CreatedAt", "LastModifiedAt" },
                values: new object[] { new DateTime(2025, 4, 21, 20, 26, 49, 130, DateTimeKind.Utc).AddTicks(3877), new DateTime(2025, 4, 21, 20, 26, 49, 130, DateTimeKind.Utc).AddTicks(3879) });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Description", "IsActive", "LastModifiedAt", "LastModifiedBy", "Name" },
                values: new object[] { new Guid("e1a8268c-d086-4098-aaf4-e326b3cd1dee"), new DateTime(2025, 4, 21, 20, 26, 49, 130, DateTimeKind.Utc).AddTicks(3886), "System", "Usuario estándar", true, new DateTime(2025, 4, 21, 20, 26, 49, 130, DateTimeKind.Utc).AddTicks(3886), "System", "User" });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "IsActive", "LastModifiedAt", "LastModifiedBy", "RoleId", "UserId" },
                values: new object[] { new Guid("b071fca7-4389-4cc7-bed5-b8c695a1f4b6"), new DateTime(2025, 4, 21, 20, 26, 49, 238, DateTimeKind.Utc).AddTicks(3769), "System", true, new DateTime(2025, 4, 21, 20, 26, 49, 238, DateTimeKind.Utc).AddTicks(3770), "System", new Guid("d7e350e8-5fb7-4517-b8da-6f602d66a3a9"), new Guid("bcab4262-01ff-410f-9948-179b1cf9154b") });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("bcab4262-01ff-410f-9948-179b1cf9154b"),
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "LastModifiedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2f24947e-db09-4b53-a116-03d93f03044f", new DateTime(2025, 4, 21, 20, 26, 49, 238, DateTimeKind.Utc).AddTicks(3169), new DateTime(2025, 4, 21, 20, 26, 49, 238, DateTimeKind.Utc).AddTicks(3182), "$2a$11$Hw4nzSysTUJHwhtGCo2CqeItVFtM0xCFUhZ5UUMdWtW7EABImD5A2", "667c1a71-e35f-494a-8782-d69cee647e58" });

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

            migrationBuilder.AddForeignKey(
                name: "FK_Modules_Modules_ParentId",
                table: "Modules",
                column: "ParentId",
                principalTable: "Modules",
                principalColumn: "Id");
        }
    }
}
