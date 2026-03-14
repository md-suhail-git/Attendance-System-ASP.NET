<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManualAttendance.aspx.cs"
    MasterPageFile="~/Dash.Master" Inherits="Attendance.ManualAttendance" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<style>
/* Base font */
body, .dashboard-container, input, select, button, .form-control {
    font-family: 'Poppins', sans-serif;
    background: #f4f6f9;
    margin: 0;
}

/* Container */
.dashboard-container {
    background: #ffffff;
    padding: 30px 25px;
    border-radius: 12px;
    box-shadow: 0 8px 20px rgba(0,0,0,0.08);
    max-width: 1200px;
    margin: 20px auto;
}

/* Page header */
h2 {
    font-weight: 700;
    color: #0072ff;
    margin-bottom: 30px;
    text-align: center;
}

/* Employee info */
.employee-info {
    display: flex;
    justify-content: space-between;
    align-items: center;
    font-size: 16px;
    margin-bottom: 25px;
    flex-wrap: wrap;
}

.employee-info span {
    font-weight: 500;
    margin-bottom: 8px;
}

/* Form Inputs Flex */
.attendance-info {
    display: flex;
    flex-wrap: wrap;
    gap: 15px;
    margin-bottom: 20px;
    align-items: flex-end;
}

.form-col {
    flex: 1 1 22%;
    min-width: 200px;
}

label {
    display: block;
    font-weight: 600;
    margin-bottom: 6px;
    color: #34495e;
}

.form-control {
    width: 100%;
    padding: 10px 14px;
    border-radius: 8px;
    border: 1px solid #dcdcdc;
    font-size: 14px;
    transition: all 0.3s ease;
}

.form-control:focus {
    border-color: #3498db;
    outline: none;
    box-shadow: 0 0 5px rgba(52,152,219,0.4);
}

/* Save Button */
.btn-save {
    width: 100%;
    border-radius: 8px;
    padding: 12px 20px;
    font-weight: 600;
    border: none;
    background: #2ecc71;
    color: #fff;
    font-family: 'Poppins', sans-serif;
    cursor: pointer;
    transition: all 0.3s ease;
}

.btn-save:hover {
    background: #27ae60;
    transform: translateY(-2px);
}

/* Message Label */
.fw-bold {
    font-weight: 600;
    display: block;
    margin-top: 15px;
    color: #2c3e50;
}

/* GridView Modern Style */
.table {
    width: 100%;
    border-collapse: collapse;
    margin-top: 20px;
    font-family: 'Poppins', sans-serif;
}

.table th, .table td {
    padding: 12px 10px;
    text-align: center;
    font-size: 14px;
}

.table th {
    background-color: #0072ff;
    color: #fff;
    font-weight: 600;
}

.table tr:nth-child(even) {
    background: #f8f9fa;
}

.table tr:hover {
    background: #e6f0ff;
}

/* Edit / Delete buttons */
.table a {
    padding: 6px 12px;
    border-radius: 6px;
    text-decoration: none;
    font-size: 13px;
    color: white;
    transition: all 0.3s ease;
}

.table a[href*="Edit"] {
    background: #3498db;

}

.table a[href*="Edit"]:hover {
    background: #1d6fa5;
}

.table a[href*="Delete"] {
    background: #e74c3c;
}

.table a[href*="Delete"]:hover {
    background: #c0392b;
}

/* Responsive */
@media only screen and (max-width: 768px) {
    .employee-info {
        flex-direction: column;
        align-items: flex-start;
    }

    .attendance-info {
        flex-direction: column;
    }

    .form-col {
        width: 100%;
    }
}
</style>



<h2>Manual Attendance</h2>

<div class="dashboard-container">
     <h2>Welcome,
        
         <asp:Label ID="lblFullName" runat="server" Text=""></asp:Label></h2>

 <div class="employee-info">
     <span>Employee Code:
            
             <asp:Label ID="lblEmployeeCode" runat="server" Text=""></asp:Label></span>
     <span>Phone:
            
             <asp:Label ID="lblPhone" runat="server" Text=""></asp:Label></span>
     <br />


 </div>
    <div class="attendance-info">
        <div class="form-col">
            <label>Date</label>
           <asp:TextBox ID="txtDate" runat="server" CssClass="form-control datepicker" />
        </div>

        <div class="form-col">
            <label>Time</label>
            <asp:TextBox ID="txtTime" runat="server" TextMode="Time" CssClass="form-control" />
        </div>

        <div class="form-col">
            <label>Type</label>
            <asp:DropDownList ID="ddlType" runat="server" CssClass="form-control">
                <asp:ListItem Text="-- Select --" Value="" />
                <asp:ListItem Text="IN" Value="IN" />
                <asp:ListItem Text="OUT" Value="OUT" />
            </asp:DropDownList>
        </div>

        <div class="form-col">
            <asp:Button ID="btnAdd" runat="server" Text="Save Attendance" CssClass="btn-save" OnClick="btnAdd_Click" />
        </div>
    </div>

    <asp:Label ID="lblMessage" runat="server" CssClass="fw-bold"></asp:Label>

    <hr />

    <!-- GridView untouched -->
    <asp:GridView
        ID="gvAttendance"
        runat="server"
        CssClass="table table-bordered text-center"
        AutoGenerateColumns="False"
        DataKeyNames="AttendanceID"
        OnRowEditing="gvAttendance_RowEditing"
        OnRowUpdating="gvAttendance_RowUpdating"
        OnRowCancelingEdit="gvAttendance_RowCancelingEdit"
        OnRowDeleting="gvAttendance_RowDeleting">

        <Columns>
            <asp:BoundField DataField="AttendanceDate"
                HeaderText="Date"
                DataFormatString="{0:yyyy-MM-dd}"
                ReadOnly="true" />

            <asp:TemplateField HeaderText="In Time">
                <ItemTemplate>
                    <%# Eval("InTime", "{0:hh:mm tt}") %>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtInTime" runat="server"
                        Text='<%# Bind("InTime","{0:HH:mm}") %>' TextMode="Time" />
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Out Time">
                <ItemTemplate>
                    <%# Eval("OutTime", "{0:hh:mm tt}") %>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtOutTime" runat="server"
                        Text='<%# Bind("OutTime","{0:HH:mm}") %>' TextMode="Time" />
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:CommandField HeaderText="Actions" ShowEditButton="true" ShowDeleteButton="true" />
        </Columns>
    </asp:GridView>

</div>

</asp:Content>

