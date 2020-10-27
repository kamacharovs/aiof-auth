using Microsoft.EntityFrameworkCore.Migrations;

namespace aiof.auth.data.Migrations
{
    public partial class update_token : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_client_refresh_token_client_client_id",
                table: "client_refresh_token");

            migrationBuilder.AlterColumn<string>(
                name: "token",
                table: "user_refresh_token",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "user",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "token",
                table: "client_refresh_token",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddForeignKey(
                name: "FK_client_refresh_token_client_client_id",
                table: "client_refresh_token",
                column: "client_id",
                principalTable: "client",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_client_refresh_token_client_client_id",
                table: "client_refresh_token");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "user");

            migrationBuilder.AlterColumn<string>(
                name: "token",
                table: "user_refresh_token",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "token",
                table: "client_refresh_token",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 200);

            migrationBuilder.AddForeignKey(
                name: "FK_client_refresh_token_client_client_id",
                table: "client_refresh_token",
                column: "client_id",
                principalTable: "client",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
