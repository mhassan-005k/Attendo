using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using AttendanceApp.Models;
using BCrypt.Net;

namespace AttendanceApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel vm)
        {
            var teacher = _context.Teachers.FirstOrDefault(t => t.Username == vm.Username);

            if (teacher == null || !BCrypt.Net.BCrypt.Verify(vm.Password, teacher.PasswordHash))
            {
                ViewBag.Error = "Invalid username or password.";
                return View(vm);
            }

            HttpContext.Session.SetInt32("TeacherId", teacher.Id);
            return RedirectToAction("Dashboard", "Teacher");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
