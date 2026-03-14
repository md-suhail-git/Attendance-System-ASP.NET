using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Attendance
{
    public partial class EmployeeDashboard : System.Web.UI.Page
    {
        AttendanceSystemDBEntities1 db = new AttendanceSystemDBEntities1();

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.UtcNow.AddSeconds(-1));
            if (Session["EmployeeID"] == null)
            {
                Response.Redirect("Login.aspx");
                return; 
            }
            
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //Response.Cache.SetNoStore();
            //Response.Cache.SetExpires(DateTime.UtcNow.AddSeconds(-1));
            //Response.Redirect("Login.aspx");


            if (!IsPostBack)
            {

                LoadEmployeeDetails();
                LoadAttendanceGrid();
                SetButtonStatus();
            }
        }

        protected void SetButtonStatus()
        {
            if (Session["EmployeeID"] == null)
                return;

            int empId = Convert.ToInt32(Session["EmployeeID"]);
            DateTime today = DateTime.Today;

            var att = db.Attendances.FirstOrDefault(a =>
                 a.EmployeeID == empId &&
                 System.Data.Entity.DbFunctions.TruncateTime(a.AttendanceDate) == today);

            if (att == null)
            {
                btnPunchIn.Enabled = true;
                btnBreakIn.Enabled = false;
                btnBreakOut.Enabled = false;
                btnLunchIn.Enabled = false;
                btnLunchOut.Enabled = false;
                btnPunchOut.Enabled = false;
                btnFinalOut.Enabled = false;
                return;
            }

            btnPunchIn.Enabled = att.InTime == null;
            btnBreakIn.Enabled = att.InTime != null && att.BreakIn == null;
            btnBreakOut.Enabled = att.BreakIn != null && att.BreakOut == null;
            btnLunchIn.Enabled = att.BreakOut != null && att.LunchIn == null;
            btnLunchOut.Enabled = att.LunchIn != null && att.LunchOut == null;
            btnPunchOut.Enabled = att.LunchOut != null && att.OutTime == null;
            btnFinalOut.Enabled = att.OutTime != null && att.FinalOut == null;
        }
        protected void gvEmployees_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int empId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "ViewAttendance")
            {
                Response.Redirect($"EmployeeAttendance.aspx?EmpID={empId}");
            }

            if (e.CommandName == "GenerateReport")
            {
                Response.Redirect($"MonthlyReport.aspx?EmpID={empId}");
            }
        }

        private void LoadEmployeeDetails()
        {
            int empId = Convert.ToInt32(Session["EmployeeID"]);
            var emp = db.Employees.Find(empId);

            if (emp != null)
            {
                lblFullName.Text = emp.FullName;
                lblEmployeeCode.Text = emp.EmployeeCode;
                lblPhone.Text = emp.Phone;
            }
        }

        private void LoadAttendanceGrid()
        {
            if (Session["EmployeeID"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            int empId = Convert.ToInt32(Session["EmployeeID"]);
            DateTime today = DateTime.Today;

            var attendanceRaw = db.Attendances
                 .Where(a => a.EmployeeID == empId &&
                        System.Data.Entity.DbFunctions.TruncateTime(a.AttendanceDate) == today)
                 .ToList();
            // Step 2: Convert in C#
            var attendance = attendanceRaw.Select(a => new
            {
                a.AttendanceDate,
                a.InTime,
                a.BreakIn,
                a.BreakOut,
                a.LunchIn,
                a.LunchOut,
                a.OutTime,
                a.FinalOut,

                TotalHours = a.TotalHours.HasValue
                    ? ConvertDecimalHoursToHoursMinutes(a.TotalHours.Value)
                    : "0 hr 0 min",

                OvertimeHours = a.OvertimeHours.HasValue
                    ? ConvertDecimalHoursToHoursMinutes(a.OvertimeHours.Value)
                    : "0 hr 0 min"
            }).ToList();

            gvAttendance.DataSource = attendance;
            gvAttendance.DataBind();
        }


        private string ConvertDecimalHoursToHoursMinutes(decimal totalHours)
        {

            int hours = (int)Math.Floor(totalHours);
            int minutes = (int)Math.Round((totalHours - hours) * 60);
            return $"{hours} hr {minutes} min";
        }

        // Punch Button Click Events
        protected void btnPunchIn_Click(object sender, EventArgs e)
        {
            int empId = Convert.ToInt32(Session["EmployeeID"]);
            DateTime today = DateTime.Today;

            var attendance = db.Attendances.FirstOrDefault(a =>
                a.EmployeeID == empId &&
                System.Data.Entity.DbFunctions.TruncateTime(a.AttendanceDate) == today);


            if (attendance == null)
            {
                attendance = new Attendance
                {
                    EmployeeID = empId,
                    AttendanceDate = today,
                    InTime = DateTime.Now
                };
                db.Attendances.Add(attendance);
            }
            else
            {
                attendance.InTime = DateTime.Now;
            }

            db.SaveChanges();
            LoadAttendanceGrid();
            SetButtonStatus();
        }

        protected void btnBreakIn_Click(object sender, EventArgs e)
        {
            UpdateTodayAttendance(a => a.BreakIn = DateTime.Now);
        }

        protected void btnBreakOut_Click(object sender, EventArgs e)
        {
            UpdateTodayAttendance(a => a.BreakOut = DateTime.Now);
        }

        protected void btnLunchIn_Click(object sender, EventArgs e)
        {
            UpdateTodayAttendance(a => a.LunchIn = DateTime.Now);
        }

        protected void btnLunchOut_Click(object sender, EventArgs e)
        {
            UpdateTodayAttendance(a => a.LunchOut = DateTime.Now);
        }

        protected void btnPunchOut_Click(object sender, EventArgs e)
        {
            UpdateTodayAttendance(a => a.OutTime = DateTime.Now);
        }

        protected void btnFinalOut_Click(object sender, EventArgs e)
        {
            UpdateTodayAttendance(a => a.FinalOut = DateTime.Now);
        }
        private void UpdateTodayAttendance(Action<Attendance> updateAction)
        {
            int empId = Convert.ToInt32(Session["EmployeeID"]);
            DateTime today = DateTime.Today;

            var attendance = db.Attendances.FirstOrDefault(a =>
               a.EmployeeID == empId &&
               System.Data.Entity.DbFunctions.TruncateTime(a.AttendanceDate) == today);


            if (attendance == null)
            {
                attendance = new Attendance
                {
                    EmployeeID = empId,
                    AttendanceDate = today
                };
                db.Attendances.Add(attendance);
            }

            updateAction(attendance);
            CalculateHours(attendance);
            db.SaveChanges();
            LoadAttendanceGrid();
        }
        //protected void gvAttendance_RowDataBound(object sender, GridViewRowEventArgs e)
        //{
        //    if (e.Row.RowType == DataControlRowType.DataRow)
        //    {
        //        dynamic att = e.Row.DataItem;

        //        // Highlight the next required punch
        //        if (att.InTime == null)
        //            e.Row.Cells[1].BackColor = System.Drawing.Color.LightGreen;

        //        else if (att.BreakIn == null)
        //            e.Row.Cells[2].BackColor = System.Drawing.Color.LightGreen;

        //        else if (att.BreakOut == null)
        //            e.Row.Cells[3].BackColor = System.Drawing.Color.LightGreen;

        //        else if (att.LunchIn == null)
        //            e.Row.Cells[4].BackColor = System.Drawing.Color.LightGreen;

        //        else if (att.LunchOut == null)
        //            e.Row.Cells[5].BackColor = System.Drawing.Color.LightGreen;

        //        else if (att.OutTime == null)
        //            e.Row.Cells[6].BackColor = System.Drawing.Color.LightGreen;

        //        else if (att.FinalOut == null)
        //            e.Row.Cells[7].BackColor = System.Drawing.Color.LightGreen;
        //    }
        //}

        private void CalculateHours(Attendance attendance)
        {
            if (attendance.InTime == null || attendance.FinalOut == null)
                return;

            TimeSpan totalWorking = attendance.FinalOut.Value - attendance.InTime.Value;

            TimeSpan breakTime = TimeSpan.Zero;
            if (attendance.BreakIn != null && attendance.BreakOut != null)
                breakTime = attendance.BreakOut.Value - attendance.BreakIn.Value;

            TimeSpan lunchTime = TimeSpan.Zero;
            if (attendance.LunchIn != null && attendance.LunchOut != null)
                lunchTime = attendance.LunchOut.Value - attendance.LunchIn.Value;

            TimeSpan netHours = totalWorking - breakTime - lunchTime;

            attendance.TotalHours = Math.Round((decimal)netHours.TotalHours, 2);

            attendance.OvertimeHours = attendance.TotalHours > 9
                ? attendance.TotalHours - 9
                : 0;
        }
    

}
}

