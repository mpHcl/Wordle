using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class AddDailyChallangeGameLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DailyChallangeId",
                table: "Games",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Games_DailyChallangeId",
                table: "Games",
                column: "DailyChallangeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_DailyChallenges_DailyChallangeId",
                table: "Games",
                column: "DailyChallangeId",
                principalTable: "DailyChallenges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_DailyChallenges_DailyChallangeId",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_Games_DailyChallangeId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "DailyChallangeId",
                table: "Games");
        }
    }
}
