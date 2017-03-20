using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DSHomework.Migrations
{
    public partial class Createdijinfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GoCost",
                table: "Sights",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Gone",
                table: "Sights",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PreviousSightId",
                table: "Sights",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GoCost",
                table: "Sights");

            migrationBuilder.DropColumn(
                name: "Gone",
                table: "Sights");

            migrationBuilder.DropColumn(
                name: "PreviousSightId",
                table: "Sights");
        }
    }
}
