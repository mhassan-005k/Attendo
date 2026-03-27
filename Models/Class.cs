namespace AttendanceApp.Models
{
    public class Class
    {
        public int ClassId { get; set; }

        public string ClassName { get; set; }

        public string ZoomLink { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
