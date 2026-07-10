using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PianoLearningTracker.Models;
using PianoLearningTracker.Repositories;
using System.Diagnostics;

namespace PianoLearningTracker.Controllers
{
    [Route("pocetna")]
    public class HomeController : Controller
    {
        private readonly IRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(IRepository mockRepository, UserManager<ApplicationUser> userManager)
        {
            _repository = mockRepository;
            _userManager = userManager;
        }

        // Javna landing page — dostupna bez prijave
        [AllowAnonymous]
        [Route("/")]
        public IActionResult Landing()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index");

            var vm = new LandingViewModel
            {
                StudentCount = _repository.CountStudents(),
                TeacherCount = _repository.CountTeachers(),
                LessonCount = _repository.CountLessons(),
                PieceCount = _repository.CountPieces()
            };
            return View(vm);
        }

        // Dashboard — prilagođen prema ulozi
        [Authorize]
        [Route("")]
        [Route("dashboard")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var role = User.IsInRole("Administrator") ? "Administrator"
                     : User.IsInRole("Teacher") ? "Teacher"
                     : "Student";

            HomeIndexViewModel vm;

            if (role == "Teacher" && user?.TeacherId != null)
                vm = BuildTeacherDashboard(user.TeacherId.Value, user.Email ?? "");
            else if (role == "Student" && user?.StudentId != null)
                vm = BuildStudentDashboard(user.StudentId.Value, user.Email ?? "");
            else
                vm = BuildAdminDashboard();

            vm.UserRole = role;
            vm.UserDisplayName = user?.Email;

            return View(vm);
        }

        private HomeIndexViewModel BuildAdminDashboard()
        {
            var allStudents = _repository.GetStudentsWithProgress();
            var allLessons = _repository.GetAllLessons();
            var allSessions = _repository.GetAllPracticeSessions();
            var today = DateTime.Today;

            return new HomeIndexViewModel
            {
                StudentCount = allStudents.Count,
                TeacherCount = _repository.CountTeachers(),
                PieceCount = _repository.CountPieces(),
                LessonCount = allLessons.Count,
                PracticeSessionCount = allSessions.Count,
                ScheduledLessonCount = allLessons.Count(l => l.Status == LessonStatus.Scheduled),
                CompletedPieceCount = allStudents.SelectMany(s => s.StudentPieces).Count(sp => sp.IsCompleted),
                NextLesson = allLessons.Where(l => l.Status == LessonStatus.Scheduled && l.ScheduledDate >= DateTime.Now)
                                       .OrderBy(l => l.ScheduledDate).FirstOrDefault(),
                LatestPracticeSession = allSessions.OrderByDescending(s => s.Date).FirstOrDefault(),
                PracticeStreaks = allStudents.Select(s => new PracticeStreakItem
                {
                    StudentName = $"{s.FirstName} {s.LastName}",
                    SessionCount = s.PracticeSessions.Count,
                    TotalMinutes = s.PracticeSessions.Sum(ps => ps.DurationMinutes)
                }).OrderByDescending(x => x.SessionCount).ToList(),
                RecentAchievements = allStudents.SelectMany(s => s.StudentPieces).Where(sp => sp.IsCompleted).ToList(),
                HomeworkLessons = allLessons
                    .Where(l => !string.IsNullOrWhiteSpace(l.HomeworkAssigned))
                    .OrderByDescending(l => l.ScheduledDate)
                    .Take(5)
                    .ToList(),
                CalendarLessons = allLessons
                    .Where(l => l.ScheduledDate.Year == today.Year && l.ScheduledDate.Month == today.Month)
                    .OrderBy(l => l.ScheduledDate)
                    .ToList()
            };
        }

