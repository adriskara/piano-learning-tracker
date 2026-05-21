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
        private readonly IMockRepository _mockRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(IMockRepository mockRepository, UserManager<ApplicationUser> userManager)
        {
            _mockRepository = mockRepository;
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
                StudentCount = _mockRepository.GetAllStudents().Count,
                TeacherCount = _mockRepository.GetAllTeachers().Count,
                LessonCount = _mockRepository.GetAllLessons().Count,
                PieceCount = _mockRepository.GetAllPieces().Count
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
            var allStudents = _mockRepository.GetAllStudents();
            var allLessons = _mockRepository.GetAllLessons();
            var allSessions = _mockRepository.GetAllPracticeSessions();

            return new HomeIndexViewModel
            {
                StudentCount = allStudents.Count,
                TeacherCount = _mockRepository.GetAllTeachers().Count,
                PieceCount = _mockRepository.GetAllPieces().Count,
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
                RecentAchievements = allStudents.SelectMany(s => s.StudentPieces).Where(sp => sp.IsCompleted).ToList()
            };
        }

        private HomeIndexViewModel BuildTeacherDashboard(int teacherId, string email)
        {
            var myStudents = _mockRepository.GetStudentsByTeacherId(teacherId);
            var myLessons = _mockRepository.GetLessonsByTeacherId(teacherId);
            var mySessions = _mockRepository.GetPracticeSessionsByTeacherId(teacherId);

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
                RecentAchievements = myStudents.SelectMany(s => s.StudentPieces).Where(sp => sp.IsCompleted).ToList()
            };
        }

        private HomeIndexViewModel BuildStudentDashboard(int studentId, string email)
        {
            var myLessons = _mockRepository.GetLessonsByStudentId(studentId);
            var mySessions = _mockRepository.GetPracticeSessionsByStudentId(studentId);
            var myStudent = _mockRepository.GetStudentById(studentId);

            return new HomeIndexViewModel
            {
                LessonCount = myLessons.Count,
                PracticeSessionCount = mySessions.Count,
                ScheduledLessonCount = myLessons.Count(l => l.Status == LessonStatus.Scheduled),
                PieceCount = _mockRepository.GetAllPieces().Count,
                CompletedPieceCount = myStudent?.StudentPieces.Count(sp => sp.IsCompleted) ?? 0,
                NextLesson = myLessons.Where(l => l.Status == LessonStatus.Scheduled && l.ScheduledDate >= DateTime.Now)
                                      .OrderBy(l => l.ScheduledDate).FirstOrDefault(),
                LatestPracticeSession = mySessions.OrderByDescending(s => s.Date).FirstOrDefault(),
                RecentAchievements = myStudent?.StudentPieces.Where(sp => sp.IsCompleted).ToList() ?? new()
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
