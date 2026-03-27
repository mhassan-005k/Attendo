# Attendance Tracking System

A simple **online class attendance tracking system** built with **ASP.NET Core MVC**. Teachers can manage Zoom recurring meetings, track student attendance, and download attendance reports as Excel files. Students can join classes directly via a secure form, and reconnect attempts are limited.
## 🔹 Features

### **For Students**

* Select class from a dropdown.
* Fill in Name and CMS ID.
* Automatically redirected to the Zoom meeting after submission.
* Tracks joining time and limits reconnects to 4 per day.

### **For Teachers**

* Login with a secure account.
* Dashboard displays attendance summary **date-wise per class**.
* Download attendance in Excel format.
* Admin-protected page to create new teacher accounts.

### **Admin**

* Admin login protected by a password (`AdminSecret123!` by default).
* Admin can create new teacher accounts with secure BCrypt password hashing.

---

## 🔹 Tech Stack

* **Backend:** ASP.NET Core MVC (.NET 7)
* **Database:** Entity Framework Core (SQL Server)
* **Frontend:** Razor Pages with Bootstrap 5
* **Password Security:** BCrypt.Net
* **Excel Export:** ClosedXML (for generating Excel reports)

---

## 🔹 Installation

1. **Clone the repository**

```bash
git clone https://github.com/mhassan-005k/Attendo.git
cd AttendanceApp
```

2. **Update Connection String** in `appsettings.json`

```json
"ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=AttendanceDB;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

3. **Run Migrations**

```bash
dotnet ef database update
```

4. **Run the Application**

```bash
dotnet run
```

---

## 🔹 Usage

### **Admin**

1. Go to `/Teacher/AdminLogin`
2. Enter the admin password (`AdminSecret123!` by default)
3. Create teacher accounts via `/Teacher/CreateTeacher`

### **Teacher**

1. Go to `/Auth/Login`
2. Enter your username and password
3. Access the dashboard to view attendance summaries and download Excel reports

### **Student**

1. Go to `/Student/JoinClass`
2. Select your class, enter Name and CMS ID
3. Click Join → redirected to Zoom meeting
4. Attendance is automatically recorded

---

## 🔹 Notes

* **Reconnect Limit:** Students can reconnect **up to 4 times per day**. After that, access is blocked.
* **Password Security:** Teacher passwords are securely hashed with **BCrypt**.
* **Excel Export:** Attendance is exported as `.xlsx` with class name and date.

---

## 🔹 Future Enhancements

* Email notifications to teachers on new attendance.
* Admin panel with multiple roles.
* Dashboard analytics with graphs.
* Real-time attendance monitoring.

---

## 🔹 License

MIT License © Muhammad Hassan
