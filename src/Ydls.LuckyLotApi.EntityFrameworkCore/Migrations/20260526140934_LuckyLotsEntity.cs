using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ydls.LuckyLotApi.Migrations
{
    /// <inheritdoc />
    public partial class LuckyLotsEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppExperts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nickname = table.Column<string>(type: "text", nullable: false),
                    WinningRate = table.Column<double>(type: "double precision", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppExperts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppNumberThree",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OpenDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    One = table.Column<short>(type: "smallint", nullable: false),
                    Two = table.Column<short>(type: "smallint", nullable: false),
                    Three = table.Column<short>(type: "smallint", nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppNumberThree", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppKillNumbers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    KillDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    KillNumber = table.Column<short[]>(type: "smallint[]", nullable: false),
                    IsTrue = table.Column<bool>(type: "boolean", nullable: true),
                    ExpertId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppKillNumbers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppKillNumbers_AppExperts_ExpertId",
                        column: x => x.ExpertId,
                        principalTable: "AppExperts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppKillNumbers_ExpertId",
                table: "AppKillNumbers",
                column: "ExpertId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppKillNumbers");

            migrationBuilder.DropTable(
                name: "AppNumberThree");

            migrationBuilder.DropTable(
                name: "AppExperts");
        }
    }
}
