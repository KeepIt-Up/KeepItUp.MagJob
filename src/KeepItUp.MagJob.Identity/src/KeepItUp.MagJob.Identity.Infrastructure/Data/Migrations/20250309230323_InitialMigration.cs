using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace KeepItUp.MagJob.Identity.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class InitialMigration : Migration
{
  /// <inheritdoc />
  protected override void Up(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.EnsureSchema(
        name: "identity");

    migrationBuilder.RenameTable(
        name: "Contributors",
        newName: "Contributors",
        newSchema: "identity");

    migrationBuilder.AlterDatabase()
        .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

    migrationBuilder.AlterColumn<int>(
        name: "Status",
        schema: "identity",
        table: "Contributors",
        type: "integer",
        nullable: false,
        oldClrType: typeof(int),
        oldType: "INTEGER");

    migrationBuilder.AlterColumn<string>(
        name: "PhoneNumber_Number",
        schema: "identity",
        table: "Contributors",
        type: "text",
        nullable: true,
        oldClrType: typeof(string),
        oldType: "TEXT",
        oldNullable: true);

    migrationBuilder.AlterColumn<string>(
        name: "PhoneNumber_Extension",
        schema: "identity",
        table: "Contributors",
        type: "text",
        nullable: true,
        oldClrType: typeof(string),
        oldType: "TEXT",
        oldNullable: true);

    migrationBuilder.AlterColumn<string>(
        name: "PhoneNumber_CountryCode",
        schema: "identity",
        table: "Contributors",
        type: "text",
        nullable: true,
        oldClrType: typeof(string),
        oldType: "TEXT",
        oldNullable: true);

    migrationBuilder.AlterColumn<string>(
        name: "Name",
        schema: "identity",
        table: "Contributors",
        type: "character varying(100)",
        maxLength: 100,
        nullable: false,
        oldClrType: typeof(string),
        oldType: "TEXT",
        oldMaxLength: 100);

    migrationBuilder.AlterColumn<int>(
        name: "Id",
        schema: "identity",
        table: "Contributors",
        type: "integer",
        nullable: false,
        oldClrType: typeof(int),
        oldType: "INTEGER")
        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

    migrationBuilder.CreateTable(
        name: "Organizations",
        schema: "identity",
        columns: table => new
        {
          Id = table.Column<Guid>(type: "uuid", nullable: false),
          Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
          Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
          OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
          IsActive = table.Column<bool>(type: "boolean", nullable: false),
          CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
          UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_Organizations", x => x.Id);
        });

    migrationBuilder.CreateTable(
        name: "Permissions",
        schema: "identity",
        columns: table => new
        {
          Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
          Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_Permissions", x => x.Name);
        });

    migrationBuilder.CreateTable(
        name: "Users",
        schema: "identity",
        columns: table => new
        {
          Id = table.Column<Guid>(type: "uuid", nullable: false),
          ExternalId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
          Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
          FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
          LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
          Profile_PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
          Profile_Address = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
          Profile_ProfileImage = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
          Profile_IsProfileCreated = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true),
          IsActive = table.Column<bool>(type: "boolean", nullable: false),
          CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
          UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_Users", x => x.Id);
        });

    migrationBuilder.CreateTable(
        name: "Invitations",
        schema: "identity",
        columns: table => new
        {
          Id = table.Column<Guid>(type: "uuid", nullable: false),
          OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
          Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
          Token = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
          RoleId = table.Column<Guid>(type: "uuid", nullable: false),
          Status = table.Column<string>(type: "text", nullable: false),
          ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
          CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
          UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_Invitations", x => x.Id);
          table.ForeignKey(
                    name: "FK_Invitations_Organizations_OrganizationId",
                    column: x => x.OrganizationId,
                    principalSchema: "identity",
                    principalTable: "Organizations",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
        });

    migrationBuilder.CreateTable(
        name: "Members",
        schema: "identity",
        columns: table => new
        {
          Id = table.Column<Guid>(type: "uuid", nullable: false),
          UserId = table.Column<Guid>(type: "uuid", nullable: false),
          OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
          JoinedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
          RoleIds = table.Column<string>(type: "text", nullable: false),
          CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
          UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_Members", x => x.Id);
          table.ForeignKey(
                    name: "FK_Members_Organizations_OrganizationId",
                    column: x => x.OrganizationId,
                    principalSchema: "identity",
                    principalTable: "Organizations",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
        });

    migrationBuilder.CreateTable(
        name: "Roles",
        schema: "identity",
        columns: table => new
        {
          Id = table.Column<Guid>(type: "uuid", nullable: false),
          Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
          Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
          Color = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
          OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
          CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
          UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_Roles", x => x.Id);
          table.ForeignKey(
                    name: "FK_Roles_Organizations_OrganizationId",
                    column: x => x.OrganizationId,
                    principalSchema: "identity",
                    principalTable: "Organizations",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
        });

    migrationBuilder.CreateTable(
        name: "MemberRoles",
        schema: "identity",
        columns: table => new
        {
          MemberId = table.Column<Guid>(type: "uuid", nullable: false),
          RoleId = table.Column<Guid>(type: "uuid", nullable: false)
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_MemberRoles", x => new { x.MemberId, x.RoleId });
          table.ForeignKey(
                    name: "FK_MemberRoles_Members_MemberId",
                    column: x => x.MemberId,
                    principalSchema: "identity",
                    principalTable: "Members",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
          table.ForeignKey(
                    name: "FK_MemberRoles_Roles_RoleId",
                    column: x => x.RoleId,
                    principalSchema: "identity",
                    principalTable: "Roles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
        });

    migrationBuilder.CreateTable(
        name: "RolePermissions",
        schema: "identity",
        columns: table => new
        {
          RoleId = table.Column<Guid>(type: "uuid", nullable: false),
          PermissionId = table.Column<string>(type: "character varying(100)", nullable: false)
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
          table.ForeignKey(
                    name: "FK_RolePermissions_Permissions_PermissionId",
                    column: x => x.PermissionId,
                    principalSchema: "identity",
                    principalTable: "Permissions",
                    principalColumn: "Name",
                    onDelete: ReferentialAction.Cascade);
          table.ForeignKey(
                    name: "FK_RolePermissions_Roles_RoleId",
                    column: x => x.RoleId,
                    principalSchema: "identity",
                    principalTable: "Roles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
        });

    migrationBuilder.InsertData(
        schema: "identity",
        table: "Permissions",
        columns: new[] { "Name", "Description" },
        values: new object[,]
        {
                  { "invitations.manage", "Zarządzanie zaproszeniami do organizacji" },
                  { "invitations.view", "Przeglądanie zaproszeń do organizacji" },
                  { "members.manage", "Zarządzanie członkami organizacji" },
                  { "members.view", "Przeglądanie członków organizacji" },
                  { "organization.manage", "Zarządzanie organizacją" },
                  { "organization.view", "Przeglądanie organizacji" },
                  { "roles.manage", "Zarządzanie rolami w organizacji" },
                  { "roles.view", "Przeglądanie ról w organizacji" }
        });

    migrationBuilder.CreateIndex(
        name: "IX_Invitations_Email_OrganizationId_Status",
        schema: "identity",
        table: "Invitations",
        columns: new[] { "Email", "OrganizationId", "Status" });

    migrationBuilder.CreateIndex(
        name: "IX_Invitations_OrganizationId",
        schema: "identity",
        table: "Invitations",
        column: "OrganizationId");

    migrationBuilder.CreateIndex(
        name: "IX_Invitations_Token",
        schema: "identity",
        table: "Invitations",
        column: "Token",
        unique: true);

    migrationBuilder.CreateIndex(
        name: "IX_MemberRoles_RoleId",
        schema: "identity",
        table: "MemberRoles",
        column: "RoleId");

    migrationBuilder.CreateIndex(
        name: "IX_Members_OrganizationId",
        schema: "identity",
        table: "Members",
        column: "OrganizationId");

    migrationBuilder.CreateIndex(
        name: "IX_Members_UserId_OrganizationId",
        schema: "identity",
        table: "Members",
        columns: new[] { "UserId", "OrganizationId" },
        unique: true);

    migrationBuilder.CreateIndex(
        name: "IX_Organizations_Name",
        schema: "identity",
        table: "Organizations",
        column: "Name");

    migrationBuilder.CreateIndex(
        name: "IX_RolePermissions_PermissionId",
        schema: "identity",
        table: "RolePermissions",
        column: "PermissionId");

    migrationBuilder.CreateIndex(
        name: "IX_Roles_Name_OrganizationId",
        schema: "identity",
        table: "Roles",
        columns: new[] { "Name", "OrganizationId" },
        unique: true);

    migrationBuilder.CreateIndex(
        name: "IX_Roles_OrganizationId",
        schema: "identity",
        table: "Roles",
        column: "OrganizationId");

    migrationBuilder.CreateIndex(
        name: "IX_Users_Email",
        schema: "identity",
        table: "Users",
        column: "Email",
        unique: true);

    migrationBuilder.CreateIndex(
        name: "IX_Users_ExternalId",
        schema: "identity",
        table: "Users",
        column: "ExternalId",
        unique: true);
  }

  /// <inheritdoc />
  protected override void Down(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.DropTable(
        name: "Invitations",
        schema: "identity");

    migrationBuilder.DropTable(
        name: "MemberRoles",
        schema: "identity");

    migrationBuilder.DropTable(
        name: "RolePermissions",
        schema: "identity");

    migrationBuilder.DropTable(
        name: "Users",
        schema: "identity");

    migrationBuilder.DropTable(
        name: "Members",
        schema: "identity");

    migrationBuilder.DropTable(
        name: "Permissions",
        schema: "identity");

    migrationBuilder.DropTable(
        name: "Roles",
        schema: "identity");

    migrationBuilder.DropTable(
        name: "Organizations",
        schema: "identity");

    migrationBuilder.RenameTable(
        name: "Contributors",
        schema: "identity",
        newName: "Contributors");

    migrationBuilder.AlterDatabase()
        .OldAnnotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

    migrationBuilder.AlterColumn<int>(
        name: "Status",
        table: "Contributors",
        type: "INTEGER",
        nullable: false,
        oldClrType: typeof(int),
        oldType: "integer");

    migrationBuilder.AlterColumn<string>(
        name: "PhoneNumber_Number",
        table: "Contributors",
        type: "TEXT",
        nullable: true,
        oldClrType: typeof(string),
        oldType: "text",
        oldNullable: true);

    migrationBuilder.AlterColumn<string>(
        name: "PhoneNumber_Extension",
        table: "Contributors",
        type: "TEXT",
        nullable: true,
        oldClrType: typeof(string),
        oldType: "text",
        oldNullable: true);

    migrationBuilder.AlterColumn<string>(
        name: "PhoneNumber_CountryCode",
        table: "Contributors",
        type: "TEXT",
        nullable: true,
        oldClrType: typeof(string),
        oldType: "text",
        oldNullable: true);

    migrationBuilder.AlterColumn<string>(
        name: "Name",
        table: "Contributors",
        type: "TEXT",
        maxLength: 100,
        nullable: false,
        oldClrType: typeof(string),
        oldType: "character varying(100)",
        oldMaxLength: 100);

    migrationBuilder.AlterColumn<int>(
        name: "Id",
        table: "Contributors",
        type: "INTEGER",
        nullable: false,
        oldClrType: typeof(int),
        oldType: "integer")
        .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
  }
}
