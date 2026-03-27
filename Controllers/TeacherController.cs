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

            //var data = _context.StudentAttendance
            //    .GroupBy(a => new { a.ClassId, a.JoinDate })
            //    .Select(g => new AttendanceSummary
            //    {
            //        ClassId = g.Key.ClassId,
            //        ClassName = _context.Classes.First(c => c.ClassId == g.Key.ClassId).ClassName,
            //        Date = g.Key.JoinDate,
            //        Total = g.Count()
            //    })
            //    .OrderByDescending(x => x.Date)
            //    .ToList();
            int teacherId = HttpContext.Session.GetInt32("TeacherId").Value;
            var data = _context.StudentAttendance
                .Where(a => _context.Classes
                .Any(c => c.ClassId == a.ClassId && c.TeacherId == teacherId))
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

            // Get classes for this teacher (for Edit buttons)
            var classes = _context.Classes
                .Where(c => c.TeacherId == teacherId)
                .ToList();

            ViewBag.Classes = classes;

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

            //var model = new Class();
            return View(new Class());
        }

        [HttpPost]
        public IActionResult CreateClass(Class model)
        {
            if (!LoggedIn())
                return RedirectToAction("Login", "Auth");

            var teacherId = HttpContext.Session.GetInt32("TeacherId");
            if (model.ClassId == 0)
            {

                model.TeacherId = teacherId.Value;   //Set TeacherId here

                model.IsActive = true;
                _context.Classes.Add(model);
            }
            else
            {
                var cls = _context.Classes.FirstOrDefault(c => c.ClassId == model.ClassId && c.TeacherId == teacherId);
                if (cls == null)
                    return NotFound();
                cls.ClassName = model.ClassName;
                cls.ZoomLink = model.ZoomLink;
            }

            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        public IActionResult EditClass(int id)
        {
            if (!LoggedIn())
                return RedirectToAction("Login", "Auth");

            var teacherId = HttpContext.Session.GetInt32("TeacherId");

            var cls = _context.Classes
                .FirstOrDefault(c => c.ClassId == id && c.TeacherId == teacherId);

            if (cls == null)
                return NotFound();

            return View("CreateClass", cls);  // ← Load same page
        }

        public IActionResult DeleteClass(int id)
        {
            if (!LoggedIn())
                return RedirectToAction("Login", "Auth");

            var teacherId = HttpContext.Session.GetInt32("TeacherId");

            var cls = _context.Classes
                .FirstOrDefault(c => c.ClassId == id && c.TeacherId == teacherId);

            if (cls == null)
                return NotFound();

            // Option 1: Delete class but keep attendance
            _context.Classes.Remove(cls);
            _context.SaveChanges();

            // Option 2 (safer): just mark it inactive
            // cls.IsActive = false;
            // _context.SaveChanges();

            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("TeacherId");   // Remove teacher session
            return RedirectToAction("Login", "Auth");   // Redirect to login page
        }
        public IActionResult History()
        {
            if (!LoggedIn())
                return RedirectToAction("Login", "Auth");

            var teacherId = HttpContext.Session.GetInt32("TeacherId");

            var data = _context.StudentAttendance
                .Where(x => x.TeacherId == teacherId)
                .GroupBy(a => new { a.ClassId, a.JoinDate })
                .Select(g => new AttendanceSummary
                {
                    ClassId = g.Key.ClassId,
                    ClassName = _context.Classes
                                .First(c => c.ClassId == g.Key.ClassId).ClassName,
                    Date = g.Key.JoinDate,
                    Total = g.Count()
                })
                .OrderByDescending(x => x.Date)
                .ToList();

            return View(data);
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