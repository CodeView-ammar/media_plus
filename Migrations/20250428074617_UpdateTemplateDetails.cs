using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaPlus.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTemplateDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ShowTemplateTempId",
                table: "Template_Details",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Template_Details_ShowTemplateTempId",
                table: "Template_Details",
                column: "ShowTemplateTempId");

            migrationBuilder.AddForeignKey(
                name: "FK_Template_Details_Show_Template_ShowTemplateTempId",
                table: "Template_Details",
                column: "ShowTemplateTempId",
                principalTable: "Show_Template",
                principalColumn: "temp_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Template_Details_Show_Template_ShowTemplateTempId",
                table: "Template_Details");

            migrationBuilder.DropIndex(
                name: "IX_Template_Details_ShowTemplateTempId",
                table: "Template_Details");

            migrationBuilder.DropColumn(
                name: "ShowTemplateTempId",
                table: "Template_Details");
        }
    }
}
