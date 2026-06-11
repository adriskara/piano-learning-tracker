using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PianoLearningTracker.Models;

namespace PianoLearningTracker.DAL
{
    public class PianoLearningTrackerDbContext : IdentityDbContext<ApplicationUser>
    {
        public PianoLearningTrackerDbContext(DbContextOptions<PianoLearningTrackerDbContext> options)
            : base(options)
        { }

        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Piece> Pieces { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<PracticeSession> PracticeSessions { get; set; }
        public DbSet<LessonPiece> LessonPieces { get; set; }
        public DbSet<StudentPiece> StudentPieces { get; set; }
        public DbSet<PieceAttachment> PieceAttachments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Složeni primarni ključ za bridge tablicu LessonPiece (Lesson ↔ Piece)
            modelBuilder.Entity<LessonPiece>()
                .HasKey(lp => new { lp.LessonId, lp.PieceId });

            // Složeni primarni ključ za bridge tablicu StudentPiece (Student ↔ Piece)
            modelBuilder.Entity<StudentPiece>()
                .HasKey(sp => new { sp.StudentId, sp.PieceId });

            // Student.TeacherId → Teacher (bez kaskadnog brisanja da bi se izbjegao ciklus)
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Teacher)
                .WithMany(t => t.Students)
                .HasForeignKey(s => s.TeacherId)
                .OnDelete(DeleteBehavior.SetNull);

            // Seed podaci — nastavnici
            modelBuilder.Entity<Teacher>().HasData(
                new Teacher { Id = 1, FirstName = "Marija", LastName = "Horvat", Email = "marija.horvat@glazbena.hr", PhoneNumber = "091-111-2233", Specialization = "Klasična glazba", YearsOfExperience = 12, HireDate = new DateTime(2015, 9, 1), Biography = "Diplomirala na Muzičkoj akademiji u Zagrebu, specijalist za Bacha i Chopina." },
                new Teacher { Id = 2, FirstName = "Ivan", LastName = "Kovač", Email = "ivan.kovac@glazbena.hr", PhoneNumber = "092-444-5566", Specialization = "Jazz i improvizacija", YearsOfExperience = 8, HireDate = new DateTime(2019, 2, 1), Biography = "Aktivni jazz glazbenik i pedagog s iskustvom u radu s učenicima svih uzrasta." },
                new Teacher { Id = 3, FirstName = "Ana", LastName = "Petrić", Email = "ana.petric@glazbena.hr", PhoneNumber = "095-777-8899", Specialization = "Suvremena glazba", YearsOfExperience = 5, HireDate = new DateTime(2022, 10, 1), Biography = "Magistrica glazbene pedagogije, fokus na suvremenim tehnikama sviranja." }
            );

            // Seed podaci — skladbe
            modelBuilder.Entity<Piece>().HasData(
                new Piece { Id = 1, Title = "Für Elise", Composer = "Ludwig van Beethoven", Difficulty = DifficultyLevel.Elementary, Genre = "Klasika", DurationMinutes = 3, YearComposed = 1810, Description = "Jedna od najpoznatijih klavirskih minijatura, izvrsna za učenike srednje razine." },
                new Piece { Id = 2, Title = "Nocturne Op. 9 No. 2", Composer = "Frédéric Chopin", Difficulty = DifficultyLevel.Advanced, Genre = "Romantizam", DurationMinutes = 5, YearComposed = 1832, Description = "Lirska skladba koja zahtijeva izražajnu dinamiku i kontrolu tona." },
                new Piece { Id = 3, Title = "Minuet in G Major", Composer = "Johann Sebastian Bach", Difficulty = DifficultyLevel.Beginner, Genre = "Barok", DurationMinutes = 2, YearComposed = 1725, Description = "Idealna skladba za početnike, jasna melodijska linija i jednostavan ritam." }
            );

            // Seed podaci — učenici (TeacherId = primarni profesor)
            modelBuilder.Entity<Student>().HasData(
                new Student { Id = 1, FirstName = "Luka", LastName = "Babić", DateOfBirth = new DateTime(2012, 4, 15), Email = "luka.babic@email.com", PhoneNumber = "098-123-4567", EnrollmentDate = new DateTime(2022, 9, 1), Grade = 3, Notes = "Brzo napreduje, posebno zainteresiran za klasiku.", TeacherId = 1 },
                new Student { Id = 2, FirstName = "Petra", LastName = "Novak", DateOfBirth = new DateTime(2010, 11, 3), Email = "petra.novak@email.com", PhoneNumber = "099-234-5678", EnrollmentDate = new DateTime(2020, 9, 1), Grade = 5, Notes = "Odlična tehnika, radi na izražajnosti i dinamici.", TeacherId = 1 },
                new Student { Id = 3, FirstName = "Tomislav", LastName = "Jurić", DateOfBirth = new DateTime(2015, 7, 22), Email = "tomislav.juric@email.com", PhoneNumber = "091-345-6789", EnrollmentDate = new DateTime(2024, 9, 1), Grade = 1, Notes = "Početnik, još usvaja osnove notnog čitanja.", TeacherId = 3 }
            );

