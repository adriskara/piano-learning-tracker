using PianoLearningTracker.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


// -------------------------------------------------------
// Punjenje objekata podacima
// -------------------------------------------------------

// --- Nastavnici ---
var teacher1 = new Teacher
{
    Id = 1,
    FirstName = "Marija",
    LastName = "Horvat",
    Email = "marija.horvat@glazbena.hr",
    PhoneNumber = "091-111-2233",
    Specialization = "Klasična glazba",
    YearsOfExperience = 12,
    HireDate = new DateTime(2015, 9, 1),
    Biography = "Diplomirala na Muzičkoj akademiji u Zagrebu, specijalist za Bacha i Chopina."
};

var teacher2 = new Teacher
{
    Id = 2,
    FirstName = "Ivan",
    LastName = "Kovač",
    Email = "ivan.kovac@glazbena.hr",
    PhoneNumber = "092-444-5566",
    Specialization = "Jazz i improvizacija",
    YearsOfExperience = 8,
    HireDate = new DateTime(2019, 2, 1),
    Biography = "Aktivni jazz glazbenik i pedagog s iskustvom u radu s učenicima svih uzrasta."
};

var teacher3 = new Teacher
{
    Id = 3,
    FirstName = "Ana",
    LastName = "Petrić",
    Email = "ana.petric@glazbena.hr",
    PhoneNumber = "095-777-8899",
    Specialization = "Suvremena glazba",
    YearsOfExperience = 5,
    HireDate = new DateTime(2022, 10, 1),
    Biography = "Magistrica glazbene pedagogije, fokus na suvremenim tehnikama sviranja."
};

// --- Skladbe ---
var piece1 = new Piece
{
    Id = 1,
    Title = "Für Elise",
    Composer = "Ludwig van Beethoven",
    Difficulty = DifficultyLevel.Elementary,
    Genre = "Klasika",
    DurationMinutes = 3,
    YearComposed = 1810,
    Description = "Jedna od najpoznatijih klavirskih minijatura, izvrsna za učenike srednje razine."
};

var piece2 = new Piece
{
    Id = 2,
    Title = "Nocturne Op. 9 No. 2",
    Composer = "Frédéric Chopin",
    Difficulty = DifficultyLevel.Advanced,
    Genre = "Romantizam",
    DurationMinutes = 5,
    YearComposed = 1832,
    Description = "Lirska skladba koja zahtijeva izražajnu dinamiku i kontrolu tona."
};

var piece3 = new Piece
{
    Id = 3,
    Title = "Minuet in G Major",
    Composer = "Johann Sebastian Bach",
    Difficulty = DifficultyLevel.Beginner,
    Genre = "Barok",
    DurationMinutes = 2,
    YearComposed = 1725,
    Description = "Idealna skladba za početnike, jasna melodijska linija i jednostavan ritam."
};

// --- Učenici ---
var student1 = new Student
{
    Id = 1,
    FirstName = "Luka",
    LastName = "Babić",
    DateOfBirth = new DateTime(2012, 4, 15),
    Email = "luka.babic@email.com",
    PhoneNumber = "098-123-4567",
    EnrollmentDate = new DateTime(2022, 9, 1),
    Grade = 3,
    Notes = "Brzo napreduje, posebno zainteresiran za klasiku."
};

var student2 = new Student
{
    Id = 2,
    FirstName = "Petra",
    LastName = "Novak",
    DateOfBirth = new DateTime(2010, 11, 3),
    Email = "petra.novak@email.com",
    PhoneNumber = "099-234-5678",
    EnrollmentDate = new DateTime(2020, 9, 1),
    Grade = 5,
    Notes = "Odlična tehnika, radi na izražajnosti i dinamici."
};

var student3 = new Student
{
    Id = 3,
    FirstName = "Tomislav",
    LastName = "Jurić",
    DateOfBirth = new DateTime(2015, 7, 22),
    Email = "tomislav.juric@email.com",
    PhoneNumber = "091-345-6789",
    EnrollmentDate = new DateTime(2024, 9, 1),
    Grade = 1,
    Notes = "Početnik, još usvaja osnove notnog čitanja."
};

// --- Satovi ---
var lesson1 = new Lesson
{
    Id = 1,
    ScheduledDate = new DateTime(2025, 3, 10, 16, 0, 0),
    DurationMinutes = 45,
    Status = LessonStatus.Completed,
    Notes = "Luka je dobro svladao prve dvije stranice Für Elise.",
    HomeworkAssigned = "Vježbati taktove 1-20 svaki dan po 15 minuta.",
    StudentId = 1,
    TeacherId = 1,
    Student = student1,
    Teacher = teacher1
};

