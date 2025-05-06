using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaPlus.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldsToShow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IsScheduled",
                table: "Show",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledFrom",
                table: "Show",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ScheduledRunNo",
                table: "Show",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledTo",
                table: "Show",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsScheduled",
                table: "Show");

            migrationBuilder.DropColumn(
                name: "ScheduledFrom",
                table: "Show");

            migrationBuilder.DropColumn(
                name: "ScheduledRunNo",
                table: "Show");

            migrationBuilder.DropColumn(
                name: "ScheduledTo",
                table: "Show");
        }
    }
}
