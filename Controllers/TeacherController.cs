using AttendanceApp.Models;
using Microsoft.AspNetCore.Mvc;
using AttendanceApp.Models;
using AttendanceApp.Services;
using Microsoft.AspNetCore.Identity;
using BCrypt.Net;

namespace AttendanceApp.Controllers
{
    public class TeacherController : Controller
    {
        private readonly AppDbContext _context;
        // In TeacherController
        private const string AdminPass = "AdminSecret123!";

        public TeacherController(AppDbContext context)
        {
            _context = context;
        }

        private bool LoggedIn()
        {
            return HttpContext.Session.GetInt32("TeacherId") != null;
        }

        public IActionResult Dashboard()
        {
            if (!LoggedIn())
                return RedirectToAction("Login", "Auth");

            var data = _context.StudentAttendance
                .GroupBy(a => new { a.ClassId, a.JoinDate })
                .Select(g => new AttendanceSummary
                {
                    ClassId = g.Key.ClassId,
                    ClassName = _context.Classes.First(c => c.ClassId == g.Key.ClassId).ClassName,
                    Date = g.Key.JoinDate,
                    Total = g.Count()
                })
                .OrderByDescending(x => x.Date)
                .ToList();

            return View(data);
        }

        public IActionResult DownloadExcel(int classId, string date)
        {
            var dt = DateTime.Parse(date);

            var list = _context.StudentAttendance
                .Where(a => a.ClassId == classId && a.JoinDate == dt)
                .ToList();

            var className = _context.Classes.First(c => c.ClassId == classId).ClassName;

            var file = ExcelService.GenerateAttendanceExcel(className, dt, list);

            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"{className}_{dt:yyyy-MM-dd}.xlsx");
        }

        public IActionResult CreateClass()
        {
            if (!LoggedIn())
                return RedirectToAction("Login", "Auth");

            return View();
        }

        [HttpPost]
        public IActionResult CreateClass(Class model)
        {
            if (!LoggedIn())
                return RedirectToAction("Login", "Auth");

            model.IsActive = true;
            _context.Classes.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Dashboard");
        }

        // GET: Admin Login
        public IActionResult AdminLogin()
        {
            return View();
        }

        // POST: Admin Login
        [HttpPost]
        public IActionResult AdminLogin(string password)
        {
            if (password != AdminPass)
            {
                ViewBag.Error = "Invalid admin password";
                return View();
            }

            // store session to remember admin is logged in
            HttpContext.Session.SetString("AdminLogged", "1");
            return RedirectToAction("CreateTeacher");
        }

        // GET: CreateTeacher
        // ----- Create Teacher -----
        [HttpGet]
        public IActionResult CreateTeacher()
        {
            if (HttpContext.Session.GetString("AdminLogged") != "1")
                return RedirectToAction("AdminLogin");

            return View();
        }

        [HttpPost]
        public IActionResult CreateTeacher(string username, string password)
        {
            if (HttpContext.Session.GetString("AdminLogged") != "1")
                return RedirectToAction("AdminLogin");

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Username and Password required";
                return View();
            }

            // Check if username already exists
            if (_context.Teachers.Any(t => t.Username == username))
            {
                ViewBag.Error = "Username already exists!";
                return View();
            }

            // Hash password using BCrypt
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var teacher = new Teacher
            {
                Username = username,
                PasswordHash = hashedPassword
            };

            _context.Teachers.Add(teacher);
            _context.SaveChanges();

            ViewBag.Success = $"Teacher '{username}' created successfully!";
            return View();
        }
    }

    public class AttendanceSummary
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public DateTime Date { get; set; }
        public int Total { get; set; }
    }
}