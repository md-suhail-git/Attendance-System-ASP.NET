<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EmployeeAttendance.aspx.cs" MasterPageFile="~/Dash.Master" Inherits="Attendance.EmployeeAttendance" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="preconnect" href="https://fonts.googleapis.com">
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@400;600&display=swap" rel="stylesheet">

    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>

    <script src="https://code.jquery.com/ui/1.13.2/jquery-ui.min.js"></script>
    <link rel="stylesheet"
        href="https://code.jquery.com/ui/1.13.2/themes/base/jquery-ui.css" />

  <%--  <script>
        $(function () {
            $(".datepicker").datepicker({
                dateFormat: "yy-mm-dd",
                changeMonth: true,
                changeYear: true,
                showButtonPanel: true
            });
        });
    </script>--%>

    <%--<title>Employee Attendance History</title>--%>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        /* Apply Poppins to the whole body */
   body {
    font-family: 'Poppins', sans-serif;
    background: #f4f6f9;
    margin: 0;
}

/* Fix container inside Dash.Master */
.container {
    width: 100% !important;
    max-width: 100% !important;
    padding: 10px 20px;
}

/* Card style like NewReport */
.card {
    border-radius: 12px;
    border: none;
}

.card-header {
    font-size: 1.3rem;
    font-weight: 600;
    background: linear-gradient(120deg, #0d6efd, #6610f2);
    color: #fff;
    border-radius: 12px 12px 0 0;
}

/* Header title */
h4 {
    font-weight: 600;
}

/* Textboxes and inputs */
.form-control {
    border-radius: 8px;
    padding: 10px;
    border: 1px solid #dee2e6;
    transition: .3s;
}

.form-control:focus {
    border-color: #0072ff;
    box-shadow: 0 0 0 .15rem rgba(0,123,255,.25);
}

/* Grid Styling */
.table-attendance {
    border-radius: 10px;
    overflow: hidden;
}

.table-attendance th {
    background: #0072ff !important;
    color: #fff;
    text-align: center;
    font-weight: 600;
    padding: 12px;
}

.table-attendance td {
    text-align: center;
    padding: 10px;
}

/* Zebra effect */
.table-striped tbody tr:nth-of-type(odd) {
    background-color: #f8f9fa;
}

/* Buttons */
.btn {
    border-radius: 8px;
    padding: 10px 15px;
    font-weight: 500;
}

.btn-primary {
    background: linear-gradient(120deg, #0072ff, #00c6ff);
    border: none;
}

.btn-success {
    background: linear-gradient(120deg, #198754, #28a745);
    border: none;
}

.btn-export-pdf {
    background: linear-gradient(120deg, #dc3545, #ff5f6d);
    border: none;
    color: #fff;
}

/* Label message */
#lblMessage {
    font-weight: 600;
    padding: 10px;
}

/* Responsive Fix for small screen */
@media (max-width: 768px) {
    .row > div {
        margin-bottom: 10px;
    }
}
 }
    </style>


    <div class="container-fluid">

        <div class="card shadow-sm mb-4">
            <div class="card-header text-center py-3">
                📊 Employee Attendance History
            </div>
        </div>

        <div class="card shadow-sm mb-4">
            <div class="card-body">

                <div class="row align-items-end g-3">

                    <div class="col-lg-3 col-md-6">
                        <label class="form-label fw-bold text-muted">Employee (Name/Code)</label>
                        <asp:TextBox ID="txtEmployee" runat="server"
                            CssClass="form-control"
                            placeholder="All employees"></asp:TextBox>
                    </div>

                    <div class="col-lg-3 col-md-6">
                        <label class="form-label fw-bold text-muted">From Date</label>
                        <asp:TextBox ID="txtFrom" runat="server"
                            CssClass="form-control datepicker"
                            placeholder="Select start date"></asp:TextBox>
                    </div>

                    <div class="col-lg-3 col-md-6">
                        <label class="form-label fw-bold text-muted">To Date</label>
                        <asp:TextBox ID="txtTo" runat="server"
                            CssClass="form-control datepicker"
                            placeholder="Select end date"></asp:TextBox>
                    </div>

                    <div class="col-lg-3 col-md-6">
                        <asp:LinkButton ID="Button1" runat="server"
                            CssClass="btn btn-primary w-100"
                            OnClick="btnFilter_Click">

    
    <i class="bi bi-search me-2"></i> Apply Filter
    
</asp:LinkButton>

                    </div>
                </div>

            </div>
            <center>
                <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label></center>
        </div>

        <div class="d-flex justify-content-between align-items-center mb-3">

            <h4 class="mb-0 text-secondary">Attendance Records</h4>

            <div class="d-flex gap-2">
                <asp:Button ID="btnExportPdf" runat="server"
                    Text="Export PDF"
                    CssClass="btn btn-export-pdf"
                    OnClick="btnExportPdf_Click" />

                <asp:Button ID="btnExportExcel" runat="server"
                    Text="Export Excel"
                    CssClass="btn btn-success"
                    OnClick="btnExportExcel_Click" />
            </div>
        </div>

        <div class="card shadow-sm">
            <div class="card-body p-0">
                <div class="table-responsive">
                    <asp:GridView ID="gvAttendance" runat="server" AutoGenerateColumns="False"
                        CssClass="table table-striped table-hover table-attendance mb-0" GridLines="None">

                        <Columns>
                            <asp:BoundField DataField="AttendanceDate" HeaderText="Date" DataFormatString="{0:dd-MM-yyyy}" />
                            <asp:BoundField DataField="InTime" HeaderText="In Time" DataFormatString="{0:hh:mm tt}" />
                            <asp:BoundField DataField="BreakIn" HeaderText="Break In" DataFormatString="{0:hh:mm tt}" />
                            <asp:BoundField DataField="BreakOut" HeaderText="Break Out" DataFormatString="{0:hh:mm tt}" />
                            <asp:BoundField DataField="LunchIn" HeaderText="Lunch In" DataFormatString="{0:hh:mm tt}" />
                            <asp:BoundField DataField="LunchOut" HeaderText="Lunch Out" DataFormatString="{0:hh:mm tt}" />
                            <asp:BoundField DataField="OutTime" HeaderText="Out Time" DataFormatString="{0:hh:mm tt}" />
                            <asp:BoundField DataField="FinalOut" HeaderText="Final Out" DataFormatString="{0:hh:mm tt}" />
                            <asp:BoundField DataField="TotalHours" HeaderText="Total Worked" />
                            <asp:BoundField DataField="OvertimeHours" HeaderText="Overtime" />
                        </Columns>
                    </asp:GridView>
                </div>
            </div>

            <asp:Label ID="lblNoRecords" runat="server" Text="No attendance records found for the selected filter criteria."
                Visible="false" CssClass="alert alert-info text-center m-4"></asp:Label>
        </div>

    </div>
     <script>
         $(function () {
             $(".datepicker").datepicker({
                 dateFormat: "yy-mm-dd",
                 changeMonth: true,
                 changeYear: true,
                 showButtonPanel: true
             });
         });
     </script>

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css">
</asp:Content>