        private HomeIndexViewModel BuildTeacherDashboard(int teacherId, string email)
        {
            var myStudents = _repository.GetStudentsWithProgress(teacherId);
            var myLessons = _repository.GetLessonsByTeacherId(teacherId);
            var mySessions = _repository.GetPracticeSessionsByTeacherId(teacherId);
            var today = DateTime.Today;
            var myTeacher = _repository.GetTeacherById(teacherId);
            var firstName = myTeacher?.FirstName ?? "";

            return new HomeIndexViewModel
            {
                StudentCount = myStudents.Count,
                LessonCount = myLessons.Count,
                PracticeSessionCount = mySessions.Count,
                ScheduledLessonCount = myLessons.Count(l => l.Status == LessonStatus.Scheduled),
                CompletedPieceCount = myStudents.SelectMany(s => s.StudentPieces).Count(sp => sp.IsCompleted),
                NextLesson = myLessons.Where(l => l.Status == LessonStatus.Scheduled && l.ScheduledDate >= DateTime.Now)
                                      .OrderBy(l => l.ScheduledDate).FirstOrDefault(),
                LatestPracticeSession = mySessions.OrderByDescending(s => s.Date).FirstOrDefault(),
                PracticeStreaks = myStudents.Select(s => new PracticeStreakItem
                {
                    StudentName = $"{s.FirstName} {s.LastName}",
                    SessionCount = s.PracticeSessions.Count,
                    TotalMinutes = s.PracticeSessions.Sum(ps => ps.DurationMinutes)
                }).OrderByDescending(x => x.SessionCount).ToList(),
                RecentAchievements = myStudents.SelectMany(s => s.StudentPieces).Where(sp => sp.IsCompleted).ToList(),
                HomeworkLessons = myLessons
                    .Where(l => !string.IsNullOrWhiteSpace(l.HomeworkAssigned))
                    .OrderByDescending(l => l.ScheduledDate)
                    .Take(5)
                    .ToList(),
                CalendarLessons = myLessons
                    .Where(l => l.ScheduledDate.Year == today.Year && l.ScheduledDate.Month == today.Month)
                    .OrderBy(l => l.ScheduledDate)
                    .ToList(),
                UserFirstName = firstName
            };
        }

        private HomeIndexViewModel BuildStudentDashboard(int studentId, string email)
        {
            var myLessons = _repository.GetLessonsByStudentId(studentId);
            var mySessions = _repository.GetPracticeSessionsByStudentId(studentId);
            var myStudent = _repository.GetStudentById(studentId);
            var today = DateTime.Today;
            var firstName = myStudent?.FirstName ?? "";

            return new HomeIndexViewModel
            {
                LessonCount = myLessons.Count,
                PracticeSessionCount = mySessions.Count,
                ScheduledLessonCount = myLessons.Count(l => l.Status == LessonStatus.Scheduled),
                PieceCount = _repository.CountPieces(),
                CompletedPieceCount = myStudent?.StudentPieces.Count(sp => sp.IsCompleted) ?? 0,
                NextLesson = myLessons.Where(l => l.Status == LessonStatus.Scheduled && l.ScheduledDate >= DateTime.Now)
                                      .OrderBy(l => l.ScheduledDate).FirstOrDefault(),
                LatestPracticeSession = mySessions.OrderByDescending(s => s.Date).FirstOrDefault(),
                RecentAchievements = myStudent?.StudentPieces.Where(sp => sp.IsCompleted).ToList() ?? new(),
                HomeworkLessons = myLessons
                    .Where(l => !string.IsNullOrWhiteSpace(l.HomeworkAssigned))
                    .OrderByDescending(l => l.ScheduledDate)
                    .Take(5)
                    .ToList(),
                CalendarLessons = myLessons
                    .Where(l => l.ScheduledDate.Year == today.Year && l.ScheduledDate.Month == today.Month)
                    .OrderBy(l => l.ScheduledDate)
                    .ToList(),
                UserFirstName = firstName
            };
        }

        // URL: /pocetna/privatnost
        [Authorize]
        [Route("privatnost")]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
