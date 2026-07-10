using System.Linq;
using PianoLearningTracker.Models;

namespace PianoLearningTracker.Repositories
{
    // NAPOMENA: Mock implementacija iz Lab2 (statički podaci u memoriji).
    // Od Lab3 aplikacija koristi EfRepository (registriran u Program.cs).
    // Zadržano namjerno kao referenca DI/repository obrasca — nije aktivno u upotrebi.
    public class MockRepository : IRepository
    {
        private readonly List<Student> _students;
        private readonly List<Teacher> _teachers;
        private readonly List<Piece> _pieces;
        private readonly List<Lesson> _lessons;
        private readonly List<PracticeSession> _practiceSessions;

        public MockRepository()
        {
            _students = new List<Student>();
            _teachers = new List<Teacher>();
            _pieces = new List<Piece>();
            _lessons = new List<Lesson>();
            _practiceSessions = new List<PracticeSession>();

            InitializeMockData();
        }

        // --- Read ---
        public IReadOnlyList<Student> GetStudentsForList(int? teacherId = null) =>
            _students.Where(s => teacherId == null || s.TeacherId == teacherId).ToList();
        public IReadOnlyList<Student> GetStudentsWithProgress(int? teacherId = null) =>
            _students.Where(s => teacherId == null || s.TeacherId == teacherId).ToList();
        public Student? GetStudentById(int id) => _students.FirstOrDefault(s => s.Id == id);

        public IReadOnlyList<Teacher> GetAllTeachers() => _teachers;
        public Teacher? GetTeacherById(int id) => _teachers.FirstOrDefault(t => t.Id == id);
        public Teacher? GetStudentTeacher(int studentId)
        {
            var student = _students.FirstOrDefault(s => s.Id == studentId);
            return student?.Teacher
                ?? (student?.TeacherId != null ? _teachers.FirstOrDefault(t => t.Id == student.TeacherId) : null);
        }

        public IReadOnlyList<Piece> GetAllPieces() => _pieces;
        public Piece? GetPieceById(int id) => _pieces.FirstOrDefault(p => p.Id == id);

        public IReadOnlyList<Lesson> GetAllLessons() => _lessons;
        public IReadOnlyList<Lesson> GetLessonsByTeacherId(int teacherId) => _lessons.Where(l => l.TeacherId == teacherId).ToList();
        public IReadOnlyList<Lesson> GetLessonsByStudentId(int studentId) => _lessons.Where(l => l.StudentId == studentId).ToList();
        public Lesson? GetLessonById(int id) => _lessons.FirstOrDefault(l => l.Id == id);

        public IReadOnlyList<PracticeSession> GetAllPracticeSessions() => _practiceSessions;
        public IReadOnlyList<PracticeSession> GetPracticeSessionsByStudentId(int studentId) => _practiceSessions.Where(ps => ps.StudentId == studentId).ToList();
        public IReadOnlyList<PracticeSession> GetPracticeSessionsByTeacherId(int teacherId) => _practiceSessions.Where(ps => ps.Student?.TeacherId == teacherId).ToList();
        public PracticeSession? GetPracticeSessionById(int id) => _practiceSessions.FirstOrDefault(ps => ps.Id == id);

        // --- Count ---
        public int CountStudents() => _students.Count;
        public int CountTeachers() => _teachers.Count;
        public int CountPieces() => _pieces.Count;
        public int CountLessons() => _lessons.Count;

        // --- Search ---
        public IReadOnlyList<Student> SearchStudents(string query, int? teacherId = null) =>
            _students.Where(s => (teacherId == null || s.TeacherId == teacherId) &&
                                 (string.IsNullOrEmpty(query) ||
                                  (s.FirstName + " " + s.LastName).Contains(query, StringComparison.OrdinalIgnoreCase) ||
                                  s.Email.Contains(query, StringComparison.OrdinalIgnoreCase))).ToList();

