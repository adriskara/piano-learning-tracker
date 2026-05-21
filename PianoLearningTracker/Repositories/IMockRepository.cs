using PianoLearningTracker.Models;

namespace PianoLearningTracker.Repositories
{
    public interface IMockRepository
    {
        // --- Read ---
        IReadOnlyList<Student> GetAllStudents();
        IReadOnlyList<Student> GetStudentsByTeacherId(int teacherId);
        Student? GetStudentById(int id);

        IReadOnlyList<Teacher> GetAllTeachers();
        Teacher? GetTeacherById(int id);

        IReadOnlyList<Piece> GetAllPieces();
        Piece? GetPieceById(int id);

        IReadOnlyList<Lesson> GetAllLessons();
        IReadOnlyList<Lesson> GetLessonsByTeacherId(int teacherId);
        IReadOnlyList<Lesson> GetLessonsByStudentId(int studentId);
        Lesson? GetLessonById(int id);

        IReadOnlyList<PracticeSession> GetAllPracticeSessions();
        IReadOnlyList<PracticeSession> GetPracticeSessionsByStudentId(int studentId);
        IReadOnlyList<PracticeSession> GetPracticeSessionsByTeacherId(int teacherId);
        PracticeSession? GetPracticeSessionById(int id);

        // --- Search (AJAX) ---
        IReadOnlyList<Student> SearchStudents(string query, int? teacherId = null);
        IReadOnlyList<Teacher> SearchTeachers(string query);
        IReadOnlyList<Piece> SearchPieces(string query);
        IReadOnlyList<Lesson> SearchLessons(string query, int? teacherId = null, int? studentId = null);
        IReadOnlyList<PracticeSession> SearchPracticeSessions(string query, int? studentId = null, int? teacherId = null);

        // --- Autocomplete (AJAX dropdown) ---
        IReadOnlyList<AutocompleteItem> AutocompleteTeachers(string query);
        IReadOnlyList<AutocompleteItem> AutocompleteStudents(string query, int? teacherId = null);
        IReadOnlyList<AutocompleteItem> AutocompletePieces(string query);

        // --- Student CRUD ---
        void AddStudent(Student student);
        void UpdateStudent(Student student);
        void DeleteStudent(int id);

        // --- Teacher CRUD ---
        void AddTeacher(Teacher teacher);
        void UpdateTeacher(Teacher teacher);
        void DeleteTeacher(int id);

        // --- Piece CRUD ---
        void AddPiece(Piece piece);
        void UpdatePiece(Piece piece);
        void DeletePiece(int id);

        // --- Lesson CRUD ---
        void AddLesson(Lesson lesson);
        void UpdateLesson(Lesson lesson);
        void DeleteLesson(int id);

        // --- PracticeSession CRUD ---
        void AddPracticeSession(PracticeSession session);
        void UpdatePracticeSession(PracticeSession session);
        void DeletePracticeSession(int id);
    }
}
