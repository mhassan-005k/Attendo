namespace AttendanceApp.Models
{
    public class StudentAttendance
    {
        public int Id { get; set; }
        public int ClassId { get; set; }

        public string Name { get; set; }

        public string CMS_ID { get; set; }

        public string Email { get; set; }

        public DateTime JoinDate { get; set; }

        public DateTime JoinTime { get; set; }

        public int JoinCount { get; set; }
    }
}
