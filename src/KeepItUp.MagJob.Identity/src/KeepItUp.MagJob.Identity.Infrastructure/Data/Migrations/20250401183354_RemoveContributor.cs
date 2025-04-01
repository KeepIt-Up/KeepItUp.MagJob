using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeepItUp.MagJob.Identity.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class RemoveContributor : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Contributors",
            schema: "identity");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Contributors",
            schema: "identity",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                Status = table.Column<int>(type: "integer", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                PhoneNumber_CountryCode = table.Column<string>(type: "text", nullable: true),
                PhoneNumber_Extension = table.Column<string>(type: "text", nullable: true),
                PhoneNumber_Number = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Contributors", x => x.Id);
            });
    }
}
