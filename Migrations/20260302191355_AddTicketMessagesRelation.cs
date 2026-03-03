using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpTrackAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketMessagesRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TicketMessages_TicketId",
                table: "TicketMessages",
                column: "TicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketMessages_ticket_TicketId",
                table: "TicketMessages",
                column: "TicketId",
                principalTable: "ticket",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketMessages_ticket_TicketId",
                table: "TicketMessages");

            migrationBuilder.DropIndex(
                name: "IX_TicketMessages_TicketId",
                table: "TicketMessages");
        }
    }
}
