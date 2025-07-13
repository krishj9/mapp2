using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MAPP.Modules.Observations.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Observations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    PriorityName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ObservedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ObserverId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    Location = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Observations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ObservationData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ObservationId = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObservationData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObservationData_Observations_ObservationId",
                        column: x => x.ObservationId,
                        principalTable: "Observations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ObservationData_Key",
                table: "ObservationData",
                column: "Key");

            migrationBuilder.CreateIndex(
                name: "IX_ObservationData_ObservationId",
                table: "ObservationData",
                column: "ObservationId");

            migrationBuilder.CreateIndex(
                name: "IX_ObservationData_ObservationId_Key",
                table: "ObservationData",
                columns: new[] { "ObservationId", "Key" });

            migrationBuilder.CreateIndex(
                name: "IX_Observations_Created",
                table: "Observations",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_Observations_Location",
                table: "Observations",
                column: "Location");

            migrationBuilder.CreateIndex(
                name: "IX_Observations_ObservedAt",
                table: "Observations",
                column: "ObservedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Observations_ObserverId",
                table: "Observations",
                column: "ObserverId");

            migrationBuilder.CreateIndex(
                name: "IX_Observations_Status",
                table: "Observations",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ObservationData");

            migrationBuilder.DropTable(
                name: "Observations");
        }
    }
}
