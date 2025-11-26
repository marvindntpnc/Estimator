using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Estimator.Migrations
{
    /// <inheritdoc />
    public partial class RefactoringAndUpdateAppSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrencyRate",
                table: "Estimates");

            migrationBuilder.DropColumn(
                name: "LocationName",
                table: "Estimates");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Estimates",
                newName: "CreatedOn");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "TarifficatorItems",
                type: "decimal(18,5)",
                precision: 18,
                scale: 5,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<bool>(
                name: "IsCustomAdding",
                table: "TarifficatorItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "TarifficatorItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "Estimates",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerName",
                table: "Estimates",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedOn",
                table: "Estimates",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContractId",
                table: "Estimates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DiscountMaterials",
                table: "Estimates",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EstimateName",
                table: "Estimates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FacilityId",
                table: "Estimates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "Qty",
                table: "EstimateItems",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<decimal>(
                name: "CustomRate",
                table: "EstimateItems",
                type: "decimal(18,5)",
                precision: 18,
                scale: 5,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "EstimateCurrencyRates",
                columns: table => new
                {
                    EstimateId = table.Column<int>(type: "int", nullable: false),
                    CurrencyType = table.Column<int>(type: "int", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstimateCurrencyRates", x => new { x.EstimateId, x.CurrencyType });
                });

            migrationBuilder.CreateTable(
                name: "Facilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StateName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AreaName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CityName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HouseNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EnclosureNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BuildingNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HourRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ActiveContractId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facilities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Contracts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FacilityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contracts_Facilities_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiscountRequirement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacilityId = table.Column<int>(type: "int", nullable: false),
                    StartRange = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EndRange = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UninstallRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InstallRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SuppliesRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountRequirement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscountRequirement_Facilities_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EstimateFacilities",
                columns: table => new
                {
                    EstimateId = table.Column<int>(type: "int", nullable: false),
                    FacilityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstimateFacilities", x => new { x.EstimateId, x.FacilityId });
                    table.ForeignKey(
                        name: "FK_EstimateFacilities_Estimates_EstimateId",
                        column: x => x.EstimateId,
                        principalTable: "Estimates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EstimateFacilities_Facilities_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_FacilityId",
                table: "Contracts",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountRequirement_FacilityId",
                table: "DiscountRequirement",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_EstimateFacilities_FacilityId",
                table: "EstimateFacilities",
                column: "FacilityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contracts");

            migrationBuilder.DropTable(
                name: "DiscountRequirement");

            migrationBuilder.DropTable(
                name: "EstimateCurrencyRates");

            migrationBuilder.DropTable(
                name: "EstimateFacilities");

            migrationBuilder.DropTable(
                name: "Facilities");

            migrationBuilder.DropColumn(
                name: "IsCustomAdding",
                table: "TarifficatorItems");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "TarifficatorItems");

            migrationBuilder.DropColumn(
                name: "ClosedOn",
                table: "Estimates");

            migrationBuilder.DropColumn(
                name: "ContractId",
                table: "Estimates");

            migrationBuilder.DropColumn(
                name: "DiscountMaterials",
                table: "Estimates");

            migrationBuilder.DropColumn(
                name: "EstimateName",
                table: "Estimates");

            migrationBuilder.DropColumn(
                name: "FacilityId",
                table: "Estimates");

            migrationBuilder.DropColumn(
                name: "CustomRate",
                table: "EstimateItems");

            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "Estimates",
                newName: "Date");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "TarifficatorItems",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,5)",
                oldPrecision: 18,
                oldScale: 5);

            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "Estimates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CustomerName",
                table: "Estimates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CurrencyRate",
                table: "Estimates",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "LocationName",
                table: "Estimates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "Qty",
                table: "EstimateItems",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}
