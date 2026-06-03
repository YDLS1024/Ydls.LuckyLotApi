using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ydls.LuckyLotApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateExperts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "AppExperts");

            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "AppExperts");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "AppExperts");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "AppExperts");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "AppExperts");

            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                table: "AppExperts");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AppExperts");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "AppExperts");

            migrationBuilder.DropColumn(
                name: "LastModifierId",
                table: "AppExperts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "AppExperts",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "AppExperts",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "AppExperts",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                table: "AppExperts",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "AppExperts",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExtraProperties",
                table: "AppExperts",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AppExperts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "AppExperts",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifierId",
                table: "AppExperts",
                type: "uuid",
                nullable: true);
        }
    }
}
