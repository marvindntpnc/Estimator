using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Estimator.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTarifficatorEntitAndCreateEstimateEntitySchemaMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TarifficatorItems_Tarifficators_TarificatorId",
                table: "TarifficatorItems");

            migrationBuilder.DropTable(
                name: "Tarifficators");

            migrationBuilder.DropIndex(
                name: "IX_TarifficatorItems_TarificatorId",
                table: "TarifficatorItems");

            migrationBuilder.RenameColumn(
                name: "TarificatorId",
                table: "TarifficatorItems",
                newName: "TarifficatorType");

            migrationBuilder.CreateTable(
                name: "Estimates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LocationName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrencyRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsDiscounts = table.Column<bool>(type: "bit", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estimates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EstimateItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EstimateId = table.Column<int>(type: "int", nullable: false),
                    TarifficatorItemId = table.Column<int>(type: "int", nullable: false),
                    Qty = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstimateItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EstimateItems_Estimates_EstimateId",
                        column: x => x.EstimateId,
                        principalTable: "Estimates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EstimateItems_EstimateId",
                table: "EstimateItems",
                column: "EstimateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EstimateItems");

            migrationBuilder.DropTable(
                name: "Estimates");

            migrationBuilder.RenameColumn(
                name: "TarifficatorType",
                table: "TarifficatorItems",
                newName: "TarificatorId");

            migrationBuilder.CreateTable(
                name: "Tarifficators",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    TarifficatorType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tarifficators", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TarifficatorItems_TarificatorId",
                table: "TarifficatorItems",
                column: "TarificatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_TarifficatorItems_Tarifficators_TarificatorId",
                table: "TarifficatorItems",
                column: "TarificatorId",
                principalTable: "Tarifficators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
