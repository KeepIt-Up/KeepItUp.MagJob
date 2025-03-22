using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeepItUp.MagJob.Identity.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class UpdateUserInformations : Migration
{
  /// <inheritdoc />
  protected override void Up(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.AddColumn<DateTime>(
        name: "CreatedDate",
        schema: "identity",
        table: "Users",
        type: "timestamp with time zone",
        nullable: false,
        defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

    migrationBuilder.AddColumn<DateTime>(
        name: "LastLoginDate",
        schema: "identity",
        table: "Users",
        type: "timestamp with time zone",
        nullable: false,
        defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

    migrationBuilder.AddColumn<DateTime>(
        name: "LastModifiedDate",
        schema: "identity",
        table: "Users",
        type: "timestamp with time zone",
        nullable: false,
        defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

    migrationBuilder.AddColumn<List<string>>(
        name: "Permissions",
        schema: "identity",
        table: "Users",
        type: "text[]",
        nullable: false);

    migrationBuilder.AddColumn<string>(
        name: "Username",
        schema: "identity",
        table: "Users",
        type: "text",
        nullable: false,
        defaultValue: "");
  }

  /// <inheritdoc />
  protected override void Down(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.DropColumn(
        name: "CreatedDate",
        schema: "identity",
        table: "Users");

    migrationBuilder.DropColumn(
        name: "LastLoginDate",
        schema: "identity",
        table: "Users");

    migrationBuilder.DropColumn(
        name: "LastModifiedDate",
        schema: "identity",
        table: "Users");

    migrationBuilder.DropColumn(
        name: "Permissions",
        schema: "identity",
        table: "Users");

    migrationBuilder.DropColumn(
        name: "Username",
        schema: "identity",
        table: "Users");
  }
}
