using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pharmacy.Infarstructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableUserTokeninColumId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1️⃣ أضف عمود جديد مؤقت بخصائص التوليد التلقائي
            migrationBuilder.AddColumn<Guid>(
                name: "TempId",
                table: "UserTokens",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWSEQUENTIALID()");

            // 2️⃣ انسخ البيانات من العمود القديم (إن وجدت)
            migrationBuilder.Sql("UPDATE UserTokens SET TempId = NEWID()");

            // 3️⃣ احذف الـ Primary Key الحالي
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserTokens",
                table: "UserTokens");

            // 4️⃣ احذف العمود القديم Id
            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserTokens");

            // 5️⃣ أعد تسمية العمود الجديد إلى Id
            migrationBuilder.RenameColumn(
                name: "TempId",
                table: "UserTokens",
                newName: "Id");

            // 6️⃣ أضف Primary Key جديد
            migrationBuilder.AddPrimaryKey(
                name: "PK_UserTokens",
                table: "UserTokens",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // عكس الخطوات لو رجعت المايجريشن
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserTokens",
                table: "UserTokens");

            migrationBuilder.AddColumn<Guid>(
                name: "OldId",
                table: "UserTokens",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: Guid.NewGuid());

            migrationBuilder.Sql("UPDATE UserTokens SET OldId = NEWID()");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserTokens");

            migrationBuilder.RenameColumn(
                name: "OldId",
                table: "UserTokens",
                newName: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserTokens",
                table: "UserTokens",
                column: "Id");
        }
    }
}
