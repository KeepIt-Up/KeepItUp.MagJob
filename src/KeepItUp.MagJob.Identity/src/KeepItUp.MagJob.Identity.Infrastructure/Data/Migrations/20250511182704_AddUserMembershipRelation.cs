using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeepItUp.MagJob.Identity.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class AddUserMembershipRelation : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Permissions",
            schema: "identity",
            table: "Users",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddForeignKey(
            name: "FK_Members_Users_UserId",
            schema: "identity",
            table: "Members",
            column: "UserId",
            principalSchema: "identity",
            principalTable: "Users",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Members_Users_UserId",
            schema: "identity",
            table: "Members");

        migrationBuilder.DropColumn(
            name: "Permissions",
            schema: "identity",
            table: "Users");
    }
}
