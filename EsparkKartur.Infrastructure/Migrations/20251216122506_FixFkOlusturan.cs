using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EsparkKartur.Infrastructure.Migrations
{
	/// <inheritdoc />
	public partial class FixFkOlusturan : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			// ❌ KALDIRILDI / YORUMA ALINDI: Bu sütun zaten veritabanında var olduğu için 'duplicate column name' hatasına neden oluyordu.
			/*
            migrationBuilder.AddColumn<int>(
                name: "OlusturanID",
                table: "SevkFisleri",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
            */

			// ✅ YON SÜTUNU EKLENİYOR (İlk kez ekleniyorsa bırakın)
			migrationBuilder.AddColumn<string>(
				name: "Yon",
				table: "SevkFisleri",
				type: "TEXT",
				nullable: false,
				defaultValue: "");

			// ✅ INDEX VE FOREIGN KEY KISITLAMALARI EKLENİYOR
			migrationBuilder.CreateIndex(
				name: "IX_SevkFisleri_OlusturanID",
				table: "SevkFisleri",
				column: "OlusturanID");

			migrationBuilder.AddForeignKey(
				name: "FK_SevkFisleri_Kullanicilar_OlusturanID",
				table: "SevkFisleri",
				column: "OlusturanID",
				principalTable: "Kullanicilar",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_SevkFisleri_Kullanicilar_OlusturanID",
				table: "SevkFisleri");

			migrationBuilder.DropIndex(
				name: "IX_SevkFisleri_OlusturanID",
				table: "SevkFisleri");

			// ❌ KALDIRILDI / YORUMA ALINDI: Var olan sütunu silmeye çalışmak da hata verebilir.
			/*
            migrationBuilder.DropColumn(
                name: "OlusturanID",
                table: "SevkFisleri");
            */

			// ✅ YON SÜTUNU SİLİNİYOR (Eklendiyse silinmeli)
			migrationBuilder.DropColumn(
				name: "Yon",
				table: "SevkFisleri");
		}
	}
}