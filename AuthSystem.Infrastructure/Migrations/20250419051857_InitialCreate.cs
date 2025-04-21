using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AuthSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UserType = table.Column<int>(type: "int", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false),
                    SecurityStamp = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorSecretKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TwoFactorRecoveryCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PasswordResetToken = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PasswordResetTokenExpiry = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastPasswordChangeAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DeviceInfo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastActivity = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Name",
                table: "Permissions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleId_PermissionId",
                table: "RolePermissions",
                columns: new[] { "RoleId", "PermissionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId_RoleId",
                table: "UserRoles",
                columns: new[] { "UserId", "RoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_RefreshToken",
                table: "UserSessions",
                column: "RefreshToken",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_UserId",
                table: "UserSessions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserSessions");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
