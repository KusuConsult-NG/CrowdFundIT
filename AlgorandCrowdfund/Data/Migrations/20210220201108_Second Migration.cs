using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AlgorandCrowdfund.Data.Migrations
{
    public partial class SecondMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountAddress",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "RequestFunds",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FundTitle = table.Column<string>(nullable: false),
                    FundDescription = table.Column<string>(nullable: false),
                    AmountNeeded = table.Column<int>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestFunds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestFunds_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Funders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<int>(nullable: false),
                    Key = table.Column<string>(nullable: true),
                    Receiver = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    RequestFundsId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Funders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Funders_RequestFunds_RequestFundsId",
                        column: x => x.RequestFundsId,
                        principalTable: "RequestFunds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Funders_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Funders_RequestFundsId",
                table: "Funders",
                column: "RequestFundsId");

            migrationBuilder.CreateIndex(
                name: "IX_Funders_UserId",
                table: "Funders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestFunds_UserId",
                table: "RequestFunds",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Funders");

            migrationBuilder.DropTable(
                name: "RequestFunds");

            migrationBuilder.DropColumn(
                name: "AccountAddress",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Key",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");
        }
    }
}
