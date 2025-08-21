using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init_StajContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BelgeTipleri",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BelgeTipleri", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "StajTurleri",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StajTurleri", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Stajyerler",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Universite = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OgrenciNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bolum = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fakulte = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BaslamaYili = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Sinif = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PAU_ogrencisi_mi = table.Column<bool>(type: "bit", nullable: false),
                    Ad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Soyad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TCKimlikNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DogumTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Cinsiyet = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TelNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Adres = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stajyerler", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Basvurular",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Soyad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TCKimlikNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DogumTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Cinsiyet = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TelNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Adres = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Universite = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fakulte = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bolum = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sinif = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BaslamaYili = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OgrenciNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Departman = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SorumluID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Yetkiler = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BaslamaTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BitisTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StajTuruID = table.Column<int>(type: "int", nullable: false),
                    BasvuruTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    RedNedeni = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RedTarihi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Basvurular", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Basvurular_StajTurleri_StajTuruID",
                        column: x => x.StajTuruID,
                        principalTable: "StajTurleri",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Stajlar",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Departman = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SorumluID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BaslamaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BitisTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Yetkiler = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StajyerID = table.Column<int>(type: "int", nullable: false),
                    StajTuruID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stajlar", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Stajlar_StajTurleri_StajTuruID",
                        column: x => x.StajTuruID,
                        principalTable: "StajTurleri",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Stajlar_Stajyerler_StajyerID",
                        column: x => x.StajyerID,
                        principalTable: "Stajyerler",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BasvuruBelgeleri",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BelgeAdı = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Açıklama = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Yolu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BelgeTipiID = table.Column<int>(type: "int", nullable: false),
                    BasvuruID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasvuruBelgeleri", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BasvuruBelgeleri_Basvurular_BasvuruID",
                        column: x => x.BasvuruID,
                        principalTable: "Basvurular",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BasvuruBelgeleri_BelgeTipleri_BelgeTipiID",
                        column: x => x.BelgeTipiID,
                        principalTable: "BelgeTipleri",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Belgeler",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BelgeAdı = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Açıklama = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Yolu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BelgeTipiID = table.Column<int>(type: "int", nullable: false),
                    StajID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Belgeler", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Belgeler_BelgeTipleri_BelgeTipiID",
                        column: x => x.BelgeTipiID,
                        principalTable: "BelgeTipleri",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Belgeler_Stajlar_StajID",
                        column: x => x.StajID,
                        principalTable: "Stajlar",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "BelgeTipleri",
                columns: new[] { "ID", "Ad" },
                values: new object[,]
                {
                    { 1, "Öğrenci Belgesi" },
                    { 2, "Transkript" },
                    { 3, "Staj Başvuru Formu" },
                    { 4, "Bilgi Taahütnamesi" },
                    { 5, "Referans Mektubu" },
                    { 6, "Staj Onay Raporu" },
                    { 7, "Detaylı Staj Durum Raporu" },
                    { 8, "Verilen_Referans Mektubu" }
                });

            migrationBuilder.InsertData(
                table: "StajTurleri",
                columns: new[] { "ID", "Ad" },
                values: new object[,]
                {
                    { 1, "Zorunlu" },
                    { 2, "İŞKUR" },
                    { 3, "Kısmi Zamanlı" },
                    { 4, "İşletmede Mesleki Eğitim" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BasvuruBelgeleri_BasvuruID",
                table: "BasvuruBelgeleri",
                column: "BasvuruID");

            migrationBuilder.CreateIndex(
                name: "IX_BasvuruBelgeleri_BelgeTipiID",
                table: "BasvuruBelgeleri",
                column: "BelgeTipiID");

            migrationBuilder.CreateIndex(
                name: "IX_Basvurular_StajTuruID",
                table: "Basvurular",
                column: "StajTuruID");

            migrationBuilder.CreateIndex(
                name: "IX_Belgeler_BelgeTipiID",
                table: "Belgeler",
                column: "BelgeTipiID");

            migrationBuilder.CreateIndex(
                name: "IX_Belgeler_StajID",
                table: "Belgeler",
                column: "StajID");

            migrationBuilder.CreateIndex(
                name: "IX_Stajlar_StajTuruID",
                table: "Stajlar",
                column: "StajTuruID");

            migrationBuilder.CreateIndex(
                name: "IX_Stajlar_StajyerID",
                table: "Stajlar",
                column: "StajyerID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BasvuruBelgeleri");

            migrationBuilder.DropTable(
                name: "Belgeler");

            migrationBuilder.DropTable(
                name: "Basvurular");

            migrationBuilder.DropTable(
                name: "BelgeTipleri");

            migrationBuilder.DropTable(
                name: "Stajlar");

            migrationBuilder.DropTable(
                name: "StajTurleri");

            migrationBuilder.DropTable(
                name: "Stajyerler");
        }
    }
}
