using AttendanceApp.Models;
using ClosedXML.Excel;
using System.IO;

namespace AttendanceApp.Services
{
    public static class ExcelService
    {
        public static byte[] GenerateAttendanceExcel(string className, DateTime date, List<StudentAttendance> list)
        {
            // No license configuration needed!

            using (var workbook = new XLWorkbook())
            {
                var sheet = workbook.Worksheets.Add("Attendance");

                // Set headers
                sheet.Cell(1, 1).Value = "Name";
                sheet.Cell(1, 2).Value = "CMS";
                sheet.Cell(1, 3).Value = "Email";
                sheet.Cell(1, 4).Value = "Join Time";
                sheet.Cell(1, 5).Value = "Attempts";

                int row = 2;

                // Populate data
                foreach (var s in list)
                {
                    sheet.Cell(row, 1).Value = s.Name;
                    sheet.Cell(row, 2).Value = s.CMS_ID;
                    sheet.Cell(row, 3).Value = s.Email;
                    sheet.Cell(row, 4).Value = s.JoinTime.ToString("hh:mm tt");
                    sheet.Cell(row, 5).Value = s.JoinCount;

                    row++;
                }

                // ClosedXML requires saving to a MemoryStream to get the byte array
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }
    }
}