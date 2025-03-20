using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeepItUp.MagJob.Identity.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class ChangeUserExternalIdType : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // First ensure the uuid-ossp extension is created
        migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS \"uuid-ossp\";");

        // Use raw SQL with explicit USING clause to convert string to UUID
        migrationBuilder.Sql("ALTER TABLE identity.\"Users\" ALTER COLUMN \"ExternalId\" TYPE uuid USING \"ExternalId\"::uuid;");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Convert back to string (text) from UUID
        migrationBuilder.Sql("ALTER TABLE identity.\"Users\" ALTER COLUMN \"ExternalId\" TYPE character varying(64) USING \"ExternalId\"::text;");
    }
}
