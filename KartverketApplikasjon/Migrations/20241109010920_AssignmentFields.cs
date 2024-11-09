using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KartverketApplikasjon.Migrations
{
    /// <inheritdoc />
    public partial class AssignmentFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssignedTo",
                table: "MapCorrections",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "AssignmentDate",
                table: "MapCorrections",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssignmentNotes",
                table: "MapCorrections",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "AssignmentStatus",
                table: "MapCorrections",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "MapCorrections",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "MapCorrections",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "AssignedTo",
                table: "GeoChanges",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "AssignmentDate",
                table: "GeoChanges",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssignmentNotes",
                table: "GeoChanges",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "AssignmentStatus",
                table: "GeoChanges",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "GeoChanges",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "GeoChanges",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedTo",
                table: "MapCorrections");

            migrationBuilder.DropColumn(
                name: "AssignmentDate",
                table: "MapCorrections");

            migrationBuilder.DropColumn(
                name: "AssignmentNotes",
                table: "MapCorrections");

            migrationBuilder.DropColumn(
                name: "AssignmentStatus",
                table: "MapCorrections");

            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "MapCorrections");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "MapCorrections");

            migrationBuilder.DropColumn(
                name: "AssignedTo",
                table: "GeoChanges");

            migrationBuilder.DropColumn(
                name: "AssignmentDate",
                table: "GeoChanges");

            migrationBuilder.DropColumn(
                name: "AssignmentNotes",
                table: "GeoChanges");

            migrationBuilder.DropColumn(
                name: "AssignmentStatus",
                table: "GeoChanges");

            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "GeoChanges");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "GeoChanges");
        }
    }
}
