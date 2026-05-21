using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PianoLearningTracker.DAL;
using PianoLearningTracker.Models;

namespace PianoLearningTracker.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly PianoLearningTrackerDbContext _context;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            PianoLearningTrackerDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
                return LocalRedirect(returnUrl ?? "/");

            ModelState.AddModelError(string.Empty, "Pogrešan email ili lozinka.");
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Register()
        {
            ViewBag.Roles = await GetAvailableRolesAsync();
            ViewBag.Teachers = await _context.Teachers.OrderBy(t => t.LastName).ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            ViewBag.Roles = await GetAvailableRolesAsync();
            ViewBag.Teachers = await _context.Teachers.OrderBy(t => t.LastName).ToListAsync();

            ValidateRoleSpecificFields(model);

            if (!ModelState.IsValid)
                return View(model);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(model);
            }

            await _userManager.AddToRoleAsync(user, model.Role);

            if (model.Role == "Teacher")
            {
                var teacher = new Teacher
                {
                    FirstName = model.FirstName!,
                    LastName = model.LastName!,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber ?? "",
                    Specialization = model.Specialization,
                    YearsOfExperience = model.YearsOfExperience ?? 0,
                    HireDate = DateTime.Today
                };
                _context.Teachers.Add(teacher);
                await _context.SaveChangesAsync();

                user.TeacherId = teacher.Id;
                await _userManager.UpdateAsync(user);
            }
            else if (model.Role == "Student")
            {
                var student = new Student
                {
                    FirstName = model.FirstName!,
                    LastName = model.LastName!,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber ?? "",
                    DateOfBirth = model.DateOfBirth ?? DateTime.Today,
                    EnrollmentDate = DateTime.Today,
                    Grade = model.Grade ?? 1,
                    TeacherId = model.TeacherId
                };
                _context.Students.Add(student);
                await _context.SaveChangesAsync();

                user.StudentId = student.Id;
                await _userManager.UpdateAsync(user);
            }

            TempData["Success"] = $"Korisnik {model.Email} uspješno kreiran s ulogom {model.Role}.";
            return RedirectToAction("UserList");
        }

        private void ValidateRoleSpecificFields(RegisterViewModel model)
        {
            if (model.Role == "Teacher" || model.Role == "Student")
            {
                if (string.IsNullOrWhiteSpace(model.FirstName))
                    ModelState.AddModelError(nameof(model.FirstName), "Ime je obavezno.");
                if (string.IsNullOrWhiteSpace(model.LastName))
                    ModelState.AddModelError(nameof(model.LastName), "Prezime je obavezno.");
            }
            if (model.Role == "Student")
            {
                if (model.DateOfBirth == null)
                    ModelState.AddModelError(nameof(model.DateOfBirth), "Datum rođenja je obavezan.");
                if (model.Grade == null)
                    ModelState.AddModelError(nameof(model.Grade), "Razred je obavezan.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UserList()
        {
            var users = _userManager.Users.ToList();
            var userRoles = new Dictionary<string, IList<string>>();

            foreach (var user in users)
                userRoles[user.Id] = await _userManager.GetRolesAsync(user);

            ViewBag.UserRoles = userRoles;
            return View(users);
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        private async Task<List<string>> GetAvailableRolesAsync()
        {
            return await Task.FromResult(
                _roleManager.Roles.Select(r => r.Name!).OrderBy(n => n).ToList()
            );
        }
    }
}
