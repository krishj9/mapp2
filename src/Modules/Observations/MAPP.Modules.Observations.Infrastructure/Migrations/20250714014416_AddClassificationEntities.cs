using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MAPP.Modules.Observations.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddClassificationEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ObservationData");

            migrationBuilder.DropIndex(
                name: "IX_Observations_Location",
                table: "Observations");

            migrationBuilder.DropIndex(
                name: "IX_Observations_ObservedAt",
                table: "Observations");

            migrationBuilder.DropIndex(
                name: "IX_Observations_ObserverId",
                table: "Observations");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Observations");

            migrationBuilder.DropColumn(
                name: "ObservedAt",
                table: "Observations");

            migrationBuilder.DropColumn(
                name: "ObserverId",
                table: "Observations");

            migrationBuilder.DropColumn(
                name: "PriorityName",
                table: "Observations");

            migrationBuilder.EnsureSchema(
                name: "observations");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Observations",
                newName: "TeacherName");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Observations",
                newName: "DomainId");

            migrationBuilder.RenameColumn(
                name: "Priority",
                table: "Observations",
                newName: "AttributeId");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Observations",
                newName: "LearningContext");

            migrationBuilder.RenameIndex(
                name: "IX_Observations_Status",
                table: "Observations",
                newName: "IX_Observations_DomainId");

            migrationBuilder.AddColumn<string>(
                name: "AttributeName",
                table: "Observations",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "ChildId",
                table: "Observations",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "ChildName",
                table: "Observations",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DomainName",
                table: "Observations",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsDraft",
                table: "Observations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ObservationDate",
                table: "Observations",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ObservationText",
                table: "Observations",
                type: "character varying(5000)",
                maxLength: 5000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProgressionPointIds",
                table: "Observations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "TeacherId",
                table: "Observations",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "ObservationArtifacts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ObservationId = table.Column<int>(type: "integer", nullable: false),
                    OriginalFileName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    StoredFileName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    BucketName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    StoragePath = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    StoragePathNormalized = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    PublicUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ContentType = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    FileSizeFormatted = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MediaType = table.Column<int>(type: "integer", nullable: false),
                    Caption = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsUploaded = table.Column<bool>(type: "boolean", nullable: false),
                    UploadError = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    UploadedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UploadedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Metadata = table.Column<string>(type: "jsonb", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObservationArtifacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObservationArtifacts_Observations_ObservationId",
                        column: x => x.ObservationId,
                        principalTable: "Observations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObservationDomains",
                schema: "observations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CategoryName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CategoryTitle = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObservationDomains", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tag",
                columns: table => new
                {
                    ObservationId = table.Column<int>(type: "integer", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Value = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NormalizedValue = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tag", x => new { x.ObservationId, x.Id });
                    table.ForeignKey(
                        name: "FK_Tag_Observations_ObservationId",
                        column: x => x.ObservationId,
                        principalTable: "Observations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObservationAttributes",
                schema: "observations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Number = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CategoryInformation = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    DomainId = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObservationAttributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObservationAttributes_ObservationDomains_DomainId",
                        column: x => x.DomainId,
                        principalSchema: "observations",
                        principalTable: "ObservationDomains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProgressionPoints",
                schema: "observations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Points = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Order = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    CategoryInformation = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    AttributeId = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgressionPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgressionPoints_ObservationAttributes_AttributeId",
                        column: x => x.AttributeId,
                        principalSchema: "observations",
                        principalTable: "ObservationAttributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Observations_AttributeId",
                table: "Observations",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_Observations_ChildId",
                table: "Observations",
                column: "ChildId");

            migrationBuilder.CreateIndex(
                name: "IX_Observations_IsDraft",
                table: "Observations",
                column: "IsDraft");

            migrationBuilder.CreateIndex(
                name: "IX_Observations_ObservationDate",
                table: "Observations",
                column: "ObservationDate");

            migrationBuilder.CreateIndex(
                name: "IX_Observations_TeacherId",
                table: "Observations",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_ObservationArtifacts_Created",
                table: "ObservationArtifacts",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_ObservationArtifacts_DisplayOrder",
                table: "ObservationArtifacts",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_ObservationArtifacts_IsUploaded",
                table: "ObservationArtifacts",
                column: "IsUploaded");

            migrationBuilder.CreateIndex(
                name: "IX_ObservationArtifacts_MediaType",
                table: "ObservationArtifacts",
                column: "MediaType");

            migrationBuilder.CreateIndex(
                name: "IX_ObservationArtifacts_ObservationId",
                table: "ObservationArtifacts",
                column: "ObservationId");

            migrationBuilder.CreateIndex(
                name: "IX_ObservationAttributes_DomainId",
                schema: "observations",
                table: "ObservationAttributes",
                column: "DomainId");

            migrationBuilder.CreateIndex(
                name: "IX_ObservationAttributes_DomainId_Number",
                schema: "observations",
                table: "ObservationAttributes",
                columns: new[] { "DomainId", "Number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObservationAttributes_IsActive",
                schema: "observations",
                table: "ObservationAttributes",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ObservationAttributes_Number",
                schema: "observations",
                table: "ObservationAttributes",
                column: "Number");

            migrationBuilder.CreateIndex(
                name: "IX_ObservationAttributes_SortOrder",
                schema: "observations",
                table: "ObservationAttributes",
                column: "SortOrder");

            migrationBuilder.CreateIndex(
                name: "IX_ObservationDomains_IsActive",
                schema: "observations",
                table: "ObservationDomains",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ObservationDomains_Name",
                schema: "observations",
                table: "ObservationDomains",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ObservationDomains_SortOrder",
                schema: "observations",
                table: "ObservationDomains",
                column: "SortOrder");

            migrationBuilder.CreateIndex(
                name: "IX_ProgressionPoints_AttributeId",
                schema: "observations",
                table: "ProgressionPoints",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgressionPoints_AttributeId_SortOrder",
                schema: "observations",
                table: "ProgressionPoints",
                columns: new[] { "AttributeId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_ProgressionPoints_Created",
                schema: "observations",
                table: "ProgressionPoints",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_ProgressionPoints_IsActive",
                schema: "observations",
                table: "ProgressionPoints",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ProgressionPoints_Points",
                schema: "observations",
                table: "ProgressionPoints",
                column: "Points");

            migrationBuilder.CreateIndex(
                name: "IX_ProgressionPoints_SortOrder",
                schema: "observations",
                table: "ProgressionPoints",
                column: "SortOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Tag_NormalizedValue",
                table: "Tag",
                column: "NormalizedValue");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ObservationArtifacts");

            migrationBuilder.DropTable(
                name: "ProgressionPoints",
                schema: "observations");

            migrationBuilder.DropTable(
                name: "Tag");

            migrationBuilder.DropTable(
                name: "ObservationAttributes",
                schema: "observations");

            migrationBuilder.DropTable(
                name: "ObservationDomains",
                schema: "observations");

            migrationBuilder.DropIndex(
                name: "IX_Observations_AttributeId",
                table: "Observations");

            migrationBuilder.DropIndex(
                name: "IX_Observations_ChildId",
                table: "Observations");

            migrationBuilder.DropIndex(
                name: "IX_Observations_IsDraft",
                table: "Observations");

            migrationBuilder.DropIndex(
                name: "IX_Observations_ObservationDate",
                table: "Observations");

            migrationBuilder.DropIndex(
                name: "IX_Observations_TeacherId",
                table: "Observations");

            migrationBuilder.DropColumn(
                name: "AttributeName",
                table: "Observations");

            migrationBuilder.DropColumn(
                name: "ChildId",
                table: "Observations");

            migrationBuilder.DropColumn(
                name: "ChildName",
                table: "Observations");

            migrationBuilder.DropColumn(
                name: "DomainName",
                table: "Observations");

            migrationBuilder.DropColumn(
                name: "IsDraft",
                table: "Observations");

            migrationBuilder.DropColumn(
                name: "ObservationDate",
                table: "Observations");

            migrationBuilder.DropColumn(
                name: "ObservationText",
                table: "Observations");

            migrationBuilder.DropColumn(
                name: "ProgressionPointIds",
                table: "Observations");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "Observations");

            migrationBuilder.RenameColumn(
                name: "TeacherName",
                table: "Observations",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "LearningContext",
                table: "Observations",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "DomainId",
                table: "Observations",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "AttributeId",
                table: "Observations",
                newName: "Priority");

            migrationBuilder.RenameIndex(
                name: "IX_Observations_DomainId",
                table: "Observations",
                newName: "IX_Observations_Status");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Observations",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ObservedAt",
                table: "Observations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ObserverId",
                table: "Observations",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PriorityName",
                table: "Observations",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ObservationData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ObservationId = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    Key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Value = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false)
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
        }
    }
}
