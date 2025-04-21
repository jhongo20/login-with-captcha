using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AuthSystem.Infrastructure.Persistence.Migrations
{
    public partial class AddModulePermissions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Crear los permisos para módulos
            var viewModulesPermissionId = Guid.NewGuid();
            var createModulesPermissionId = Guid.NewGuid();
            var editModulesPermissionId = Guid.NewGuid();
            var deleteModulesPermissionId = Guid.NewGuid();

            // Insertar permisos para módulos
            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Name", "Description", "CreatedAt", "CreatedBy", "LastModifiedAt", "LastModifiedBy", "IsActive" },
                values: new object[] {
                    viewModulesPermissionId.ToString(),
                    "Modules.View",
                    "Permiso para ver módulos",
                    DateTime.UtcNow,
                    "System",
                    DateTime.UtcNow,
                    "System",
                    true
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Name", "Description", "CreatedAt", "CreatedBy", "LastModifiedAt", "LastModifiedBy", "IsActive" },
                values: new object[] {
                    createModulesPermissionId.ToString(),
                    "Modules.Create",
                    "Permiso para crear módulos",
                    DateTime.UtcNow,
                    "System",
                    DateTime.UtcNow,
                    "System",
                    true
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Name", "Description", "CreatedAt", "CreatedBy", "LastModifiedAt", "LastModifiedBy", "IsActive" },
                values: new object[] {
                    editModulesPermissionId.ToString(),
                    "Modules.Edit",
                    "Permiso para editar módulos",
                    DateTime.UtcNow,
                    "System",
                    DateTime.UtcNow,
                    "System",
                    true
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Name", "Description", "CreatedAt", "CreatedBy", "LastModifiedAt", "LastModifiedBy", "IsActive" },
                values: new object[] {
                    deleteModulesPermissionId.ToString(),
                    "Modules.Delete",
                    "Permiso para eliminar módulos",
                    DateTime.UtcNow,
                    "System",
                    DateTime.UtcNow,
                    "System",
                    true
                });

            // Asignar permisos al rol Admin (ID: D7E350E8-5FB7-4517-B8DA-6F602D66A3A9)
            var adminRoleId = "D7E350E8-5FB7-4517-B8DA-6F602D66A3A9";

            // Asignar permiso para ver módulos
            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "RoleId", "PermissionId", "CreatedAt", "CreatedBy", "LastModifiedAt", "LastModifiedBy", "IsActive" },
                values: new object[] {
                    Guid.NewGuid().ToString(),
                    adminRoleId,
                    viewModulesPermissionId.ToString(),
                    DateTime.UtcNow,
                    "System",
                    DateTime.UtcNow,
                    "System",
                    true
                });

            // Asignar permiso para crear módulos
            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "RoleId", "PermissionId", "CreatedAt", "CreatedBy", "LastModifiedAt", "LastModifiedBy", "IsActive" },
                values: new object[] {
                    Guid.NewGuid().ToString(),
                    adminRoleId,
                    createModulesPermissionId.ToString(),
                    DateTime.UtcNow,
                    "System",
                    DateTime.UtcNow,
                    "System",
                    true
                });

            // Asignar permiso para editar módulos
            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "RoleId", "PermissionId", "CreatedAt", "CreatedBy", "LastModifiedAt", "LastModifiedBy", "IsActive" },
                values: new object[] {
                    Guid.NewGuid().ToString(),
                    adminRoleId,
                    editModulesPermissionId.ToString(),
                    DateTime.UtcNow,
                    "System",
                    DateTime.UtcNow,
                    "System",
                    true
                });

            // Asignar permiso para eliminar módulos
            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "RoleId", "PermissionId", "CreatedAt", "CreatedBy", "LastModifiedAt", "LastModifiedBy", "IsActive" },
                values: new object[] {
                    Guid.NewGuid().ToString(),
                    adminRoleId,
                    deleteModulesPermissionId.ToString(),
                    DateTime.UtcNow,
                    "System",
                    DateTime.UtcNow,
                    "System",
                    true
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Eliminar los permisos de módulos
            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Name",
                keyValue: "Modules.View");

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Name",
                keyValue: "Modules.Create");

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Name",
                keyValue: "Modules.Edit");

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Name",
                keyValue: "Modules.Delete");
        }
    }
}
