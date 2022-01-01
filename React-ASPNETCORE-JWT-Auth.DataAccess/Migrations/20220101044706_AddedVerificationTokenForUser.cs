using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReactASPNETCOREJWTAuth.DataAccess.Migrations
{
    public partial class AddedVerificationTokenForUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VerificationToken",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VerificationToken",
                table: "Users");
        }
    }
}
