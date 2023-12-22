using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_todo_lisk.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class MigrationUpdateComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CommentUserId",
                table: "Comments",
                newName: "CommentTitle");

            migrationBuilder.AddColumn<DateTime>(
                name: "CommentDateUpdate",
                table: "Comments",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommentDateUpdate",
                table: "Comments");

            migrationBuilder.RenameColumn(
                name: "CommentTitle",
                table: "Comments",
                newName: "CommentUserId");
        }
    }
}
