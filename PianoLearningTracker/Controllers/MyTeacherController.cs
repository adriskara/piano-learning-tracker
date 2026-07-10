using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PianoLearningTracker.Models;
using PianoLearningTracker.Repositories;

namespace PianoLearningTracker.Controllers
{
    // Student-facing prikaz vlastitog profesora. Namjerno NE koristi TeacherController
    // (koji je Admin-only) niti Teacher/Details view (koji izlaže roster drugih učenika).
    [Authorize(Roles = "Student")]
    [Route("moj-profesor")]
    public class MyTeacherController : Controller
    {
        private readonly IRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;

        public MyTeacherController(IRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        // URL: /moj-profesor
        [Route("")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.StudentId == null)
                return View(null);

            var teacher = _repository.GetStudentTeacher(user.StudentId.Value);
            if (teacher == null)
                return View(null);

            var vm = new MyTeacherViewModel
            {
                FirstName = teacher.FirstName,
                LastName = teacher.LastName,
                Specialization = teacher.Specialization,
                YearsOfExperience = teacher.YearsOfExperience,
                Biography = teacher.Biography,
                Email = teacher.Email,
                PhoneNumber = teacher.PhoneNumber
            };
            return View(vm);
        }
    }
}
