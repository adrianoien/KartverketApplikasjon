using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KartverketApplikasjon.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnsToGeoChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReviewComment",
                table: "GeoChanges",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ReviewedBy",
                table: "GeoChanges",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewedDate",
                table: "GeoChanges",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReviewComment",
                table: "GeoChanges");

            migrationBuilder.DropColumn(
                name: "ReviewedBy",
                table: "GeoChanges");

            migrationBuilder.DropColumn(
                name: "ReviewedDate",
                table: "GeoChanges");
        }
    }
}