var lesson2 = new Lesson
{
    Id = 2,
    ScheduledDate = new DateTime(2025, 3, 12, 17, 0, 0),
    DurationMinutes = 60,
    Status = LessonStatus.Completed,
    Notes = "Petra radi na rubatu i dinamičkim nijansama Nocturna.",
    HomeworkAssigned = "Fokusirati se na desnu ruku, pp dinamiku u središnjem dijelu.",
    StudentId = 2,
    TeacherId = 1,
    Student = student2,
    Teacher = teacher1
};

var lesson3 = new Lesson
{
    Id = 3,
    ScheduledDate = new DateTime(2025, 3, 14, 15, 0, 0),
    DurationMinutes = 30,
    Status = LessonStatus.Completed,
    Notes = "Tomislav uči pravilno držanje ruku i poziciju za Minuet.",
    HomeworkAssigned = "Svirati ljestvicu C-dur obje ruke, 5 minuta dnevno.",
    StudentId = 3,
    TeacherId = 3,
    Student = student3,
    Teacher = teacher3
};

var lesson4 = new Lesson
{
    Id = 4,
    ScheduledDate = new DateTime(2025, 3, 17, 16, 0, 0),
    DurationMinutes = 45,
    Status = LessonStatus.Scheduled,
    Notes = "",
    HomeworkAssigned = "",
    StudentId = 1,
    TeacherId = 1,
    Student = student1,
    Teacher = teacher1
};

// --- N-N: Satovi ↔ Skladbe (LessonPiece) ---
var lessonPiece1 = new LessonPiece
{
    LessonId = 1, Lesson = lesson1,
    PieceId = 1,  Piece = piece1,
    Notes = "Obrađeni uvodnim taktovi."
};

var lessonPiece2 = new LessonPiece
{
    LessonId = 2, Lesson = lesson2,
    PieceId = 2,  Piece = piece2,
    Notes = "Rad na dinamici cijele skladbe."
};

var lessonPiece3 = new LessonPiece
{
    LessonId = 3, Lesson = lesson3,
    PieceId = 3,  Piece = piece3,
    Notes = "Usvajanje prvih 8 taktova."
};

lesson1.LessonPieces.Add(lessonPiece1);
lesson2.LessonPieces.Add(lessonPiece2);
lesson3.LessonPieces.Add(lessonPiece3);

// --- N-N: Učenici ↔ Skladbe (StudentPiece) ---
var studentPiece1 = new StudentPiece
{
    StudentId = 1, Student = student1,
    PieceId = 1,   Piece = piece1,
    StartDate = new DateTime(2025, 3, 10),
    IsCompleted = false,
    CompletionDate = null,
    ProgressRating = 6,
    Notes = "Svladao prvu stranicu, radi na drugoj."
};

var studentPiece2 = new StudentPiece
{
    StudentId = 2, Student = student2,
    PieceId = 2,   Piece = piece2,
    StartDate = new DateTime(2025, 1, 20),
    IsCompleted = false,
    CompletionDate = null,
    ProgressRating = 8,
    Notes = "Tehnički dobro, dorađuje izražajnost."
};

var studentPiece3 = new StudentPiece
{
    StudentId = 3, Student = student3,
    PieceId = 3,   Piece = piece3,
    StartDate = new DateTime(2025, 3, 14),
    IsCompleted = false,
    CompletionDate = null,
    ProgressRating = 3,
    Notes = "Početak rada na skladbi."
};

// Petra uči i Für Elise kao pripremu za naprednu razinu
var studentPiece4 = new StudentPiece
{
    StudentId = 2, Student = student2,
    PieceId = 1,   Piece = piece1,
    StartDate = new DateTime(2024, 10, 5),
    IsCompleted = true,
    CompletionDate = new DateTime(2024, 12, 20),
    ProgressRating = 10,
    Notes = "Završeno, izvedeno na godišnjem koncertu."
};

student1.StudentPieces.Add(studentPiece1);
student2.StudentPieces.Add(studentPiece2);
student2.StudentPieces.Add(studentPiece4);
student3.StudentPieces.Add(studentPiece3);

// --- Vježbe ---
var practiceSession1 = new PracticeSession
{
    Id = 1,
    Date = new DateTime(2025, 3, 11, 17, 0, 0),
    DurationMinutes = 20,
    Notes = "Fokus na lijevoj ruci, taktovi 1-10.",
    QualityRating = 4,
    Goals = "Svladati lijevu ruku taktova 1-20.",
    StudentId = 1, Student = student1,
    PieceId = 1,   Piece = piece1
};

