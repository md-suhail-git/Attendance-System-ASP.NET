using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Attendance
{
    public partial class AdminDashboard : System.Web.UI.Page
    {
        AttendanceSystemDBEntities1 db = new AttendanceSystemDBEntities1();
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                LoadEMployee();
            }
        }

        private void LoadEMployee()
        {
            var list =db.Employees.OrderBy(e=>e.EmployeeCode).ToList(); 
            gvEmployees.DataSource = list;
            gvEmployees.DataBind(); 
        }
        protected void gvEmployees_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ViewAttendance")
            {
                int empId = Convert.ToInt32(e.CommandArgument);
                Response.Redirect($"EmployeeAttendance.aspx?empId={empId}");
            }
            else if (e.CommandName == "GenerateReport")
            {
                int empId = Convert.ToInt32(e.CommandArgument);
                Response.Redirect($"EmployeeAttendance.aspx?empId={empId}");
            }
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string search = txtSearch.Text.Trim();

            // 1. Check empty first
            if (string.IsNullOrWhiteSpace(search))
            {
                gvEmployees.DataSource = null;
                gvEmployees.DataBind();

                lblnoRcord.Text = "Please enter a name or employee code";
                lblnoRcord.Visible = true;
                return;
            }

            // 2. Search
            var list = db.Employees
                         .Where(x => x.FullName.Contains(search) || x.EmployeeCode.Contains(search))
                         .ToList();

            // 3. Result handling
            if (list.Count > 0)
            {
                gvEmployees.DataSource = list;
                gvEmployees.DataBind();

                lblnoRcord.Text = "";
                lblnoRcord.Visible = false;
            }
            else
            {
                gvEmployees.DataSource = null;
                gvEmployees.DataBind();

                lblnoRcord.Text = "No matching records found";
                lblnoRcord.Visible = true;
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            LoadEMployee();

        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {

        }

       
    }
}