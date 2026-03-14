using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Attendance
{
    public partial class ShiftAttendance : System.Web.UI.Page
    {
        double grandTotalMinutes = 0;
        double grandOTMinutes = 0;
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

            if (!IsPostBack)
            {

                LoadEmployeeDetails();
                LoadAttendanceGrid();
                SetButtonStatus();
                //CheckPunchStatus();
            }
        }
        //void CheckPunchStatus()
        //{
        //    string empCode = lblEmpCode.Text;
        //    var emp = db.Employees.SingleOrDefault(x => x.EmployeeCode == empCode);

        //    if (emp == null) return;

        //    var shift = db.shifts
        //    TimeSpan now = DateTime.Now.TimeOfDay;

        //    // Night Shift
        //    if (shift.ShiftID == 3)
        //    {
        //        // After midnight to shift end → allow OUT
        //        if (now < shift.EndTime)
        //        {
        //            btnPunchIn.Enabled = false;
        //            btnPunchOut.Enabled = true;
        //        }
        //        else
        //        {
        //            btnPunchIn.Enabled = true;
        //            btnPunchOut.Enabled = false;
        //        }
        //    }
        //}

        // ✅ Load Employee info
        private void LoadEmployeeDetails()
        {
            int empId = Convert.ToInt32(Session["EmployeeID"]);
            var emp = db.Employees.FirstOrDefault(e => e.EmployeeID == empId);

            if (emp != null)
            {
                lblFullName.Text = emp.FullName;
                lblEmployeeCode.Text = emp.EmployeeCode;
                //lblPhone.Text = emp.Phone;
                lbldate.Text = Convert.ToDateTime(emp.CreatedAt).ToString("MM/dd/yyyy");
                //lblOvertime.Text = null;
                //lblTotalhours.Text = null;

            }
        }

        // ✅ Load today's attendance records (MULTIPLE rows)
        private void LoadAttendanceGrid()
        {
            int empId = Convert.ToInt32(Session["EmployeeID"]);
            DateTime today = DateTime.Today;

            // Reset daily totals
            grandTotalMinutes = 0;
            grandOTMinutes = 0;

            var records = db.Attendances
                .Where(a => a.EmployeeID == empId &&
                            DbFunctions.TruncateTime(a.AttendanceDate) == today)
                .OrderBy(a => a.InTime)
                .ToList();

            var data = records.Select(a =>
            {
                double totalMinutes = 0;
                double otMinutes = 0;

                if (a.InTime != null && a.OutTime != null)
                {
                    TimeSpan diff = ((DateTime)a.OutTime) - ((DateTime)a.InTime);

                    totalMinutes = diff.TotalMinutes;
                    grandTotalMinutes += totalMinutes;

                    if (diff.TotalHours > 8)
                    {
                        otMinutes = (diff.TotalHours - 8) * 60;
                    }

                    grandOTMinutes += otMinutes;
                }

                return new
                {
                    InTime = a.InTime != null
                        ? ((DateTime)a.InTime).ToString("hh:mm tt")
                        : "-",

                    OutTime = a.OutTime != null
                        ? ((DateTime)a.OutTime).ToString("hh:mm tt")
                        : "-",

                    TotalHours = (a.InTime != null && a.OutTime != null)
                        ? FormatTime(totalMinutes)
                        : "-",

                    OvertimeHours = (a.InTime != null && a.OutTime != null)
                        ? FormatTime(otMinutes)
                        : "-"
                };
            }).ToList();

            gvAttendance.DataSource = data;
            gvAttendance.DataBind();

            // ✅ FINAL TOTAL IN LABELS
            lblTotalhours.Text = FormatTime(grandTotalMinutes);
            lblOvertime.Text = FormatTime(grandOTMinutes);
        }

        //private string GetTotalHours(DateTime inTime, DateTime outTime)
        //{
        //    TimeSpan diff = outTime - inTime;

        //    int hours = (int)diff.TotalHours;
        //    int minutes = diff.Minutes;

        //    return $"{hours} hr {minutes} min";
        //}
        private string FormatTime(double totalMinutes)
        {
            int hours = (int)(totalMinutes / 60);
            int minutes = (int)(totalMinutes % 60);

            return $"{hours} hr {minutes} min";
        }

        private string GetOverTime(DateTime inTime, DateTime outTime)
        {
            TimeSpan diff = outTime - inTime;

            if (diff.TotalHours > 8)
            {
                double ot = diff.TotalHours - 8;
                int hours = (int)ot;
                int minutes = (int)((ot - hours) * 60);

                return $"{hours} hr {minutes} min";
            }

            return "0 hr 0 min";
        }




        protected void btnPunchIn_Click(object sender, EventArgs e)
        {
            int empId = Convert.ToInt32(Session["EmployeeID"]);

            var resultParam = new System.Data.SqlClient.SqlParameter
            {
                ParameterName = "@ResultMessage",
                Direction = System.Data.ParameterDirection.Output,
                Size = 200
            };

            db.Database.ExecuteSqlCommand(
                "EXEC SP_PunchIn @EmployeeID, @ResultMessage OUTPUT",
                new System.Data.SqlClient.SqlParameter("@EmployeeID", empId),
                resultParam
            );

            string message = resultParam.Value?.ToString() ?? "Unknown error";
            var safeMessage = message.Replace("'", "\\'");

            if (message != "SUCCESS")
            {
                ClientScript.RegisterStartupScript(this.GetType(), "swal",
                    $"Swal.fire({{ icon: 'error', title: 'Oops...', text: '{safeMessage}', timer: 3000, showConfirmButton: false }});", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "swal",
                    "Swal.fire({ icon: 'success', title: 'Success', text: 'Punch IN Successful', timer: 2000, showConfirmButton: false });", true);
            }

            LoadAttendanceGrid();
            SetButtonStatus();
        }

        protected void btnPunchOut_Click(object sender, EventArgs e)
        {
            int empId = Convert.ToInt32(Session["EmployeeID"]);
            var emp = db.Employees.FirstOrDefault(ep => ep.EmployeeID == empId);
            string empCode = emp.EmployeeCode;

            var result = new System.Data.SqlClient.SqlParameter
            {
                ParameterName = "@ResultMessage",
                Direction = System.Data.ParameterDirection.Output,
                Size = 200
            };

            db.Database.ExecuteSqlCommand(
                "EXEC SP_PunchOut @EmployeeCode, @ResultMessage OUTPUT",
                new System.Data.SqlClient.SqlParameter("@EmployeeCode", empCode),
                result
            );

            string message = result.Value.ToString();

            var safeMessage = message.Replace("'", "\\'");

            if (message != "OUT SUCCESS")
            {
                ClientScript.RegisterStartupScript(this.GetType(), "swal",
                    $"Swal.fire({{ icon: 'error', title: 'Oops...', text: '{safeMessage}', timer: 3000, showConfirmButton: false }});", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "swal",
                    "Swal.fire({ icon: 'success', title: 'Success', text: 'Punch OUT Successful', timer: 2000, showConfirmButton: false });", true);
            }

            LoadAttendanceGrid();
            SetButtonStatus();
        }


        private void SetButtonStatus()
        {
            int empId = Convert.ToInt32(Session["EmployeeID"]);
            DateTime today = DateTime.Today;

            var last = db.Attendances
                         .Where(a => a.EmployeeID == empId && a.AttendanceDate == today)
                         .OrderByDescending(a => a.InTime)
                         .FirstOrDefault();

            if (last == null)
            {
                btnPunchIn.Enabled = true;
                btnPunchOut.Enabled = false;
            }
            else if (last.InTime != null && last.OutTime == null)
            {
                btnPunchIn.Enabled = false;
                btnPunchOut.Enabled = true;
            }
            else
            {
                btnPunchIn.Enabled = true;
                btnPunchOut.Enabled = false;
            }
        }

    }
}
