using System;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace Attendance
{
    public partial class ManualAttendance : System.Web.UI.Page
    {
        AttendanceSystemDBEntities1 db = new AttendanceSystemDBEntities1();

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.UtcNow.AddSeconds(-1));

            if (Session["EmployeeID"] == null)
                Response.Redirect("Login.aspx");

            if (!IsPostBack)
            {
                LoadGrid();
                LoadEmployeeDetails();
            }
        }

        
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtDate.Text) ||
                string.IsNullOrEmpty(txtDate.Text) ||
                ddlType.SelectedValue == "")
            {
                lblMessage.Text = "Please fill all fields.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            int empId = Convert.ToInt32(Session["EmployeeID"]);
            DateTime selectedDate = Convert.ToDateTime(txtDate.Text);
            DateTime selectedTime = Convert.ToDateTime(txtDate.Text + " " + txtTime.Text);

            var record = db.Attendances
                .FirstOrDefault(x => x.EmployeeID == empId && x.AttendanceDate == selectedDate);

            if (record == null)
            {
                record = new Attendance
                {
                    EmployeeID = empId,
                    AttendanceDate = selectedDate
                };

                if (ddlType.SelectedValue == "IN")
                    record.InTime = selectedTime;
                else
                    record.OutTime = selectedTime;

                db.Attendances.Add(record);
            }
            else
            {
                if (ddlType.SelectedValue == "IN")
                    record.InTime = selectedTime;
                else
                    record.OutTime = selectedTime;
            }

            db.SaveChanges();

            lblMessage.Text = "Attendance saved successfully ✅";
            lblMessage.ForeColor = System.Drawing.Color.Green;

            LoadGrid();
            ClearFields();
        }
        private void LoadEmployeeDetails()
        {
            int empId = Convert.ToInt32(Session["EmployeeID"]);
            var emp = db.Employees.FirstOrDefault(e => e.EmployeeID == empId);

            if (emp != null)
            {
                lblFullName.Text = emp.FullName;
                lblEmployeeCode.Text = emp.EmployeeCode;
                lblPhone.Text = emp.Phone;
                txtDate.Text = DateTime.Today.ToString("MM/dd/yyyy");
                //lblOvertime.Text = null;
                //lblTotalhours.Text = null;

            }
        }
        // ================= LOAD GRID ==================
        void LoadGrid()
        {
            int empId = Convert.ToInt32(Session["EmployeeID"]);

            var data = db.Attendances
                .Where(x => x.EmployeeID == empId)
                .OrderByDescending(x => x.AttendanceDate)
                .Select(x => new
                {
                    x.AttendanceID,
                    x.AttendanceDate,
                    x.InTime,
                    x.OutTime
                })
                .ToList();

            gvAttendance.DataSource = data;
            gvAttendance.DataBind();
        }

        // ================= EDIT ==================
        protected void gvAttendance_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvAttendance.EditIndex = e.NewEditIndex;
            LoadGrid();
        }

        protected void gvAttendance_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvAttendance.EditIndex = -1;
            LoadGrid();
        }

        protected void gvAttendance_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int id = Convert.ToInt32(gvAttendance.DataKeys[e.RowIndex].Value);
            var record = db.Attendances.FirstOrDefault(x => x.AttendanceID == id);

            TextBox txtIn = (TextBox)gvAttendance.Rows[e.RowIndex].FindControl("txtInTime");
            TextBox txtOut = (TextBox)gvAttendance.Rows[e.RowIndex].FindControl("txtOutTime");

            if (txtIn != null && !string.IsNullOrEmpty(txtIn.Text))
            {
                DateTime inTime = DateTime.Parse(record.AttendanceDate.ToString("yyyy-MM-dd") + " " + txtIn.Text);
                record.InTime = inTime;
            }

            if (txtOut != null && !string.IsNullOrEmpty(txtOut.Text))
            {
                DateTime outTime = DateTime.Parse(record.AttendanceDate.ToString("yyyy-MM-dd") + " " + txtOut.Text);
                record.OutTime = outTime;
            }

            db.SaveChanges();

            gvAttendance.EditIndex = -1;
            LoadGrid();
        }

        // ================= DELETE ==================
        protected void gvAttendance_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int id = Convert.ToInt32(gvAttendance.DataKeys[e.RowIndex].Value);
            var record = db.Attendances.FirstOrDefault(x => x.AttendanceID == id);

            if (record != null)
            {
                db.Attendances.Remove(record);
                db.SaveChanges();
            }

            LoadGrid();
        }

        void ClearFields()
        {
            txtDate.Text = "";
            txtTime.Text = "";
            ddlType.SelectedIndex = 0;
        }
    }
}
