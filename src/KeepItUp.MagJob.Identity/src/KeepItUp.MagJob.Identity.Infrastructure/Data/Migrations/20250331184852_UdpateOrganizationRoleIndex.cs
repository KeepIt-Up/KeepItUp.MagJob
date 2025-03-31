using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeepItUp.MagJob.Identity.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class UdpateOrganizationRoleIndex : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Roles_Name_OrganizationId",
            schema: "identity",
            table: "Roles");

        migrationBuilder.CreateIndex(
            name: "IX_Roles_Id_OrganizationId",
            schema: "identity",
            table: "Roles",
            columns: new[] { "Id", "OrganizationId" },
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Roles_Id_OrganizationId",
            schema: "identity",
            table: "Roles");

        migrationBuilder.CreateIndex(
            name: "IX_Roles_Name_OrganizationId",
            schema: "identity",
            table: "Roles",
            columns: new[] { "Name", "OrganizationId" },
            unique: true);
    }
}
