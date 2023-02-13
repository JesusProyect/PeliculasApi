using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace _2_PeliculasAPI.Migrations
{
    public partial class SalaDeCine_Data : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Ubicaion",
                table: "SalaDeCine",
                newName: "Ubicacion");

            migrationBuilder.InsertData(
                table: "SalaDeCine",
                columns: new[] { "Id", "Nombre", "Ubicacion" },
                values: new object[,]
                {
                    { 1, "Agora", (NetTopologySuite.Geometries.Point)new NetTopologySuite.IO.WKTReader().Read("SRID=4326;POINT (-69.9388777 18.4839233)") },
                    { 4, "Sambil", (NetTopologySuite.Geometries.Point)new NetTopologySuite.IO.WKTReader().Read("SRID=4326;POINT (-69.9118804 18.4826214)") },
                    { 5, "Megacentro", (NetTopologySuite.Geometries.Point)new NetTopologySuite.IO.WKTReader().Read("SRID=4326;POINT (-69.856427 18.506934)") },
                    { 6, "Village East Cinema", (NetTopologySuite.Geometries.Point)new NetTopologySuite.IO.WKTReader().Read("SRID=4326;POINT (-73.986227 40.730898)") }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SalaDeCine",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "SalaDeCine",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "SalaDeCine",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "SalaDeCine",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.RenameColumn(
                name: "Ubicacion",
                table: "SalaDeCine",
                newName: "Ubicaion");
        }
    }
}
