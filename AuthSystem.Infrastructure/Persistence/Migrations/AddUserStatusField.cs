using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AuthSystem.Infrastructure.Persistence.Migrations
{
    /// <summary>
    /// Migración para agregar el campo UserStatus a la tabla Users
    /// </summary>
    public class AddUserStatusField : Migration
    {
        /// <summary>
        /// Método que se ejecuta al aplicar la migración
        /// </summary>
        /// <param name="migrationBuilder">Constructor de migraciones</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Agregar la columna UserStatus a la tabla Users
            migrationBuilder.AddColumn<int>(
                name: "UserStatus",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 1); // 1 = Active por defecto

            // Actualizar los valores de UserStatus basados en IsActive
            migrationBuilder.Sql(@"
                UPDATE Users
                SET UserStatus = CASE 
                    WHEN IsActive = 1 THEN 1 -- Active
                    ELSE 2 -- Inactive
                END
            ");
        }

        /// <summary>
        /// Método que se ejecuta al revertir la migración
        /// </summary>
        /// <param name="migrationBuilder">Constructor de migraciones</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Eliminar la columna UserStatus de la tabla Users
            migrationBuilder.DropColumn(
                name: "UserStatus",
                table: "Users");
        }
    }
}
