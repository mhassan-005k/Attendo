using Microsoft.EntityFrameworkCore;
namespace AttendanceApp.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt) { }

        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<StudentAttendance> StudentAttendance { get; set; }
    }
}
