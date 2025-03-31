using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeepItUp.MagJob.Identity.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class AddRowVersionToEntityBase : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<byte[]>(
            name: "RowVersion",
            schema: "identity",
            table: "Users",
            type: "bytea",
            rowVersion: true,
            nullable: false,
            defaultValue: new byte[0]);

        migrationBuilder.AddColumn<byte[]>(
            name: "RowVersion",
            schema: "identity",
            table: "Roles",
            type: "bytea",
            rowVersion: true,
            nullable: false,
            defaultValue: new byte[0]);

        migrationBuilder.AddColumn<byte[]>(
            name: "RowVersion",
            schema: "identity",
            table: "Organizations",
            type: "bytea",
            rowVersion: true,
            nullable: false,
            defaultValue: new byte[0]);

        migrationBuilder.AddColumn<byte[]>(
            name: "RowVersion",
            schema: "identity",
            table: "Members",
            type: "bytea",
            rowVersion: true,
            nullable: false,
            defaultValue: new byte[0]);

        migrationBuilder.AddColumn<byte[]>(
            name: "RowVersion",
            schema: "identity",
            table: "Invitations",
            type: "bytea",
            rowVersion: true,
            nullable: false,
            defaultValue: new byte[0]);

        migrationBuilder.AddColumn<byte[]>(
            name: "RowVersion",
            schema: "identity",
            table: "Contributors",
            type: "bytea",
            rowVersion: true,
            nullable: false,
            defaultValue: new byte[0]);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "RowVersion",
            schema: "identity",
            table: "Users");

        migrationBuilder.DropColumn(
            name: "RowVersion",
            schema: "identity",
            table: "Roles");

        migrationBuilder.DropColumn(
            name: "RowVersion",
            schema: "identity",
            table: "Organizations");

        migrationBuilder.DropColumn(
            name: "RowVersion",
            schema: "identity",
            table: "Members");

        migrationBuilder.DropColumn(
            name: "RowVersion",
            schema: "identity",
            table: "Invitations");

        migrationBuilder.DropColumn(
            name: "RowVersion",
            schema: "identity",
            table: "Contributors");
    }
}
