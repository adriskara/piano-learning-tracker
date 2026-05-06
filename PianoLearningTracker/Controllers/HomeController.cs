using Microsoft.AspNetCore.Mvc;
using PianoLearningTracker.Models;
using PianoLearningTracker.Repositories;
using System.Diagnostics;

namespace PianoLearningTracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMockRepository _mockRepository;

        public HomeController(IMockRepository mockRepository)
        {
            _mockRepository = mockRepository;
        }

        public IActionResult Index()
        {
            var allStudents = _mockRepository.GetAllStudents();
            var allLessons = _mockRepository.GetAllLessons();
            var allSessions = _mockRepository.GetAllPracticeSessions();

            var nextLesson = allLessons
                .Where(l => l.Status == LessonStatus.Scheduled && l.ScheduledDate >= DateTime.Now)
                .OrderBy(l => l.ScheduledDate)
                .FirstOrDefault();

            var latestSession = allSessions
                .OrderByDescending(s => s.Date)
                .FirstOrDefault();

            var streaks = allStudents
                .Select(s => new PracticeStreakItem
                {
                    StudentName = $"{s.FirstName} {s.LastName}",
                    SessionCount = s.PracticeSessions.Count,
                    TotalMinutes = s.PracticeSessions.Sum(ps => ps.DurationMinutes)
                })
                .OrderByDescending(x => x.SessionCount)
                .ToList();

            var achievements = allStudents
                .SelectMany(s => s.StudentPieces)
                .Where(sp => sp.IsCompleted)
                .ToList();

            var viewModel = new HomeIndexViewModel
            {
                StudentCount = allStudents.Count,
                TeacherCount = _mockRepository.GetAllTeachers().Count,
                PieceCount = _mockRepository.GetAllPieces().Count,
                LessonCount = allLessons.Count,
                PracticeSessionCount = allSessions.Count,
                ScheduledLessonCount = allLessons.Count(l => l.Status == LessonStatus.Scheduled),
                NextLesson = nextLesson,
                LatestPracticeSession = latestSession,
                PracticeStreaks = streaks,
                RecentAchievements = achievements
            };

            return View(viewModel);
        }

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
