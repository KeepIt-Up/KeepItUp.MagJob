using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeepItUp.MagJob.Identity.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class AddIndexesForMemberOrganizationRole : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "IX_Roles_Name",
            schema: "identity",
            table: "Roles",
            column: "Name");

        migrationBuilder.CreateIndex(
            name: "IX_RolePermissions_RoleId",
            schema: "identity",
            table: "RolePermissions",
            column: "RoleId");

        migrationBuilder.CreateIndex(
            name: "IX_Organizations_Id",
            schema: "identity",
            table: "Organizations",
            column: "Id",
            descending: new bool[0]);

        migrationBuilder.CreateIndex(
            name: "IX_Organizations_IsActive",
            schema: "identity",
            table: "Organizations",
            column: "IsActive");

        migrationBuilder.CreateIndex(
            name: "IX_Organizations_OwnerId",
            schema: "identity",
            table: "Organizations",
            column: "OwnerId");

        migrationBuilder.CreateIndex(
            name: "IX_Members_UserId",
            schema: "identity",
            table: "Members",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_MemberRoles_MemberId",
            schema: "identity",
            table: "MemberRoles",
            column: "MemberId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Roles_Name",
            schema: "identity",
            table: "Roles");

        migrationBuilder.DropIndex(
            name: "IX_RolePermissions_RoleId",
            schema: "identity",
            table: "RolePermissions");

        migrationBuilder.DropIndex(
            name: "IX_Organizations_Id",
            schema: "identity",
            table: "Organizations");

        migrationBuilder.DropIndex(
            name: "IX_Organizations_IsActive",
            schema: "identity",
            table: "Organizations");

        migrationBuilder.DropIndex(
            name: "IX_Organizations_OwnerId",
            schema: "identity",
            table: "Organizations");

        migrationBuilder.DropIndex(
            name: "IX_Members_UserId",
            schema: "identity",
            table: "Members");

        migrationBuilder.DropIndex(
            name: "IX_MemberRoles_MemberId",
            schema: "identity",
            table: "MemberRoles");
    }
}
