using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeepItUp.MagJob.Identity.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class NewMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invitations_Organizations_OrganizationId",
                schema: "identity",
                table: "Invitations");

            migrationBuilder.DropIndex(
                name: "IX_Invitations_OrganizationId",
                schema: "identity",
                table: "Invitations");

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

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "identity",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "identity",
                table: "Roles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "identity",
                table: "Organizations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "identity",
                table: "Members",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "identity",
                table: "Invitations",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "identity",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "identity",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "identity",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "identity",
                table: "Invitations");

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

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_OrganizationId",
                schema: "identity",
                table: "Invitations",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invitations_Organizations_OrganizationId",
                schema: "identity",
                table: "Invitations",
                column: "OrganizationId",
                principalSchema: "identity",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
