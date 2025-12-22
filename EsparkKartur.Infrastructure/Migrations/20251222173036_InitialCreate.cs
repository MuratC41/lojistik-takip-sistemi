using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EsparkKartur.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KargoFirmalari",
                columns: table => new
                {
                    KargoID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    KargoAdi = table.Column<string>(type: "TEXT", nullable: false),
                    Durum = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KargoFirmalari", x => x.KargoID);
                });

            migrationBuilder.CreateTable(
                name: "Kullanicilar",
                columns: table => new
                {
                    KullaniciID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AdSoyad = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    KullaniciAdi = table.Column<string>(type: "TEXT", nullable: false),
                    SifreHash = table.Column<string>(type: "TEXT", nullable: false),
                    SifreSalt = table.Column<string>(type: "TEXT", nullable: false),
                    Rol = table.Column<string>(type: "TEXT", nullable: false),
                    Durum = table.Column<bool>(type: "INTEGER", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kullanicilar", x => x.KullaniciID);
                });

            migrationBuilder.CreateTable(
                name: "Magazalar",
                columns: table => new
                {
                    MagazaID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MagazaAdi = table.Column<string>(type: "TEXT", nullable: false),
                    Durum = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Magazalar", x => x.MagazaID);
                });

            migrationBuilder.CreateTable(
                name: "AuditTrails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    KullaniciId = table.Column<int>(type: "INTEGER", nullable: false),
                    İşlemTürü = table.Column<string>(type: "TEXT", nullable: false),
                    TabloAdı = table.Column<string>(type: "TEXT", nullable: false),
                    KayıtId = table.Column<int>(type: "INTEGER", nullable: false),
                    EskiDeger = table.Column<string>(type: "TEXT", nullable: false),
                    YeniDeger = table.Column<string>(type: "TEXT", nullable: false),
                    İşlemTarihi = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditTrails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditTrails_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "KullaniciID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SevkFisleri",
                columns: table => new
                {
                    FisID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FisNumarasi = table.Column<string>(type: "TEXT", nullable: false),
                    MagazaID = table.Column<int>(type: "INTEGER", nullable: false),
                    OlusturanID = table.Column<int>(type: "INTEGER", nullable: false),
                    Yon = table.Column<int>(type: "INTEGER", nullable: false),
                    GonderimModu = table.Column<int>(type: "INTEGER", nullable: false),
                    Fiyat = table.Column<decimal>(type: "TEXT", nullable: false),
                    TeslimAlan = table.Column<string>(type: "TEXT", nullable: false),
                    Aciklama = table.Column<string>(type: "TEXT", nullable: true),
                    ImzaDosyasi = table.Column<string>(type: "TEXT", nullable: true),
                    TarihSaat = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Durum = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SevkFisleri", x => x.FisID);
                    table.ForeignKey(
                        name: "FK_SevkFisleri_Kullanicilar_OlusturanID",
                        column: x => x.OlusturanID,
                        principalTable: "Kullanicilar",
                        principalColumn: "KullaniciID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SevkFisleri_Magazalar_MagazaID",
                        column: x => x.MagazaID,
                        principalTable: "Magazalar",
                        principalColumn: "MagazaID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FisKargo",
                columns: table => new
                {
                    FisID = table.Column<int>(type: "INTEGER", nullable: false),
                    KargoID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FisKargo", x => new { x.FisID, x.KargoID });
                    table.ForeignKey(
                        name: "FK_FisKargo_KargoFirmalari_KargoID",
                        column: x => x.KargoID,
                        principalTable: "KargoFirmalari",
                        principalColumn: "KargoID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FisKargo_SevkFisleri_FisID",
                        column: x => x.FisID,
                        principalTable: "SevkFisleri",
                        principalColumn: "FisID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FisUrunleri",
                columns: table => new
                {
                    UrunID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FisID = table.Column<int>(type: "INTEGER", nullable: false),
                    KoliAdet = table.Column<int>(type: "INTEGER", nullable: false),
                    PaketAdet = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FisUrunleri", x => x.UrunID);
                    table.ForeignKey(
                        name: "FK_FisUrunleri_SevkFisleri_FisID",
                        column: x => x.FisID,
                        principalTable: "SevkFisleri",
                        principalColumn: "FisID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditTrails_KullaniciId",
                table: "AuditTrails",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_FisKargo_KargoID",
                table: "FisKargo",
                column: "KargoID");

            migrationBuilder.CreateIndex(
                name: "IX_FisUrunleri_FisID",
                table: "FisUrunleri",
                column: "FisID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SevkFisleri_FisNumarasi",
                table: "SevkFisleri",
                column: "FisNumarasi",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SevkFisleri_MagazaID",
                table: "SevkFisleri",
                column: "MagazaID");

            migrationBuilder.CreateIndex(
                name: "IX_SevkFisleri_OlusturanID",
                table: "SevkFisleri",
                column: "OlusturanID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditTrails");

            migrationBuilder.DropTable(
                name: "FisKargo");

            migrationBuilder.DropTable(
                name: "FisUrunleri");

            migrationBuilder.DropTable(
                name: "KargoFirmalari");

            migrationBuilder.DropTable(
                name: "SevkFisleri");

            migrationBuilder.DropTable(
                name: "Kullanicilar");

            migrationBuilder.DropTable(
                name: "Magazalar");
        }
    }
}
