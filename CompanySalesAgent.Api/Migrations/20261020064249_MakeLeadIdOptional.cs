using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompanySalesAgent.Api.Migrations
{
    /// <inheritdoc />
    public partial class MakeLeadIdOptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Interactions_Leads_LeadId",
                table: "Interactions");

            migrationBuilder.AlterColumn<int>(
                name: "LeadId",
                table: "Interactions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Interactions_Leads_LeadId",
                table: "Interactions",
                column: "LeadId",
                principalTable: "Leads",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Interactions_Leads_LeadId",
                table: "Interactions");

            migrationBuilder.AlterColumn<int>(
                name: "LeadId",
                table: "Interactions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Interactions_Leads_LeadId",
                table: "Interactions",
                column: "LeadId",
                principalTable: "Leads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
