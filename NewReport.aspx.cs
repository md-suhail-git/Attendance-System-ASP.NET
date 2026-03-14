using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Attendance
{
    public partial class NewReport : System.Web.UI.Page
    {
        private double grandTotalMinutes = 0;
        private double grandOTMinutes = 0;

        // Ensure this points to your correct Entity Framework Context
        AttendanceSystemDBEntities5 db = new AttendanceSystemDBEntities5();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Cache control for real-time data
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.UtcNow.AddSeconds(-1));

            if (Session["EmployeeID"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadEmployeeDetails();
                LoadAttendanceGrid();
            }

            // SetButtonStatus is called on every load to ensure buttons are always enabled
            //SetButtonStatus();
        }

        private Employee GetEmployee(int empId) =>
            db.Employees.FirstOrDefault(e => e.EmployeeID == empId);

        private void LoadEmployeeDetails()
        {
            int empId = Convert.ToInt32(Session["EmployeeID"]);
            var emp = GetEmployee(empId);

            if (emp == null) return;

            lblFullName.Text = emp.FullName;
            lblEmployeeCode.Text = emp.EmployeeCode;
            lblPhone.Text = emp.Phone;

            /* -------------------------------------
                Attendance Date Calculation (Matches SP logic)
            ------------------------------------- */
            DateTime now = DateTime.Now;
            TimeSpan currentTime = now.TimeOfDay;

            var shift = db.Shifts.FirstOrDefault(s => s.ShiftID == emp.ShiftID);

            DateTime attendanceDate = DateTime.Today;

            if (shift != null && shift.EndTime < shift.StartTime)
            {
                // Night shift: if current time is before shift end (e.g., 6 AM), use yesterday's date
                if (currentTime < shift.EndTime)
                    attendanceDate = DateTime.Today.AddDays(-1);
            }

            lbldate.Text = attendanceDate.ToString("dd/MM/yyyy");

            /* Optional: Show shift name */
            if (shift != null)
                lblShift.Text = shift.ShiftName;
        }


        private void LoadAttendanceGrid()
        {
            int empId = Convert.ToInt32(Session["EmployeeID"]);

            // Reset totals before calculation
            grandTotalMinutes = 0;
            grandOTMinutes = 0;

            DateTime now = DateTime.Now;
            DateTime today = DateTime.Today;
            DateTime yesterday = today.AddDays(-1);

            var emp = GetEmployee(empId);
            if (emp == null || emp.ShiftID == null) return;

            var shift = db.Shifts.FirstOrDefault(s => s.ShiftID == emp.ShiftID);
            if (shift == null) return;

            bool isNightShift = shift.EndTime < shift.StartTime;

            /* ----------------------------------------
                Decide Attendance Date (Matches SP logic)
            ---------------------------------------- */
            DateTime attendanceDate = today;
            if (isNightShift && now.TimeOfDay < shift.EndTime)
                attendanceDate = yesterday;

            /* ----------------------------------------
                Get Attendance Header
            ---------------------------------------- */
            var attendance = db.AttendanceHeaders
                .FirstOrDefault(a =>
                    a.EmployeeID == empId &&
                    a.ShiftID == emp.ShiftID &&
                    DbFunctions.TruncateTime(a.AttendanceDate) == attendanceDate.Date); // Use TruncateTime for comparison

            if (attendance == null)
            {
                gvAttendance.DataSource = null;
                gvAttendance.DataBind();
                lblTotalhours.Text = "0 hr 0 min";
                lblOvertime.Text = "0 hr 0 min";
                return;
            }

            /* ----------------------------------------
                Get Punch Logs
            ---------------------------------------- */
            var logs = db.AttendanceLogs
                .Where(l => l.AttendanceID == attendance.AttendanceID)
                .OrderBy(l => l.PunchTime)
                .ToList();

            /* ----------------------------------------
                Pair IN / OUT & Calculate
            ---------------------------------------- */
            var gridData = new List<dynamic>();

            for (int i = 0; i < logs.Count; i++)
            {
                if (logs[i].PunchType == "IN")
                {
                    DateTime inTime = logs[i].PunchTime;
                    DateTime? outTime = null;

                    // Check for immediate next log being an OUT
                    if (i + 1 < logs.Count && logs[i + 1].PunchType == "OUT")
                    {
                        outTime = logs[i + 1].PunchTime;
                        i++; // skip next OUT log as it was paired
                    }

                    double minutes = 0;
                    double otMinutes = 0;

                    if (outTime != null)
                    {
                        minutes = (outTime.Value - inTime).TotalMinutes;
                        grandTotalMinutes += minutes;

                        if (minutes > 480) // 8 hours * 60 minutes
                        {
                            otMinutes = minutes - 480;
                            grandOTMinutes += otMinutes;
                        }
                    }

                    gridData.Add(new
                    {
                        InTime = inTime.ToString("hh:mm tt"),
                        OutTime = outTime?.ToString("hh:mm tt") ?? "Pending",
                        TotalHours = minutes > 0 ? FormatTime(minutes) : "-",
                        OvertimeHours = otMinutes > 0 ? FormatTime(otMinutes) : "-"
                    });
                }
                // Ignore any PunchType="OUT" that is not immediately preceded by an "IN"
            }

            gvAttendance.DataSource = gridData;
            gvAttendance.DataBind();

            lblTotalhours.Text = FormatTime(grandTotalMinutes);
            lblOvertime.Text = FormatTime(grandOTMinutes);
        }


        private string FormatTime(double totalMinutes)
        {
            if (totalMinutes < 0) totalMinutes = 0; // Prevent negative time display
            int hours = (int)(totalMinutes / 60);
            int minutes = (int)(totalMinutes % 60);
            return $"{hours} hr {minutes} min";
        }
       
        protected void btnPunchIn_Click(object sender, EventArgs e)
        {
            int empId = Convert.ToInt32(Session["EmployeeID"]);

            var emp = db.Employees.FirstOrDefault(ep => ep.EmployeeID == empId);
            if (emp == null || emp.ShiftID == null)
            {
                ShowAlert("error", "Oops...", "Shift not assigned.");
                return;
            }

            int shiftId = emp.ShiftID.Value;

            var resultParam = new SqlParameter("@ResultMessage", SqlDbType.VarChar, 200)
            {
                Direction = ParameterDirection.Output
            };

            try
            {
                db.Database.ExecuteSqlCommand(
                    @"EXEC dbo.Sp_punchInOut 
              @EmployeeID, 
              @ShiftID, 
              @PunchType, 
              @PunchTime, 
              @ResultMessage OUTPUT",
                    new SqlParameter("@EmployeeID", empId),
                    new SqlParameter("@ShiftID", shiftId),
                    new SqlParameter("@PunchType", "IN"),
                    new SqlParameter("@PunchTime", DateTime.Now),
                    resultParam
                );

                string message = resultParam.Value?.ToString() ?? "No response from server.";
                bool isSuccess = message.IndexOf("successful", StringComparison.OrdinalIgnoreCase) >= 0;

                ShowAlert(isSuccess ? "success" : "error",
                          isSuccess ? "Success" : "Oops...",
                          message);
            }
            catch (Exception ex)
            {
                ShowAlert("error", "Critical Error", ex.Message);
            }

            LoadAttendanceGrid();
        }
        private void ShowAlert(string icon, string title, string message)
        {
            string safeMessage = message.Replace("'", "\\'");
            ClientScript.RegisterStartupScript(
                this.GetType(),
                Guid.NewGuid().ToString(),
                $"Swal.fire({{ icon: '{icon}', title: '{title}', text: '{safeMessage}', timer: 2500, showConfirmButton: false }});",
                true
            );
        }

        protected void btnPunchOut_Click(object sender, EventArgs e)
        {
            int empId = Convert.ToInt32(Session["EmployeeID"]);

            var emp = db.Employees.FirstOrDefault(es => es.EmployeeID == empId);
            if (emp == null || emp.ShiftID == null)
            {
                ShowAlert("error", "Oops...", "Shift not assigned.");
                return;
            }

            int shiftId = emp.ShiftID.Value;

            var resultParam = new SqlParameter("@ResultMessage", SqlDbType.VarChar, 200)
            {
                Direction = ParameterDirection.Output
            };

            try
            {
                db.Database.ExecuteSqlCommand(
                    @"EXEC dbo.Sp_punchInOut 
              @EmployeeID, 
              @ShiftID, 
              @PunchType, 
              @PunchTime, 
              @ResultMessage OUTPUT",
                    new SqlParameter("@EmployeeID", empId),
                    new SqlParameter("@ShiftID", shiftId),
                    new SqlParameter("@PunchType", "OUT"),
                    new SqlParameter("@PunchTime", DateTime.Now),
                    resultParam
                );

                string message = resultParam.Value?.ToString() ?? "No response from server.";
                bool isSuccess = message.IndexOf("successful", StringComparison.OrdinalIgnoreCase) >= 0;

                ShowAlert(isSuccess ? "success" : "error",
                          isSuccess ? "Success" : "Oops...",
                          message);
            }
            catch (Exception ex)
            {
                ShowAlert("error", "Critical Error", ex.Message);
            }

            LoadAttendanceGrid();
        }

        // ----------------------------------------------------
        // CORE TRANSACTION METHOD (Calls SP, Displays Message)
        // ----------------------------------------------------
    //    private void HandlePunch(string punchType) // "IN" or "OUT"
    //    {
    //        int empId = Convert.ToInt32(Session["EmployeeID"]);

    //        var emp = db.Employees.FirstOrDefault(e => e.EmployeeID == empId);
    //        if (emp == null || emp.ShiftID == null)
    //        {
    //            ShowAlert("error", "Oops...", "Shift not assigned.");
    //            return;
    //        }

    //        int shiftId = emp.ShiftID.Value;

    //        var resultParam = new SqlParameter("@ResultMessage", SqlDbType.VarChar, 200)
    //        {
    //            Direction = ParameterDirection.Output
    //        };

    //        try
    //        {
    //            db.Database.ExecuteSqlCommand(
    //                @"EXEC dbo.Sp_punchInOut 
    //          @EmployeeID, 
    //          @ShiftID, 
    //          @PunchType, 
    //          @PunchTime, 
    //          @ResultMessage OUTPUT",
    //                new SqlParameter("@EmployeeID", empId),
    //                new SqlParameter("@ShiftID", shiftId),
    //                new SqlParameter("@PunchType", punchType),
    //                new SqlParameter("@PunchTime", DateTime.Now),
    //                resultParam
    //            );

    //            string message = resultParam.Value?.ToString() ?? "No response from server.";
    //            bool isSuccess =
    //message.IndexOf("successful", StringComparison.OrdinalIgnoreCase) >= 0;
    //            ShowAlert(
    //                isSuccess ? "success" : "error",
    //isSuccess ? "Success" : "Oops...",
    //message
    //            );
    //        }
    //        catch (Exception ex)
    //        {
    //            ShowAlert("error", "At this Punch Out is sCritical Error", ex.Message);
    //        }

    //        LoadAttendanceGrid();
            //SetButtonStatus();
        }


        // ----------------------------------------------------
        // BUTTON STATUS: Always Enabled (Per User Request)
        // ----------------------------------------------------
        // ----------------------------------------------------
        // BUTTON STATUS: Always Enabled (Per User Request)
        // ----------------------------------------------------
        //private void SetButtonStatus()
        //{
        //    btnPunchIn.Enabled = true;
        //    btnPunchOut.Enabled = true;

        //    int employeeId = Convert.ToInt32(Session["EmployeeID"]);
        //    var emp = db.Employees.FirstOrDefault(e => e.EmployeeID == employeeId);

        //    if (emp == null || emp.ShiftID == null)
        //    {
        //        btnPunchIn.Enabled = false;
        //        btnPunchOut.Enabled = false;
        //        return;
        //    }

        //    var shift = db.Shifts.FirstOrDefault(s => s.ShiftID == emp.ShiftID);
        //    if (shift == null)
        //    {
        //        btnPunchIn.Enabled = false;
        //        btnPunchOut.Enabled = false;
        //        return;
        //    }

        //    // OPTIONAL UX RULE:
        //    // Disable buttons only if it's far outside shift + 3 hrs
        //    DateTime now = DateTime.Now;

        //    DateTime shiftEnd = DateTime.Today.Add(shift.EndTime);
        //    if (shift.EndTime < shift.StartTime)
        //        shiftEnd = shiftEnd.AddDays(1); // night shift

        //    DateTime maxPunchOut = shiftEnd.AddHours(3);

        //    if (now > maxPunchOut)
        //    {
        //        btnPunchIn.Enabled = false;
        //        btnPunchOut.Enabled = false;
        //    }
        //}

    }
