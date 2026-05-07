using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PianoLearningTracker.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Pieces",
                columns: new[] { "Id", "Composer", "Description", "Difficulty", "DurationMinutes", "Genre", "Title", "YearComposed" },
                values: new object[,]
                {
                    { 1, "Ludwig van Beethoven", "Jedna od najpoznatijih klavirskih minijatura, izvrsna za učenike srednje razine.", 1, 3, "Klasika", "Für Elise", 1810 },
                    { 2, "Frédéric Chopin", "Lirska skladba koja zahtijeva izražajnu dinamiku i kontrolu tona.", 3, 5, "Romantizam", "Nocturne Op. 9 No. 2", 1832 },
                    { 3, "Johann Sebastian Bach", "Idealna skladba za početnike, jasna melodijska linija i jednostavan ritam.", 0, 2, "Barok", "Minuet in G Major", 1725 }
                });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "DateOfBirth", "Email", "EnrollmentDate", "FirstName", "Grade", "LastName", "Notes", "PhoneNumber" },
                values: new object[,]
                {
                    { 1, new DateTime(2012, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "luka.babic@email.com", new DateTime(2022, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Luka", 3, "Babić", "Brzo napreduje, posebno zainteresiran za klasiku.", "098-123-4567" },
                    { 2, new DateTime(2010, 11, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "petra.novak@email.com", new DateTime(2020, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Petra", 5, "Novak", "Odlična tehnika, radi na izražajnosti i dinamici.", "099-234-5678" },
                    { 3, new DateTime(2015, 7, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "tomislav.juric@email.com", new DateTime(2024, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tomislav", 1, "Jurić", "Početnik, još usvaja osnove notnog čitanja.", "091-345-6789" }
                });

            migrationBuilder.InsertData(
                table: "Teachers",
                columns: new[] { "Id", "Biography", "Email", "FirstName", "HireDate", "LastName", "PhoneNumber", "Specialization", "YearsOfExperience" },
                values: new object[,]
                {
                    { 1, "Diplomirala na Muzičkoj akademiji u Zagrebu, specijalist za Bacha i Chopina.", "marija.horvat@glazbena.hr", "Marija", new DateTime(2015, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Horvat", "091-111-2233", "Klasična glazba", 12 },
                    { 2, "Aktivni jazz glazbenik i pedagog s iskustvom u radu s učenicima svih uzrasta.", "ivan.kovac@glazbena.hr", "Ivan", new DateTime(2019, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kovač", "092-444-5566", "Jazz i improvizacija", 8 },
                    { 3, "Magistrica glazbene pedagogije, fokus na suvremenim tehnikama sviranja.", "ana.petric@glazbena.hr", "Ana", new DateTime(2022, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Petrić", "095-777-8899", "Suvremena glazba", 5 }
                });

            migrationBuilder.InsertData(
                table: "Lessons",
                columns: new[] { "Id", "DurationMinutes", "HomeworkAssigned", "Notes", "ScheduledDate", "Status", "StudentId", "TeacherId" },
                values: new object[,]
                {
                    { 1, 45, "Vježbati taktove 1-20 svaki dan po 15 minuta.", "Luka je dobro svladao prve dvije stranice Für Elise.", new DateTime(2025, 3, 10, 16, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, 1 },
                    { 2, 60, "Fokusirati se na desnu ruku, pp dinamiku u središnjem dijelu.", "Petra radi na rubatu i dinamičkim nijansama Nocturna.", new DateTime(2025, 3, 12, 17, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 1 },
                    { 3, 30, "Svirati ljestvicu C-dur obje ruke, 5 minuta dnevno.", "Tomislav uči pravilno držanje ruku i poziciju za Minuet.", new DateTime(2025, 3, 14, 15, 0, 0, 0, DateTimeKind.Unspecified), 1, 3, 3 },
                    { 4, 45, "", "", new DateTime(2025, 3, 17, 16, 0, 0, 0, DateTimeKind.Unspecified), 0, 1, 1 }
                });

            migrationBuilder.InsertData(
                table: "PracticeSessions",
                columns: new[] { "Id", "Date", "DurationMinutes", "Goals", "Notes", "PieceId", "QualityRating", "StudentId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 3, 11, 17, 0, 0, 0, DateTimeKind.Unspecified), 20, "Svladati lijevu ruku taktova 1-20.", "Fokus na lijevoj ruci, taktovi 1-10.", 1, 4, 1 },
                    { 2, new DateTime(2025, 3, 13, 18, 30, 0, 0, DateTimeKind.Unspecified), 30, "Postići kontinuitet bez zaustavljanja.", "Radila cijelu skladbu sporim tempom.", 2, 5, 2 },
                    { 3, new DateTime(2025, 3, 15, 16, 0, 0, 0, DateTimeKind.Unspecified), 15, "Zapamtiti noty prvih 8 taktova napamet.", "Vježbao ljestvice i prvih 8 taktova Minueta.", 3, 3, 3 }
                });

            migrationBuilder.InsertData(
                table: "StudentPieces",
                columns: new[] { "PieceId", "StudentId", "CompletionDate", "IsCompleted", "Notes", "ProgressRating", "StartDate" },
                values: new object[,]
                {
                    { 1, 1, null, false, "Svladao prvu stranicu, radi na drugoj.", 6, new DateTime(2025, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 1, 2, new DateTime(2024, 12, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Završeno, izvedeno na godišnjem koncertu.", 10, new DateTime(2024, 10, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, 2, null, false, "Tehnički dobro, dorađuje izražajnost.", 8, new DateTime(2025, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, 3, null, false, "Početak rada na skladbi.", 3, new DateTime(2025, 3, 14, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "LessonPieces",
                columns: new[] { "LessonId", "PieceId", "Notes" },
                values: new object[,]
                {
                    { 1, 1, "Obrađeni uvodnim taktovi." },
                    { 2, 2, "Rad na dinamici cijele skladbe." },
                    { 3, 3, "Usvajanje prvih 8 taktova." }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "LessonPieces",
                keyColumns: new[] { "LessonId", "PieceId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "LessonPieces",
                keyColumns: new[] { "LessonId", "PieceId" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "LessonPieces",
                keyColumns: new[] { "LessonId", "PieceId" },
                keyValues: new object[] { 3, 3 });

            migrationBuilder.DeleteData(
                table: "Lessons",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "PracticeSessions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "PracticeSessions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "PracticeSessions",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "StudentPieces",
                keyColumns: new[] { "PieceId", "StudentId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "StudentPieces",
                keyColumns: new[] { "PieceId", "StudentId" },
                keyValues: new object[] { 1, 2 });

            migrationBuilder.DeleteData(
                table: "StudentPieces",
                keyColumns: new[] { "PieceId", "StudentId" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "StudentPieces",
                keyColumns: new[] { "PieceId", "StudentId" },
                keyValues: new object[] { 3, 3 });

            migrationBuilder.DeleteData(
                table: "Teachers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Lessons",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Lessons",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Lessons",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Pieces",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Pieces",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Pieces",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Teachers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Teachers",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
