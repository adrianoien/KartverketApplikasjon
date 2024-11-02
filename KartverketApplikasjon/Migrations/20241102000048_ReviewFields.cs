using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KartverketApplikasjon.Migrations
{
    /// <inheritdoc />
    public partial class ReviewFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReviewComment",
                table: "MapCorrections",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ReviewedBy",
                table: "MapCorrections",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewedDate",
                table: "MapCorrections",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "MapCorrections",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SubmittedBy",
                table: "MapCorrections",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmittedDate",
                table: "MapCorrections",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReviewComment",
                table: "MapCorrections");

            migrationBuilder.DropColumn(
                name: "ReviewedBy",
                table: "MapCorrections");

            migrationBuilder.DropColumn(
                name: "ReviewedDate",
                table: "MapCorrections");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "MapCorrections");

            migrationBuilder.DropColumn(
                name: "SubmittedBy",
                table: "MapCorrections");

            migrationBuilder.DropColumn(
                name: "SubmittedDate",
                table: "MapCorrections");
        }
    }
}