        public IReadOnlyList<Teacher> SearchTeachers(string query) =>
            _teachers.Where(t => string.IsNullOrEmpty(query) ||
                                 (t.FirstName + " " + t.LastName).Contains(query, StringComparison.OrdinalIgnoreCase) ||
                                 t.Email.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();

        public IReadOnlyList<Piece> SearchPieces(string query) =>
            _pieces.Where(p => string.IsNullOrEmpty(query) ||
                               p.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                               p.Composer.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();

        public IReadOnlyList<Lesson> SearchLessons(string query, int? teacherId = null, int? studentId = null) =>
            _lessons.Where(l => (teacherId == null || l.TeacherId == teacherId) &&
                                (studentId == null || l.StudentId == studentId) &&
                                (string.IsNullOrEmpty(query) ||
                                 (l.Student != null && (l.Student.FirstName + " " + l.Student.LastName).Contains(query, StringComparison.OrdinalIgnoreCase)) ||
                                 (l.Teacher != null && (l.Teacher.FirstName + " " + l.Teacher.LastName).Contains(query, StringComparison.OrdinalIgnoreCase)))).ToList();

        public IReadOnlyList<PracticeSession> SearchPracticeSessions(string query, int? studentId = null, int? teacherId = null) =>
            _practiceSessions.Where(ps => (studentId == null || ps.StudentId == studentId) &&
                                         (teacherId == null || ps.Student?.TeacherId == teacherId) &&
                                         (string.IsNullOrEmpty(query) ||
                                          (ps.Student != null && (ps.Student.FirstName + " " + ps.Student.LastName).Contains(query, StringComparison.OrdinalIgnoreCase)) ||
                                          (ps.Piece != null && ps.Piece.Title.Contains(query, StringComparison.OrdinalIgnoreCase)))).ToList();

        // --- Autocomplete ---
        public IReadOnlyList<AutocompleteItem> AutocompleteTeachers(string query) =>
            _teachers.Where(t => string.IsNullOrEmpty(query) ||
                                 (t.FirstName + " " + t.LastName).Contains(query, StringComparison.OrdinalIgnoreCase))
                     .Take(10).Select(t => new AutocompleteItem(t.Id, t.FirstName + " " + t.LastName)).ToList();

        public IReadOnlyList<AutocompleteItem> AutocompleteStudents(string query, int? teacherId = null) =>
            _students.Where(s => (teacherId == null || s.TeacherId == teacherId) &&
                                 (string.IsNullOrEmpty(query) ||
                                  (s.FirstName + " " + s.LastName).Contains(query, StringComparison.OrdinalIgnoreCase)))
                     .Take(10).Select(s => new AutocompleteItem(s.Id, s.FirstName + " " + s.LastName)).ToList();

        public IReadOnlyList<AutocompleteItem> AutocompletePieces(string query) =>
            _pieces.Where(p => string.IsNullOrEmpty(query) ||
                               p.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                               p.Composer.Contains(query, StringComparison.OrdinalIgnoreCase))
                   .Take(10).Select(p => new AutocompleteItem(p.Id, p.Title + " — " + p.Composer)).ToList();

        // --- Student CRUD ---
        public void AddStudent(Student student) { student.Id = _students.Count > 0 ? _students.Max(s => s.Id) + 1 : 1; _students.Add(student); }
        public void UpdateStudent(Student student) { var i = _students.FindIndex(s => s.Id == student.Id); if (i >= 0) _students[i] = student; }
        public void DeleteStudent(int id) => _students.RemoveAll(s => s.Id == id);

        // --- Teacher CRUD ---
        public void AddTeacher(Teacher teacher) { teacher.Id = _teachers.Count > 0 ? _teachers.Max(t => t.Id) + 1 : 1; _teachers.Add(teacher); }
        public void UpdateTeacher(Teacher teacher) { var i = _teachers.FindIndex(t => t.Id == teacher.Id); if (i >= 0) _teachers[i] = teacher; }
        public void DeleteTeacher(int id) => _teachers.RemoveAll(t => t.Id == id);

        // --- Piece CRUD ---
        public void AddPiece(Piece piece) { piece.Id = _pieces.Count > 0 ? _pieces.Max(p => p.Id) + 1 : 1; _pieces.Add(piece); }
        public void UpdatePiece(Piece piece) { var i = _pieces.FindIndex(p => p.Id == piece.Id); if (i >= 0) _pieces[i] = piece; }
        public void DeletePiece(int id) => _pieces.RemoveAll(p => p.Id == id);

        // --- Lesson CRUD ---
        public void AddLesson(Lesson lesson) { lesson.Id = _lessons.Count > 0 ? _lessons.Max(l => l.Id) + 1 : 1; _lessons.Add(lesson); }
        public void UpdateLesson(Lesson lesson) { var i = _lessons.FindIndex(l => l.Id == lesson.Id); if (i >= 0) _lessons[i] = lesson; }
        public void DeleteLesson(int id) => _lessons.RemoveAll(l => l.Id == id);

        // --- PracticeSession CRUD ---
        public void AddPracticeSession(PracticeSession session) { session.Id = _practiceSessions.Count > 0 ? _practiceSessions.Max(p => p.Id) + 1 : 1; _practiceSessions.Add(session); }
        public void UpdatePracticeSession(PracticeSession session) { var i = _practiceSessions.FindIndex(p => p.Id == session.Id); if (i >= 0) _practiceSessions[i] = session; }
        public void DeletePracticeSession(int id) => _practiceSessions.RemoveAll(p => p.Id == id);

        private void InitializeMockData()
        {
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
                Notes = string.Empty,
                HomeworkAssigned = string.Empty,
                StudentId = 1,
                TeacherId = 1,
                Student = student1,
                Teacher = teacher1
            };

            var lessonPiece1 = new LessonPiece
            {
                LessonId = 1,
                Lesson = lesson1,
                PieceId = 1,
                Piece = piece1,
                Notes = "Obrađeni uvodnim taktovi."
            };

            var lessonPiece2 = new LessonPiece
            {
                LessonId = 2,
                Lesson = lesson2,
                PieceId = 2,
                Piece = piece2,
                Notes = "Rad na dinamici cijele skladbe."
            };

            var lessonPiece3 = new LessonPiece
            {
                LessonId = 3,
                Lesson = lesson3,
                PieceId = 3,
                Piece = piece3,
                Notes = "Usvajanje prvih 8 taktova."
            };

            lesson1.LessonPieces.Add(lessonPiece1);
            lesson2.LessonPieces.Add(lessonPiece2);
            lesson3.LessonPieces.Add(lessonPiece3);

            var studentPiece1 = new StudentPiece
            {
                StudentId = 1,
                Student = student1,
                PieceId = 1,
                Piece = piece1,
                StartDate = new DateTime(2025, 3, 10),
                IsCompleted = false,
                CompletionDate = null,
                ProgressRating = 6,
                Notes = "Svladao prvu stranicu, radi na drugoj."
            };

            var studentPiece2 = new StudentPiece
            {
                StudentId = 2,
                Student = student2,
                PieceId = 2,
                Piece = piece2,
                StartDate = new DateTime(2025, 1, 20),
                IsCompleted = false,
                CompletionDate = null,
                ProgressRating = 8,
                Notes = "Tehnički dobro, dorađuje izražajnost."
            };

            var studentPiece3 = new StudentPiece
            {
                StudentId = 3,
                Student = student3,
                PieceId = 3,
                Piece = piece3,
                StartDate = new DateTime(2025, 3, 14),
                IsCompleted = false,
                CompletionDate = null,
                ProgressRating = 3,
                Notes = "Početak rada na skladbi."
            };

            var studentPiece4 = new StudentPiece
            {
                StudentId = 2,
                Student = student2,
                PieceId = 1,
                Piece = piece1,
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

            var practiceSession1 = new PracticeSession
            {
                Id = 1,
                Date = new DateTime(2025, 3, 11, 17, 0, 0),
                DurationMinutes = 20,
                Notes = "Fokus na lijevoj ruci, taktovi 1-10.",
                QualityRating = 4,
                Goals = "Svladati lijevu ruku taktova 1-20.",
                StudentId = 1,
                Student = student1,
                PieceId = 1,
                Piece = piece1
            };

            var practiceSession2 = new PracticeSession
            {
                Id = 2,
                Date = new DateTime(2025, 3, 13, 18, 30, 0),
                DurationMinutes = 30,
                Notes = "Radila cijelu skladbu sporim tempom.",
                QualityRating = 5,
                Goals = "Postići kontinuitet bez zaustavljanja.",
                StudentId = 2,
                Student = student2,
                PieceId = 2,
                Piece = piece2
            };

            var practiceSession3 = new PracticeSession
            {
                Id = 3,
                Date = new DateTime(2025, 3, 15, 16, 0, 0),
                DurationMinutes = 15,
                Notes = "Vježbao ljestvice i prvih 8 taktova Minueta.",
                QualityRating = 3,
                Goals = "Zapamtiti noty prvih 8 taktova napamet.",
                StudentId = 3,
                Student = student3,
                PieceId = 3,
                Piece = piece3
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

            _students.AddRange(new[] { student1, student2, student3 });
            _teachers.AddRange(new[] { teacher1, teacher2, teacher3 });
            _pieces.AddRange(new[] { piece1, piece2, piece3 });
            _lessons.AddRange(new[] { lesson1, lesson2, lesson3, lesson4 });
            _practiceSessions.AddRange(new[] { practiceSession1, practiceSession2, practiceSession3 });
        }
    }
}