            // Seed podaci — sati
            modelBuilder.Entity<Lesson>().HasData(
                new Lesson { Id = 1, ScheduledDate = new DateTime(2025, 3, 10, 16, 0, 0), DurationMinutes = 45, Status = LessonStatus.Completed, Notes = "Luka je dobro svladao prve dvije stranice Für Elise.", HomeworkAssigned = "Vježbati taktove 1-20 svaki dan po 15 minuta.", StudentId = 1, TeacherId = 1 },
                new Lesson { Id = 2, ScheduledDate = new DateTime(2025, 3, 12, 17, 0, 0), DurationMinutes = 60, Status = LessonStatus.Completed, Notes = "Petra radi na rubatu i dinamičkim nijansama Nocturna.", HomeworkAssigned = "Fokusirati se na desnu ruku, pp dinamiku u središnjem dijelu.", StudentId = 2, TeacherId = 1 },
                new Lesson { Id = 3, ScheduledDate = new DateTime(2025, 3, 14, 15, 0, 0), DurationMinutes = 30, Status = LessonStatus.Completed, Notes = "Tomislav uči pravilno držanje ruku i poziciju za Minuet.", HomeworkAssigned = "Svirati ljestvicu C-dur obje ruke, 5 minuta dnevno.", StudentId = 3, TeacherId = 3 },
                new Lesson { Id = 4, ScheduledDate = new DateTime(2025, 3, 17, 16, 0, 0), DurationMinutes = 45, Status = LessonStatus.Scheduled, Notes = "", HomeworkAssigned = "", StudentId = 1, TeacherId = 1 }
            );

            // Seed podaci — bridge LessonPiece
            modelBuilder.Entity<LessonPiece>().HasData(
                new LessonPiece { LessonId = 1, PieceId = 1, Notes = "Obrađeni uvodnim taktovi." },
                new LessonPiece { LessonId = 2, PieceId = 2, Notes = "Rad na dinamici cijele skladbe." },
                new LessonPiece { LessonId = 3, PieceId = 3, Notes = "Usvajanje prvih 8 taktova." }
            );

            // Seed podaci — bridge StudentPiece
            modelBuilder.Entity<StudentPiece>().HasData(
                new StudentPiece { StudentId = 1, PieceId = 1, StartDate = new DateTime(2025, 3, 10), IsCompleted = false, ProgressRating = 6, Notes = "Svladao prvu stranicu, radi na drugoj." },
                new StudentPiece { StudentId = 2, PieceId = 2, StartDate = new DateTime(2025, 1, 20), IsCompleted = false, ProgressRating = 8, Notes = "Tehnički dobro, dorađuje izražajnost." },
                new StudentPiece { StudentId = 2, PieceId = 1, StartDate = new DateTime(2024, 10, 5), IsCompleted = true, CompletionDate = new DateTime(2024, 12, 20), ProgressRating = 10, Notes = "Završeno, izvedeno na godišnjem koncertu." },
                new StudentPiece { StudentId = 3, PieceId = 3, StartDate = new DateTime(2025, 3, 14), IsCompleted = false, ProgressRating = 3, Notes = "Početak rada na skladbi." }
            );

            // Seed podaci — vježbe
            modelBuilder.Entity<PracticeSession>().HasData(
                new PracticeSession { Id = 1, Date = new DateTime(2025, 3, 11, 17, 0, 0), DurationMinutes = 20, Notes = "Fokus na lijevoj ruci, taktovi 1-10.", QualityRating = 4, Goals = "Svladati lijevu ruku taktova 1-20.", StudentId = 1, PieceId = 1 },
                new PracticeSession { Id = 2, Date = new DateTime(2025, 3, 13, 18, 30, 0), DurationMinutes = 30, Notes = "Radila cijelu skladbu sporim tempom.", QualityRating = 5, Goals = "Postići kontinuitet bez zaustavljanja.", StudentId = 2, PieceId = 2 },
                new PracticeSession { Id = 3, Date = new DateTime(2025, 3, 15, 16, 0, 0), DurationMinutes = 15, Notes = "Vježbao ljestvice i prvih 8 taktova Minueta.", QualityRating = 3, Goals = "Zapamtiti noty prvih 8 taktova napamet.", StudentId = 3, PieceId = 3 }
            );

            // Seed podaci — uloge
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "role-admin", Name = "Administrator", NormalizedName = "ADMINISTRATOR", ConcurrencyStamp = "f4b865db-2f4b-4a58-848f-034748ac80f5" },
                new IdentityRole { Id = "role-teacher", Name = "Teacher", NormalizedName = "TEACHER", ConcurrencyStamp = "6c9c1a97-1317-493a-95e3-f9a63f41ea80" },
                new IdentityRole { Id = "role-student", Name = "Student", NormalizedName = "STUDENT", ConcurrencyStamp = "2185bcdc-f3ae-4fe4-9bdb-ed36ba3d2fcc" }
            );
        }
    }
}
