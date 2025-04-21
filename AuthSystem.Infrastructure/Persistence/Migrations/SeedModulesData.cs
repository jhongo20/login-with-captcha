using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AuthSystem.Infrastructure.Persistence.Migrations
{
    public partial class SeedModulesData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Módulo principal: Dashboard
            migrationBuilder.InsertData(
                table: "Modules",
                columns: new[] { "Id", "Name", "Description", "Route", "Icon", "DisplayOrder", "ParentId", "IsEnabled", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 
                    Guid.NewGuid().ToString(), 
                    "Dashboard", 
                    "Panel principal del sistema", 
                    "/dashboard", 
                    "fa-tachometer-alt", 
                    1, 
                    null, 
                    true, 
                    DateTime.UtcNow, 
                    "System", 
                    DateTime.UtcNow, 
                    "System" 
                });

            // Módulo principal: Administración
            var adminModuleId = Guid.NewGuid();
            migrationBuilder.InsertData(
                table: "Modules",
                columns: new[] { "Id", "Name", "Description", "Route", "Icon", "DisplayOrder", "ParentId", "IsEnabled", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 
                    adminModuleId.ToString(), 
                    "Administración", 
                    "Módulo de administración del sistema", 
                    "/admin", 
                    "fa-cogs", 
                    2, 
                    null, 
                    true, 
                    DateTime.UtcNow, 
                    "System", 
                    DateTime.UtcNow, 
                    "System" 
                });

            // Submódulo: Usuarios (hijo de Administración)
            migrationBuilder.InsertData(
                table: "Modules",
                columns: new[] { "Id", "Name", "Description", "Route", "Icon", "DisplayOrder", "ParentId", "IsEnabled", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 
                    Guid.NewGuid().ToString(), 
                    "Usuarios", 
                    "Gestión de usuarios del sistema", 
                    "/admin/users", 
                    "fa-users", 
                    1, 
                    adminModuleId.ToString(), 
                    true, 
                    DateTime.UtcNow, 
                    "System", 
                    DateTime.UtcNow, 
                    "System" 
                });

            // Submódulo: Roles (hijo de Administración)
            migrationBuilder.InsertData(
                table: "Modules",
                columns: new[] { "Id", "Name", "Description", "Route", "Icon", "DisplayOrder", "ParentId", "IsEnabled", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 
                    Guid.NewGuid().ToString(), 
                    "Roles", 
                    "Gestión de roles y permisos", 
                    "/admin/roles", 
                    "fa-user-shield", 
                    2, 
                    adminModuleId.ToString(), 
                    true, 
                    DateTime.UtcNow, 
                    "System", 
                    DateTime.UtcNow, 
                    "System" 
                });

            // Módulo principal: Reportes
            migrationBuilder.InsertData(
                table: "Modules",
                columns: new[] { "Id", "Name", "Description", "Route", "Icon", "DisplayOrder", "ParentId", "IsEnabled", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 
                    Guid.NewGuid().ToString(), 
                    "Reportes", 
                    "Módulo de reportes y estadísticas", 
                    "/reports", 
                    "fa-chart-bar", 
                    3, 
                    null, 
                    true, 
                    DateTime.UtcNow, 
                    "System", 
                    DateTime.UtcNow, 
                    "System" 
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Name",
                keyValue: "Dashboard");

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Name",
                keyValue: "Administración");

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Name",
                keyValue: "Usuarios");

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Name",
                keyValue: "Roles");

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Name",
                keyValue: "Reportes");
        }
    }
}
