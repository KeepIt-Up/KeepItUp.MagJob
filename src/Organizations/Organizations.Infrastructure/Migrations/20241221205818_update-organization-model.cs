using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organizations.Infrastructure.Migrations;

/// <inheritdoc />
public partial class updateorganizationmodel : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Archived",
            table: "Organizations");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "Archived",
            table: "Organizations",
            type: "boolean",
            nullable: false,
            defaultValue: false);
    }
}
