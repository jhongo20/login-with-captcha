using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AuthSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserStatusField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Modules_Modules_ModuleId",
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

            migrationBuilder.DropColumn(
                name: "ModuleId",
                table: "Modules");

            migrationBuilder.AlterColumn<string>(
                name: "IpAddress",
                table: "UserSessions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "DeviceInfo",
                table: "UserSessions",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "UserSessions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "ConcurrencyStamp",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<int>(
                name: "UserStatus",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "UserRoles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Roles",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Roles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "RolePermissions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Permissions",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Permissions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Route",
                table: "Modules",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Icon",
                table: "Modules",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Modules",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Modules",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.CreateTable(
                name: "ActivationCodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivationCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivationCodes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmailTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    HtmlContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TextContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PermissionModules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionModules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PermissionModules_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PermissionModules_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Routes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Path = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    HttpMethod = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    RequiresAuth = table.Column<bool>(type: "bit", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ModuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Routes_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PermissionRoutes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RouteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionRoutes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PermissionRoutes_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PermissionRoutes_Routes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "Routes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleRoutes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RouteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleRoutes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleRoutes_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoleRoutes_Routes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "Routes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "EmailTemplates",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Description", "HtmlContent", "IsActive", "Name", "Subject", "TextContent", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("6b5e0d6a-2a6b-3ab7-84dc-5cf6b3e3a6c6"), new DateTime(2025, 4, 24, 22, 18, 24, 501, DateTimeKind.Utc).AddTicks(9846), "System", "Plantilla para el correo de restablecimiento de contraseña", "\n                        <html>\n                        <head>\n                            <style>\n                                body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }\n                                .container { width: 100%; max-width: 600px; margin: 0 auto; padding: 20px; }\n                                .header { background-color: #4a6da7; color: white; padding: 10px 20px; text-align: center; }\n                                .content { padding: 20px; background-color: #f9f9f9; }\n                                .button { display: inline-block; padding: 10px 20px; background-color: #4a6da7; color: white; text-decoration: none; border-radius: 5px; margin: 20px 0; }\n                                .footer { text-align: center; padding: 10px; font-size: 12px; color: #666; }\n                            </style>\n                        </head>\n                        <body>\n                            <div class='container'>\n                                <div class='header'>\n                                    <h1>Restablecimiento de Contraseña</h1>\n                                </div>\n                                <div class='content'>\n                                    <p>Hola <strong>{{FullName}}</strong>,</p>\n                                    <p>Has solicitado restablecer la contraseña de tu cuenta en AuthSystem.</p>\n                                    <p>Para restablecer tu contraseña, haz clic en el siguiente enlace:</p>\n                                    <p style='text-align: center;'><a href='{{ResetUrl}}' class='button'>Restablecer Contraseña</a></p>\n                                    <p>O copia y pega la siguiente URL en tu navegador:</p>\n                                    <p>{{ResetUrl}}</p>\n                                    <p>Este enlace es válido por {{ExpirationTime}} a partir de ahora.</p>\n                                    <p>Si no has solicitado restablecer tu contraseña, por favor ignora este mensaje o contacta a nuestro equipo de soporte.</p>\n                                    <p>Saludos cordiales,<br>El equipo de AuthSystem</p>\n                                </div>\n                                <div class='footer'>\n                                    <p>Este es un correo electrónico automático, por favor no respondas a este mensaje.</p>\n                                </div>\n                            </div>\n                        </body>\n                        </html>", true, "PasswordReset", "Restablecimiento de Contraseña - AuthSystem", "\n                        Restablecimiento de Contraseña\n\n                        Hola {{FullName}},\n\n                        Has solicitado restablecer la contraseña de tu cuenta en AuthSystem.\n\n                        Para restablecer tu contraseña, visita el siguiente enlace:\n                        {{ResetUrl}}\n\n                        Este enlace es válido por {{ExpirationTime}} a partir de ahora.\n\n                        Si no has solicitado restablecer tu contraseña, por favor ignora este mensaje o contacta a nuestro equipo de soporte.\n\n                        Saludos cordiales,\n                        El equipo de AuthSystem\n\n                        Este es un correo electrónico automático, por favor no respondas a este mensaje.", null, null },
                    { new Guid("7a6f0c7b-3a7c-4bc8-95ed-6de58f4a4e7d"), new DateTime(2025, 4, 24, 22, 18, 24, 501, DateTimeKind.Utc).AddTicks(9842), "System", "Plantilla para el correo con código de activación", "\n                        <html>\n                        <head>\n                            <style>\n                                body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }\n                                .container { width: 100%; max-width: 600px; margin: 0 auto; padding: 20px; }\n                                .header { background-color: #4a6da7; color: white; padding: 10px 20px; text-align: center; }\n                                .content { padding: 20px; background-color: #f9f9f9; }\n                                .code { font-size: 24px; font-weight: bold; text-align: center; padding: 15px; background-color: #e9e9e9; margin: 20px 0; letter-spacing: 5px; }\n                                .footer { text-align: center; padding: 10px; font-size: 12px; color: #666; }\n                            </style>\n                        </head>\n                        <body>\n                            <div class='container'>\n                                <div class='header'>\n                                    <h1>Código de Activación</h1>\n                                </div>\n                                <div class='content'>\n                                    <p>Hola <strong>{{FullName}}</strong>,</p>\n                                    <p>Has solicitado un código de activación para tu cuenta en AuthSystem.</p>\n                                    <p>Tu código de activación es:</p>\n                                    <div class='code'>{{ActivationCode}}</div>\n                                    <p>Este código es válido por {{ExpirationTime}} a partir de ahora.</p>\n                                    <p>Si no has solicitado este código, por favor ignora este mensaje o contacta a nuestro equipo de soporte.</p>\n                                    <p>Saludos cordiales,<br>El equipo de AuthSystem</p>\n                                </div>\n                                <div class='footer'>\n                                    <p>Este es un correo electrónico automático, por favor no respondas a este mensaje.</p>\n                                </div>\n                            </div>\n                        </body>\n                        </html>", true, "ActivationCode", "Código de Activación - AuthSystem", "\n                        Código de Activación\n\n                        Hola {{FullName}},\n\n                        Has solicitado un código de activación para tu cuenta en AuthSystem.\n\n                        Tu código de activación es: {{ActivationCode}}\n\n                        Este código es válido por {{ExpirationTime}} a partir de ahora.\n\n                        Si no has solicitado este código, por favor ignora este mensaje o contacta a nuestro equipo de soporte.\n\n                        Saludos cordiales,\n                        El equipo de AuthSystem\n\n                        Este es un correo electrónico automático, por favor no respondas a este mensaje.", null, null },
                    { new Guid("8c7e0a8d-4b8d-4cd9-a6ed-7de69f4a5e8e"), new DateTime(2025, 4, 24, 22, 18, 24, 501, DateTimeKind.Utc).AddTicks(9817), "System", "Plantilla para el correo de bienvenida cuando se crea un usuario", "\n                        <html>\n                        <head>\n                            <style>\n                                body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }\n                                .container { width: 100%; max-width: 600px; margin: 0 auto; padding: 20px; }\n                                .header { background-color: #4a6da7; color: white; padding: 10px 20px; text-align: center; }\n                                .content { padding: 20px; background-color: #f9f9f9; }\n                                .footer { text-align: center; padding: 10px; font-size: 12px; color: #666; }\n                            </style>\n                        </head>\n                        <body>\n                            <div class='container'>\n                                <div class='header'>\n                                    <h1>¡Bienvenido a AuthSystem!</h1>\n                                </div>\n                                <div class='content'>\n                                    <p>Hola <strong>{{FullName}}</strong>,</p>\n                                    <p>Tu cuenta ha sido creada exitosamente en nuestro sistema.</p>\n                                    <p>Detalles de tu cuenta:</p>\n                                    <ul>\n                                        <li><strong>Usuario:</strong> {{Username}}</li>\n                                        <li><strong>Correo electrónico:</strong> {{Email}}</li>\n                                        <li><strong>Fecha de creación:</strong> {{CurrentDate}}</li>\n                                    </ul>\n                                    <p>Ya puedes iniciar sesión en nuestra plataforma y comenzar a utilizarla.</p>\n                                    <p>Si tienes alguna pregunta, no dudes en contactarnos.</p>\n                                    <p>Saludos cordiales,<br>El equipo de AuthSystem</p>\n                                </div>\n                                <div class='footer'>\n                                    <p>Este es un correo electrónico automático, por favor no respondas a este mensaje.</p>\n                                </div>\n                            </div>\n                        </body>\n                        </html>", true, "UserCreated", "Bienvenido a AuthSystem", "\n                        ¡Bienvenido a AuthSystem!\n\n                        Hola {{FullName}},\n\n                        Tu cuenta ha sido creada exitosamente en nuestro sistema.\n\n                        Detalles de tu cuenta:\n                        - Usuario: {{Username}}\n                        - Correo electrónico: {{Email}}\n                        - Fecha de creación: {{CurrentDate}}\n\n                        Ya puedes iniciar sesión en nuestra plataforma y comenzar a utilizarla.\n\n                        Si tienes alguna pregunta, no dudes en contactarnos.\n\n                        Saludos cordiales,\n                        El equipo de AuthSystem\n\n                        Este es un correo electrónico automático, por favor no respondas a este mensaje.", null, null },
                    { new Guid("9d8f0b9e-5c9e-5de0-b7fe-8ef7a5f6b9f9"), new DateTime(2025, 4, 24, 22, 18, 24, 501, DateTimeKind.Utc).AddTicks(9839), "System", "Plantilla para el correo de notificación cuando se actualiza un usuario", "\n                        <html>\n                        <head>\n                            <style>\n                                body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }\n                                .container { width: 100%; max-width: 600px; margin: 0 auto; padding: 20px; }\n                                .header { background-color: #4a6da7; color: white; padding: 10px 20px; text-align: center; }\n                                .content { padding: 20px; background-color: #f9f9f9; }\n                                .footer { text-align: center; padding: 10px; font-size: 12px; color: #666; }\n                            </style>\n                        </head>\n                        <body>\n                            <div class='container'>\n                                <div class='header'>\n                                    <h1>Actualización de Cuenta</h1>\n                                </div>\n                                <div class='content'>\n                                    <p>Hola <strong>{{FullName}}</strong>,</p>\n                                    <p>Tu cuenta ha sido actualizada en nuestro sistema.</p>\n                                    <p>Detalles de tu cuenta:</p>\n                                    <ul>\n                                        <li><strong>Usuario:</strong> {{Username}}</li>\n                                        <li><strong>Correo electrónico:</strong> {{Email}}</li>\n                                        <li><strong>Fecha de actualización:</strong> {{UpdateDate}}</li>\n                                    </ul>\n                                    <p>Si no has realizado esta actualización o tienes alguna pregunta, por favor contacta a nuestro equipo de soporte inmediatamente.</p>\n                                    <p>Saludos cordiales,<br>El equipo de AuthSystem</p>\n                                </div>\n                                <div class='footer'>\n                                    <p>Este es un correo electrónico automático, por favor no respondas a este mensaje.</p>\n                                </div>\n                            </div>\n                        </body>\n                        </html>", true, "UserUpdated", "Tu cuenta ha sido actualizada", "\n                        Actualización de Cuenta\n\n                        Hola {{FullName}},\n\n                        Tu cuenta ha sido actualizada en nuestro sistema.\n\n                        Detalles de tu cuenta:\n                        - Usuario: {{Username}}\n                        - Correo electrónico: {{Email}}\n                        - Fecha de actualización: {{UpdateDate}}\n\n                        Si no has realizado esta actualización o tienes alguna pregunta, por favor contacta a nuestro equipo de soporte inmediatamente.\n\n                        Saludos cordiales,\n                        El equipo de AuthSystem\n\n                        Este es un correo electrónico automático, por favor no respondas a este mensaje.", null, null }
                });

            migrationBuilder.InsertData(
                table: "Modules",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Description", "DisplayOrder", "Icon", "IsActive", "IsEnabled", "LastModifiedAt", "LastModifiedBy", "Name", "ParentId", "Route", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("0de607e9-988c-4948-8397-3469897f289e"), new DateTime(2025, 4, 24, 22, 18, 24, 501, DateTimeKind.Utc).AddTicks(9765), "System", "Módulo de reportes y estadísticas", 3, "fa-chart-bar", true, true, new DateTime(2025, 4, 24, 22, 18, 24, 501, DateTimeKind.Utc).AddTicks(9766), "System", "Reportes", null, "/reports", null, null },
                    { new Guid("557bdbf3-b609-417f-9e83-f35367dca56c"), new DateTime(2025, 4, 24, 22, 18, 24, 501, DateTimeKind.Utc).AddTicks(9644), "System", "Módulo de administración del sistema", 2, "fa-cogs", true, true, new DateTime(2025, 4, 24, 22, 18, 24, 501, DateTimeKind.Utc).AddTicks(9644), "System", "Administración", null, "/admin", null, null },
                    { new Guid("f834a42c-6cd2-41cf-9783-0f7044807c15"), new DateTime(2025, 4, 24, 22, 18, 24, 501, DateTimeKind.Utc).AddTicks(9512), "System", "Panel principal del sistema", 1, "fa-tachometer-alt", true, true, new DateTime(2025, 4, 24, 22, 18, 24, 501, DateTimeKind.Utc).AddTicks(9513), "System", "Dashboard", null, "/dashboard", null, null }
                });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("2a1ccb43-fa4f-48ce-b148-32d3bd6dae19"),
                columns: new[] { "CreatedAt", "Description", "LastModifiedAt" },
                values: new object[] { new DateTime(2025, 4, 24, 22, 18, 24, 397, DateTimeKind.Utc).AddTicks(7781), "Permiso para ver usuarios", new DateTime(2025, 4, 24, 22, 18, 24, 397, DateTimeKind.Utc).AddTicks(7782) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("5c3a4a58-2c25-4a9d-b641-a7a35f9d3c95"),
                columns: new[] { "CreatedAt", "Description", "LastModifiedAt" },
                values: new object[] { new DateTime(2025, 4, 24, 22, 18, 24, 397, DateTimeKind.Utc).AddTicks(7784), "Permiso para crear usuarios", new DateTime(2025, 4, 24, 22, 18, 24, 397, DateTimeKind.Utc).AddTicks(7785) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("7b073c81-8bcd-4a93-96e3-8ef64b87960f"),
                columns: new[] { "CreatedAt", "Description", "LastModifiedAt" },
                values: new object[] { new DateTime(2025, 4, 24, 22, 18, 24, 397, DateTimeKind.Utc).AddTicks(7786), "Permiso para editar usuarios", new DateTime(2025, 4, 24, 22, 18, 24, 397, DateTimeKind.Utc).AddTicks(7786) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("a9bb2c4d-4c46-4eba-b27a-4b2127a0df5f"),
                columns: new[] { "CreatedAt", "Description", "LastModifiedAt" },
                values: new object[] { new DateTime(2025, 4, 24, 22, 18, 24, 397, DateTimeKind.Utc).AddTicks(7788), "Permiso para eliminar usuarios", new DateTime(2025, 4, 24, 22, 18, 24, 397, DateTimeKind.Utc).AddTicks(7788) });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Description", "IsActive", "LastModifiedAt", "LastModifiedBy", "Name" },
                values: new object[,]
                {
                    { new Guid("7b8e8c2f-d39a-4b1a-b11e-0e39d3b7d8f3"), new DateTime(2025, 4, 24, 22, 18, 24, 397, DateTimeKind.Utc).AddTicks(7791), "System", "Permiso para crear módulos", true, new DateTime(2025, 4, 24, 22, 18, 24, 397, DateTimeKind.Utc).AddTicks(7792), "System", "Modules.Create" },
                    { new Guid("c4f907db-0f34-4610-b3cc-9fd1c4d323e7"), new DateTime(2025, 4, 24, 22, 18, 24, 397, DateTimeKind.Utc).AddTicks(7789), "System", "Permiso para ver módulos", true, new DateTime(2025, 4, 24, 22, 18, 24, 397, DateTimeKind.Utc).AddTicks(7790), "System", "Modules.View" },
                    { new Guid("e5d4c4f4-4a4f-4a4f-b0d1-302c9d40e00f"), new DateTime(2025, 4, 24, 22, 18, 24, 397, DateTimeKind.Utc).AddTicks(7793), "System", "Permiso para editar módulos", true, new DateTime(2025, 4, 24, 22, 18, 24, 397, DateTimeKind.Utc).AddTicks(7794), "System", "Modules.Edit" },
                    { new Guid("f8b2c5d5-5a5a-5a5a-c0e2-413d4e51f11f"), new DateTime(2025, 4, 24, 22, 18, 24, 397, DateTimeKind.Utc).AddTicks(7795), "System", "Permiso para eliminar módulos", true, new DateTime(2025, 4, 24, 22, 18, 24, 397, DateTimeKind.Utc).AddTicks(7796), "System", "Modules.Delete" }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "IsActive", "LastModifiedAt", "LastModifiedBy", "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("45b538a2-045a-4077-bae7-8496dddc0859"), new DateTime(2025, 4, 24, 22, 18, 24, 501, DateTimeKind.Utc).AddTicks(9288), "System", true, new DateTime(2025, 4, 24, 22, 18, 24, 501, DateTimeKind.Utc).AddTicks(9289), "System", new Guid("2a1ccb43-fa4f-48ce-b148-32d3bd6dae19"), new Guid("d7e350e8-5fb7-4517-b8da-6f602d66a3a9") },
                    { new Guid("50076e2d-8bc9-4d01-9120-ae2d0d6a478e"), new DateTime(2025, 4, 24, 22, 18, 24, 501, DateTimeKind.Utc).AddTicks(9291), "System", true, new DateTime(2025, 4, 24, 22, 18, 24, 501, DateTimeKind.Utc).AddTicks(9303), "System", new Guid("5c3a4a58-2c25-4a9d-b641-a7a35f9d3c95"), new Guid("d7e350e8-5fb7-4517-b8da-6f602d66a3a9") },
                    { new Guid("85e642cf-c7a1-4f52-9bb6-a7dad5b70fbf"), new DateTime(2025, 4, 24, 22, 18, 24, 501, DateTimeKind.Utc).AddTicks(9321), "System", true, new DateTime(2025, 4, 24, 22, 18, 24, 501, DateTimeKind.Utc).AddTicks(9321), "System", new Guid("a9bb2c4d-4c46-4eba-b27a-4b2127a0df5f"), new Guid("d7e350e8-5fb7-4517-b8da-6f602d66a3a9") },
                    { new Guid("f5396224-68b7-417b-a554-794862886053"), new DateTime(2025, 4, 24, 22, 18, 24, 501, DateTimeKind.Utc).AddTicks(9311), "System", true, new DateTime(2025, 4, 24, 22, 18, 24, 501, DateTimeKind.Utc).AddTicks(9312), "System", new Guid("7b073c81-8bcd-4a93-96e3-8ef64b87960f"), new Guid("d7e350e8-5fb7-4517-b8da-6f602d66a3a9") },
                    { new Guid("f9dbec3e-4842-457f-a940-e2737584c8d4"), new DateTime(2025, 4, 24, 22, 18, 24, 501, DateTimeKind.Utc).AddTicks(9323), "System", true, new DateTime(2025, 4, 24, 22, 18, 24, 501, DateTimeKind.Utc).AddTicks(9323), "System", new Guid("2a1ccb43-fa4f-48ce-b148-32d3bd6dae19"), new Guid("f7d36113-51ea-4448-a9d2-d9151d5ac28b") }
                });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("d7e350e8-5fb7-4517-b8da-6f602d66a3a9"),
                columns: new[] { "CreatedAt", "LastModifiedAt" },
                values: new object[] { new DateTime(2025, 4, 24, 22, 18, 24, 397, DateTimeKind.Utc).AddTicks(7573), new DateTime(2025, 4, 24, 22, 18, 24, 397, DateTimeKind.Utc).AddTicks(7577) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("f7d36113-51ea-4448-a9d2-d9151d5ac28b"),
                columns: new[] { "CreatedAt", "LastModifiedAt" },
                values: new object[] { new DateTime(2025, 4, 24, 22, 18, 24, 397, DateTimeKind.Utc).AddTicks(7585), new DateTime(2025, 4, 24, 22, 18, 24, 397, DateTimeKind.Utc).AddTicks(7586) });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "IsActive", "LastModifiedAt", "LastModifiedBy", "RoleId", "UserId" },
                values: new object[] { new Guid("1d02448e-45ca-496f-b2a1-a51858cc0c4b"), new DateTime(2025, 4, 24, 22, 18, 24, 501, DateTimeKind.Utc).AddTicks(9184), "System", true, new DateTime(2025, 4, 24, 22, 18, 24, 501, DateTimeKind.Utc).AddTicks(9184), "System", new Guid("d7e350e8-5fb7-4517-b8da-6f602d66a3a9"), new Guid("bcab4262-01ff-410f-9948-179b1cf9154b") });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("bcab4262-01ff-410f-9948-179b1cf9154b"),
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "LastModifiedAt", "PasswordHash", "SecurityStamp", "UserStatus" },
                values: new object[] { "9368ff47-4d98-41f5-b1d4-e08cb64f3c7a", new DateTime(2025, 4, 24, 22, 18, 24, 501, DateTimeKind.Utc).AddTicks(8659), new DateTime(2025, 4, 24, 22, 18, 24, 501, DateTimeKind.Utc).AddTicks(8665), "$2a$11$kvAUOkk4cCMwVPpGrE1eEuM9xfwdS.XfMQltB6QeAi8YzYbEtFV9C", "791bd9f3-5d45-4090-a0fe-238c0b7cb5f0", 1 });

            migrationBuilder.InsertData(
                table: "Modules",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Description", "DisplayOrder", "Icon", "IsActive", "IsEnabled", "LastModifiedAt", "LastModifiedBy", "Name", "ParentId", "Route", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("e0334eb7-50a1-4df0-bd1e-ab4bb87199e7"), new DateTime(2025, 4, 24, 22, 18, 24, 501, DateTimeKind.Utc).AddTicks(9740), "System", "Gestión de roles y permisos", 2, "fa-user-shield", true, true, new DateTime(2025, 4, 24, 22, 18, 24, 501, DateTimeKind.Utc).AddTicks(9740), "System", "Roles", new Guid("557bdbf3-b609-417f-9e83-f35367dca56c"), "/admin/roles", null, null },
                    { new Guid("efb75e85-c2f7-4077-93c1-e52ff7d8f4d6"), new DateTime(2025, 4, 24, 22, 18, 24, 501, DateTimeKind.Utc).AddTicks(9684), "System", "Gestión de usuarios del sistema", 1, "fa-users", true, true, new DateTime(2025, 4, 24, 22, 18, 24, 501, DateTimeKind.Utc).AddTicks(9685), "System", "Usuarios", new Guid("557bdbf3-b609-417f-9e83-f35367dca56c"), "/admin/users", null, null }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "IsActive", "LastModifiedAt", "LastModifiedBy", "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("09324def-36fb-4c3b-bbec-4c71f6424710"), new DateTime(2025, 4, 24, 22, 18, 24, 501, DateTimeKind.Utc).AddTicks(9329), "System", true, new DateTime(2025, 4, 24, 22, 18, 24, 501, DateTimeKind.Utc).AddTicks(9329), "System", new Guid("7b8e8c2f-d39a-4b1a-b11e-0e39d3b7d8f3"), new Guid("d7e350e8-5fb7-4517-b8da-6f602d66a3a9") },
                    { new Guid("63fe07c8-dd7e-4f05-af4b-cab265fc484d"), new DateTime(2025, 4, 24, 22, 18, 24, 501, DateTimeKind.Utc).AddTicks(9330), "System", true, new DateTime(2025, 4, 24, 22, 18, 24, 501, DateTimeKind.Utc).AddTicks(9331), "System", new Guid("e5d4c4f4-4a4f-4a4f-b0d1-302c9d40e00f"), new Guid("d7e350e8-5fb7-4517-b8da-6f602d66a3a9") },
                    { new Guid("9c7ad25f-8024-418f-a592-22f2dac6c238"), new DateTime(2025, 4, 24, 22, 18, 24, 501, DateTimeKind.Utc).AddTicks(9327), "System", true, new DateTime(2025, 4, 24, 22, 18, 24, 501, DateTimeKind.Utc).AddTicks(9327), "System", new Guid("c4f907db-0f34-4610-b3cc-9fd1c4d323e7"), new Guid("d7e350e8-5fb7-4517-b8da-6f602d66a3a9") },
                    { new Guid("beb496d6-60c2-4020-8cc0-b9c99c164c26"), new DateTime(2025, 4, 24, 22, 18, 24, 501, DateTimeKind.Utc).AddTicks(9332), "System", true, new DateTime(2025, 4, 24, 22, 18, 24, 501, DateTimeKind.Utc).AddTicks(9333), "System", new Guid("f8b2c5d5-5a5a-5a5a-c0e2-413d4e51f11f"), new Guid("d7e350e8-5fb7-4517-b8da-6f602d66a3a9") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivationCodes_UserId",
                table: "ActivationCodes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionModules_ModuleId",
                table: "PermissionModules",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionModules_PermissionId_ModuleId",
                table: "PermissionModules",
                columns: new[] { "PermissionId", "ModuleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PermissionRoutes_PermissionId_RouteId",
                table: "PermissionRoutes",
                columns: new[] { "PermissionId", "RouteId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PermissionRoutes_RouteId",
                table: "PermissionRoutes",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleRoutes_RoleId_RouteId",
                table: "RoleRoutes",
                columns: new[] { "RoleId", "RouteId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleRoutes_RouteId",
                table: "RoleRoutes",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_ModuleId",
                table: "Routes",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_Name_ModuleId",
                table: "Routes",
                columns: new[] { "Name", "ModuleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Routes_Path_HttpMethod",
                table: "Routes",
                columns: new[] { "Path", "HttpMethod" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivationCodes");

            migrationBuilder.DropTable(
                name: "EmailTemplates");

            migrationBuilder.DropTable(
                name: "PermissionModules");

            migrationBuilder.DropTable(
                name: "PermissionRoutes");

            migrationBuilder.DropTable(
                name: "RoleRoutes");

            migrationBuilder.DropTable(
                name: "Routes");

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: new Guid("0de607e9-988c-4948-8397-3469897f289e"));

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: new Guid("e0334eb7-50a1-4df0-bd1e-ab4bb87199e7"));

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: new Guid("efb75e85-c2f7-4077-93c1-e52ff7d8f4d6"));

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: new Guid("f834a42c-6cd2-41cf-9783-0f7044807c15"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("09324def-36fb-4c3b-bbec-4c71f6424710"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("45b538a2-045a-4077-bae7-8496dddc0859"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("50076e2d-8bc9-4d01-9120-ae2d0d6a478e"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("63fe07c8-dd7e-4f05-af4b-cab265fc484d"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("85e642cf-c7a1-4f52-9bb6-a7dad5b70fbf"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("9c7ad25f-8024-418f-a592-22f2dac6c238"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("beb496d6-60c2-4020-8cc0-b9c99c164c26"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("f5396224-68b7-417b-a554-794862886053"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("f9dbec3e-4842-457f-a940-e2737584c8d4"));

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("1d02448e-45ca-496f-b2a1-a51858cc0c4b"));

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: new Guid("557bdbf3-b609-417f-9e83-f35367dca56c"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("7b8e8c2f-d39a-4b1a-b11e-0e39d3b7d8f3"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("c4f907db-0f34-4610-b3cc-9fd1c4d323e7"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("e5d4c4f4-4a4f-4a4f-b0d1-302c9d40e00f"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("f8b2c5d5-5a5a-5a5a-c0e2-413d4e51f11f"));

            migrationBuilder.DropColumn(
                name: "UserStatus",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "IpAddress",
                table: "UserSessions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeviceInfo",
                table: "UserSessions",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "UserSessions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ConcurrencyStamp",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "UserRoles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Roles",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Roles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "RolePermissions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Permissions",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Permissions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Route",
                table: "Modules",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Icon",
                table: "Modules",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Modules",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Modules",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

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

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("2a1ccb43-fa4f-48ce-b148-32d3bd6dae19"),
                columns: new[] { "CreatedAt", "Description", "LastModifiedAt", "Name" },
                values: new object[] { new DateTime(2025, 4, 21, 20, 44, 25, 819, DateTimeKind.Utc).AddTicks(8053), "Ver usuarios", new DateTime(2025, 4, 21, 20, 44, 25, 819, DateTimeKind.Utc).AddTicks(8054), "users.view" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("5c3a4a58-2c25-4a9d-b641-a7a35f9d3c95"),
                columns: new[] { "CreatedAt", "Description", "LastModifiedAt", "Name" },
                values: new object[] { new DateTime(2025, 4, 21, 20, 44, 25, 819, DateTimeKind.Utc).AddTicks(8055), "Crear usuarios", new DateTime(2025, 4, 21, 20, 44, 25, 819, DateTimeKind.Utc).AddTicks(8056), "users.create" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("7b073c81-8bcd-4a93-96e3-8ef64b87960f"),
                columns: new[] { "CreatedAt", "Description", "LastModifiedAt", "Name" },
                values: new object[] { new DateTime(2025, 4, 21, 20, 44, 25, 819, DateTimeKind.Utc).AddTicks(8099), "Editar usuarios", new DateTime(2025, 4, 21, 20, 44, 25, 819, DateTimeKind.Utc).AddTicks(8100), "users.edit" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("a9bb2c4d-4c46-4eba-b27a-4b2127a0df5f"),
                columns: new[] { "CreatedAt", "Description", "LastModifiedAt", "Name" },
                values: new object[] { new DateTime(2025, 4, 21, 20, 44, 25, 819, DateTimeKind.Utc).AddTicks(8101), "Eliminar usuarios", new DateTime(2025, 4, 21, 20, 44, 25, 819, DateTimeKind.Utc).AddTicks(8102), "users.delete" });

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

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("d7e350e8-5fb7-4517-b8da-6f602d66a3a9"),
                columns: new[] { "CreatedAt", "LastModifiedAt" },
                values: new object[] { new DateTime(2025, 4, 21, 20, 44, 25, 819, DateTimeKind.Utc).AddTicks(7835), new DateTime(2025, 4, 21, 20, 44, 25, 819, DateTimeKind.Utc).AddTicks(7838) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("f7d36113-51ea-4448-a9d2-d9151d5ac28b"),
                columns: new[] { "CreatedAt", "LastModifiedAt" },
                values: new object[] { new DateTime(2025, 4, 21, 20, 44, 25, 819, DateTimeKind.Utc).AddTicks(7853), new DateTime(2025, 4, 21, 20, 44, 25, 819, DateTimeKind.Utc).AddTicks(7853) });

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
        }
    }
}
