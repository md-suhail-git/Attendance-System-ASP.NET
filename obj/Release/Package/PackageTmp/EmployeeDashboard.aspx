<%@ Page Language="C#" AutoEventWireup="true"  CodeBehind="EmployeeDashboard.aspx.cs" Inherits="Attendance.EmployeeDashboard" MasterPageFile="~/Dash.Master" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    

    <!-- Google Fonts -->
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@400;500;600&display=swap" rel="stylesheet">

    <style>
        body {
           font-family: 'Poppins', sans-serif;
background: #f4f6f9;
margin: 0;
height: 100%;
        }

        .dashboard-container {
            width: 100%;
 margin: auto;
        }

        h2 {
            text-align: center;
            color: #0072ff;
            margin-bottom: 30px;
        }

        .employee-info {
            margin-bottom: 30px;
            display: flex;
            justify-content: space-between;
            align-items: center;
            font-size: 16px;
        }

            .employee-info span {
                font-weight: 500;
            }

        .punch-buttons {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(160px, 1fr));
            gap: 15px;
            margin-top: 20px;
        }

        .punch-buttons .aspNetHidden,
        .punch-buttons button,
        .punch-buttons input[type=submit] {
            padding: 15px;
            border-radius: 12px;
            border: none;
            font-size: 15px;
            font-weight: 600;
            cursor: pointer;
            transition: 0.3s;
            box-shadow: 0 4px 6px rgba(0,0,0,0.1);
        }

        .btn-in {
            background: linear-gradient(135deg, #4CAF50, #2E7D32);
            color: white;
        }

        .btn-out {
            background: linear-gradient(135deg, #FF7043, #D84315);
            color: white;
        }

        .btn-break {
            background: linear-gradient(135deg, #FFEB3B, #FFC107);
            color: #000;
        }

        .btn-lunch {
            background: linear-gradient(135deg, #4FC3F7, #03A9F4);
            color: white;
        }

        .btn-final {
            background: linear-gradient(135deg, #90A4AE, #607D8B);
            color: white;
        }

        .punch-buttons button:hover {
            transform: translateY(-3px);
            opacity: 0.9;
        }

        .punch-buttons button:hover {
            transform: translateY(-3px);
            opacity: 0.9;
        }

        .attendance-info {
            margin-top: 30px;
        }

            .attendance-info table {
                width: 100%;
                border-collapse: collapse;
            }

            .attendance-info th, .attendance-info td {
                border: 1px solid #ddd;
                padding: 10px;
                text-align: center;
            }

            .attendance-info th {
                background-color: #0072ff;
                color: #fff;
            }
    </style>

        <div class="dashboard-container">
            <h2>Welcome,
                <asp:Label ID="lblFullName" runat="server" Text=""></asp:Label></h2>

            <div class="employee-info">
                <span>Employee Code:
                    <asp:Label ID="lblEmployeeCode" runat="server" Text=""></asp:Label></span>
                <span>Phone:
                    <asp:Label ID="lblPhone" runat="server" Text=""></asp:Label></span>
            </div>

            <div class="punch-buttons">
                <asp:Button ID="btnPunchIn" runat="server" Text="Punch IN" CssClass="btn-in" OnClick="btnPunchIn_Click" />
                <asp:Button ID="btnPunchOut" runat="server" Text="Punch OUT" CssClass="btn-out" OnClick="btnPunchOut_Click" />
                <asp:Button ID="btnBreakIn" runat="server" Text="Break IN" CssClass="btn-break" OnClick="btnBreakIn_Click" />
                <asp:Button ID="btnBreakOut" runat="server" Text="Break OUT" CssClass="btn-break" OnClick="btnBreakOut_Click" />
                <asp:Button ID="btnLunchIn" runat="server" Text="Lunch IN" CssClass="btn-lunch" OnClick="btnLunchIn_Click" />
                <asp:Button ID="btnLunchOut" runat="server" Text="Lunch OUT" CssClass="btn-lunch" OnClick="btnLunchOut_Click" />
                <asp:Button ID="btnFinalOut" runat="server" Text="Final OUT" CssClass="btn-final" OnClick="btnFinalOut_Click" />
            </div>

            <div class="attendance-info">
                <h3>Today’s Attendance</h3>
                <asp:GridView ID="gvAttendance" runat="server" AutoGenerateColumns="False" CssClass="gridview">
                    <Columns>
                        <asp:BoundField DataField="AttendanceDate" HeaderText="Date" DataFormatString="{0:dd-MMM-yyyy}" />
                        <asp:BoundField DataField="InTime" HeaderText="In Time" DataFormatString="{0:hh:mm tt}" />
                        <asp:BoundField DataField="BreakIn" HeaderText="Break IN" DataFormatString="{0:hh:mm tt}" />
                        <asp:BoundField DataField="BreakOut" HeaderText="Break OUT" DataFormatString="{0:hh:mm tt}" />
                        <asp:BoundField DataField="LunchIn" HeaderText="Lunch IN" DataFormatString="{0:hh:mm tt}" />
                        <asp:BoundField DataField="LunchOut" HeaderText="Lunch OUT" DataFormatString="{0:hh:mm tt}" />
                        <asp:BoundField DataField="OutTime" HeaderText="Punch OUT" DataFormatString="{0:hh:mm tt}" />
                        <asp:BoundField DataField="FinalOut" HeaderText="Final OUT" DataFormatString="{0:hh:mm tt}" />
                        <asp:BoundField DataField="TotalHours" HeaderText="Total Hours" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="OvertimeHours" HeaderText="Over time" DataFormatString="{0:N2}" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>
 </asp:content>

