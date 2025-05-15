using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinaTech.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class rename_money_entity_field_amount_to_value : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Amount_Amount",
                table: "Payments",
                newName: "Amount_Value");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Amount_Value",
                table: "Payments",
                newName: "Amount_Amount");
        }
    }
}