var practiceSession2 = new PracticeSession
{
    Id = 2,
    Date = new DateTime(2025, 3, 13, 18, 30, 0),
    DurationMinutes = 30,
    Notes = "Radila cijelu skladbu sporim tempom.",
    QualityRating = 5,
    Goals = "Postići kontinuitet bez zaustavljanja.",
    StudentId = 2, Student = student2,
    PieceId = 2,   Piece = piece2
};

var practiceSession3 = new PracticeSession
{
    Id = 3,
    Date = new DateTime(2025, 3, 15, 16, 0, 0),
    DurationMinutes = 15,
    Notes = "Vježbao ljestvice i prvih 8 taktova Minueta.",
    QualityRating = 3,
    Goals = "Zapamtiti noty prvih 8 taktova napamet.",
    StudentId = 3, Student = student3,
    PieceId = 3,   Piece = piece3
};

student1.Lessons.Add(lesson1);
student1.Lessons.Add(lesson4);
student1.PracticeSessions.Add(practiceSession1);

student2.Lessons.Add(lesson2);
student2.PracticeSessions.Add(practiceSession2);

student3.Lessons.Add(lesson3);
student3.PracticeSessions.Add(practiceSession3);

teacher1.Lessons.Add(lesson1);
teacher1.Lessons.Add(lesson2);
teacher1.Lessons.Add(lesson4);
teacher3.Lessons.Add(lesson3);

piece1.PracticeSessions.Add(practiceSession1);
piece2.PracticeSessions.Add(practiceSession2);
piece3.PracticeSessions.Add(practiceSession3);

// Skupljamo sve objekte u liste za lakši pristup (koristit će se za LINQ upite)
var studenti = new List<Student> { student1, student2, student3 };
var nastavnici = new List<Teacher> { teacher1, teacher2, teacher3 };
var skladbe = new List<Piece> { piece1, piece2, piece3 };
var satovi = new List<Lesson> { lesson1, lesson2, lesson3, lesson4 };

// -------------------------------------------------------
// LINQ upiti
// -------------------------------------------------------

// 1. Dohvati sve učenike razreda 3 ili višeg, sortirane po razredu silazno
var napredniUcenici = studenti
    .Where(s => s.Grade >= 3)
    .OrderByDescending(s => s.Grade)
    .ToList();

// 2. Dohvati sve završene satove (Status == Completed)
var zavrseniSatovi = satovi
    .Where(l => l.Status == LessonStatus.Completed)
    .ToList();

// 3. Dohvati sve skladbe težine Beginner ili Elementary, sortirane po naslovu
var lakeSkladbe = skladbe
    .Where(p => p.Difficulty == DifficultyLevel.Beginner || p.Difficulty == DifficultyLevel.Elementary)
    .OrderBy(p => p.Title)
    .ToList();

// 4. Pronađi učenika s najvećim brojem odrađenih vježbi
var najmarljivijiUcenik = studenti
    .OrderByDescending(s => s.PracticeSessions.Count)
    .First();

// 5. Ukupno vrijeme vježbanja po učeniku (u minutama)
var vrijemeVjezbanjaPoUceniku = studenti
    .Select(s => new
    {
        Ime = s.FirstName + " " + s.LastName,
        UkupnoMinuta = s.PracticeSessions.Sum(ps => ps.DurationMinutes)
    })
    .ToList();

// 6. Dohvati sve skladbe koje trenutno uči barem jedan učenik (nisu završene)
var skladbeUTijeku = skladbe
    .Where(p => p.StudentPieces.Any(sp => !sp.IsCompleted))
    .ToList();

// 7. Dohvati satove nastavnika Marije Horvat sortirane po datumu
var satoviMarijeHorvat = satovi
    .Where(l => l.Teacher.LastName == "Horvat")
    .OrderBy(l => l.ScheduledDate)
    .ToList();

// 8. Prosječna ocjena kvalitete vježbanja po skladbi
var prosjecnaOcjenaPoSkladbi = skladbe
    .Select(p => new
    {
        Naziv = p.Title,
        ProsjecnaOcjena = p.PracticeSessions.Count > 0
            ? p.PracticeSessions.Average(ps => ps.QualityRating)
            : 0
    })
    .ToList();

// 9. Dohvati učenike koji imaju zakazan (Scheduled) sat u budućnosti
var uceniciSaZakazanimSatom = studenti
    .Where(s => s.Lessons.Any(l => l.Status == LessonStatus.Scheduled))
    .ToList();

// 10. Broj satova po nastavniku
var brojSatovaPoNastavniku = nastavnici
    .Select(t => new
    {
        Ime = t.FirstName + " " + t.LastName,
        BrojSatova = t.Lessons.Count
    })
    .ToList();

app.Run();
