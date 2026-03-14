using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace Attendance
{
    public partial class EmployeeAttendance
    {
        public int Id { get; set; }
        public string EmployeeName { get; set; }
        public DateTime Date { get; set; }
        public string InTime { get; set; }
        public string OutTime { get; set; }

        protected void btnExportPdf_Click(object sender, EventArgs e)
        {
            try
            {
                if (gvAttendance.Rows.Count == 0)
                {
                    Response.Write("<script>alert('No data available to export');</script>");
                    return;
                }

                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 10f);
                MemoryStream ms = new MemoryStream();
                PdfWriter.GetInstance(pdfDoc, ms);
                pdfDoc.Open();

                Paragraph p = new Paragraph("Attendance Report");
                p.Alignment = Element.ALIGN_CENTER;
                p.SpacingAfter = 10f;
                pdfDoc.Add(p);

                PdfPTable table = new PdfPTable(gvAttendance.HeaderRow.Cells.Count);
                table.WidthPercentage = 100;
                table.SpacingBefore = 10f;

                // Header
                foreach (TableCell cell in gvAttendance.HeaderRow.Cells)
                {
                    PdfPCell pdfcell = new PdfPCell(new Phrase(cell.Text));
                    pdfcell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(pdfcell);
                }

                // Data rows
                foreach (GridViewRow row in gvAttendance.Rows)
                {
                    foreach (TableCell cell in row.Cells)
                    {
                        table.AddCell(cell.Text);
                    }
                }

                pdfDoc.Add(table);
                pdfDoc.Close();

                byte[] bytes = ms.ToArray();
                ms.Close();


                string empName = txtEmployee.Text.Trim();

                if (string.IsNullOrEmpty(empName))
                {
                    empName = "All_Employees";
                }


                empName = empName.Replace(" ", "_");

                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", $"attachment;filename={empName}_AttendanceReport.pdf");
                Response.Buffer = true;
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.BinaryWrite(bytes);
                Response.End();
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }
        protected void btnExportExcel_Click(object sender, EventArgs e)

        {
            try
            {
                if (gvAttendance.Rows.Count == 0)
                {
                    Response.Write("<script>alert('No data available to export');</script>");
                    return;
                }

                string empName = txtEmployee.Text.Trim();

                if (string.IsNullOrEmpty(empName))
                {
                    empName = "All_Employees";
                }

                empName = empName.Replace(" ", "_");

                Response.Clear();
                Response.Buffer = true;
                Response.ClearContent();
                Response.ClearHeaders();

                Response.AddHeader("content-disposition",
                    $"attachment; filename={empName}_AttendanceReport.xls");

                Response.ContentType = "application/vnd.ms-excel";

                StringWriter sw = new StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(sw);

                gvAttendance.AllowPaging = false;
                gvAttendance.DataBind();

                // Change header style
                gvAttendance.HeaderRow.Style.Add("background-color", "#4472C4");
                gvAttendance.HeaderRow.Style.Add("color", "white");

                // Render GridView
                gvAttendance.RenderControl(hw);

                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }

    }
    public partial class EmployeeAttendance : System.Web.UI.Page
    {
        AttendanceSystemDBEntities1 db = new AttendanceSystemDBEntities1();

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.UtcNow.AddSeconds(-1));
            if (!IsPostBack)
            {
                if (Request.QueryString["empId"] != null)
                {
                    int empId = Convert.ToInt32(Request.QueryString["empId"]);
                    LoadEmployeeDetails(empId);
                    LoadEmployeeAttendance(empId);

                }
            }
        }

        private void LoadEmployeeDetails(int empId)
        {
            var empd = db.Employees.FirstOrDefault(e => e.EmployeeID == empId);
            if (empd != null)
            {
                txtEmployee.Text = empd.EmployeeCode;

            }
        }

        private void LoadEmployeeAttendance(int empId)
        {
            var empd = db.Attendances.Where(a => a.EmployeeID == empId).
                OrderByDescending(a => a.AttendanceDate).ToList();
            gvAttendance.DataSource = empd;
            gvAttendance.DataBind();

        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            // Clear old messages
            lblMessage.Text = "";

            // VALIDATION
            if (string.IsNullOrWhiteSpace(txtFrom.Text) || string.IsNullOrWhiteSpace(txtTo.Text))
            {
                lblMessage.Text = "⚠ From Date and To Date are mandatory.";
                return;
            }

            DateTime from, to;

            if (!DateTime.TryParse(txtFrom.Text, out from) || !DateTime.TryParse(txtTo.Text, out to))
            {
                lblMessage.Text = "⚠ Invalid date format.";
                return;
            }

            if (to < from)
            {
                lblMessage.Text = "⚠ To Date cannot be less than From Date.";
                return;
            }

            string name = txtEmployee.Text.Trim();

            var query = db.Attendances
                          .Where(a => a.AttendanceDate >= from && a.AttendanceDate <= to);

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(a => a.Employee.FullName.Contains(name)
                                      || a.Employee.EmployeeCode.Contains(name));
            }

            var result = query
                .OrderBy(a => a.EmployeeID)
                .ThenBy(a => a.AttendanceDate)
                .ToList();

            gvAttendance.DataSource = result;
            gvAttendance.DataBind();
        }



    }
}