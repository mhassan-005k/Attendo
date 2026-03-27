using Microsoft.AspNetCore.Mvc;
using AttendanceApp.Models;

namespace AttendanceApp.Controllers
{
    public class StudentController : Controller
    {
        
            private readonly AppDbContext _context;

            public StudentController(AppDbContext context)
            {
                _context = context;
            }

            public IActionResult JoinClass()
            {
                ViewBag.Classes = _context.Classes.Where(c => c.IsActive).ToList();
                return View();
            }

            [HttpPost]
            public IActionResult JoinClass(StudentJoinViewModel vm)
            {
                var cls = _context.Classes.FirstOrDefault(c => c.ClassId == vm.ClassId);
                if (cls == null) return NotFound();

                var today = DateTime.Today;

                var student = _context.StudentAttendance.FirstOrDefault(a =>
                    a.CMS_ID == vm.CMS_ID &&
                    a.ClassId == vm.ClassId &&
                    a.JoinDate == today);

                if (student == null)
                {
                    student = new StudentAttendance
                    {
                        ClassId = vm.ClassId,
                        Name = vm.Name,
                        CMS_ID = vm.CMS_ID,
                        Email = vm.Email,
                        JoinDate = today,
                        JoinTime = DateTime.Now,
                        JoinCount = 1
                    };

                    _context.StudentAttendance.Add(student);
                    _context.SaveChanges();
                }
                else
                {
                    if (student.JoinCount >= 4)
                    {
                        return Content("❌ You exceeded allowed reconnect attempts for today.");
                    }

                    student.JoinCount++;
                    _context.SaveChanges();
                }

                return Redirect(cls.ZoomLink);
            }
    }
}
