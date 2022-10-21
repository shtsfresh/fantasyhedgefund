using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fantasy_Hedge_Fund.Data.Migrations
{
    public partial class update5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastPriceUpdate",
                table: "Assets",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastPriceUpdate",
                table: "Assets");
        }
    }
}
