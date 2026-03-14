using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Attendance
{
    public partial class Login : System.Web.UI.Page
    {
        AttendanceSystemDBEntities1 db = new AttendanceSystemDBEntities1();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session.Clear();
                Session.Abandon();

                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetNoStore();
                Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            }


            //string user = Session["EmployeeId"].ToString();

            //if (Session["EmployeeId"] != null)
            //{
            //    return;
            //}


            //if (!IsPostBack)
            //{

            //    txtUserName.Text = "";
            //    txtPhone.Text = "";
            //}
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        
        {
            string user = txtUserName.Text.Trim();
            string phone = txtPhone.Text.Trim();

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(phone))
            {
                lblMessage.Text = "Please enter the Username and Phone number";
                return;
            }

            var empdb = db.Employees.FirstOrDefault(x =>
                (x.EmployeeCode == user || x.UserName == user) &&
                 x.Phone == phone
            );

            if (empdb != null)
            {
                Session["EmployeeID"] = empdb.EmployeeID;
                Session["Fullname"] = empdb.FullName;
                Response.Redirect("AdminDashboard.aspx");
            }
            else
            {
                txtUserName.Text = "";
                txtPhone.Text = "";
                lblMessage.Text = "Invalid Employee Code / Username or Phone Number";
            }
        }

    }
}