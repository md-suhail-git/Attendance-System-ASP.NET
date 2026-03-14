<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NewReport.aspx.cs" Inherits="Attendance.NewReport" MasterPageFile="~/Dash.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <%-- <title>Employee Dashboard</title>--%>

    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@400;500;600&display=swap" rel="stylesheet">
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
<link href="https://cdnjs.cloudflare.com/ajax/libs/toastr.js/latest/toastr.min.css" rel="stylesheet" />
<script src="https://cdnjs.cloudflare.com/ajax/libs/toastr.js/latest/toastr.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
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
        /* ... (Existing button styles) ... */
        padding: 15px;
        border-radius: 12px;
        border: none;
        font-size: 15px;
        font-weight: 600;
        cursor: pointer;
        transition: 0.3s;
        box-shadow: 0 4px 6px rgba(0,0,0,0.1);
    }
            .punch-buttons input[type=submit]:disabled {
        /* Gray background and lighter text for disabled state */
        background: #ccc !important; /* Use !important to override the gradient colors */
        color: #777 !important;
        cursor: not-allowed;
        box-shadow: none; /* Remove shadow for a flat, disabled look */
        transform: none; /* Prevent any hover/active transformation */
        opacity: 0.6; /* Slight transparency for visual feedback */
        pointer-events: none; /* Prevents button interaction even if cursor hovers */
    }
            .punch-buttons button:hover:disabled,
    .punch-buttons input[type=submit]:disabled {
        transform: none;
        opacity: 0.6; 
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
            flex: 1;
            overflow-y: auto;
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
            <span>Shift:
             <asp:Label ID="lblShift" runat="server" Text=""></asp:Label></span>
            <br />


        </div>

        <div class="punch-buttons">
            <asp:Button ID="btnPunchIn" runat="server" Text="Punch IN" CssClass="btn-in" OnClick="btnPunchIn_Click" />
            <asp:Button ID="btnPunchOut" runat="server" Text="Punch OUT" CssClass="btn-out" OnClick="btnPunchOut_Click" />
        </div>
        <br />
        <div>
            <span>Date:
                    <asp:Label ID="lbldate" runat="server" Text=""></asp:Label></span>
        </div>

        <div class="attendance-info">
            <h3>Today’s Attendance</h3>
            <asp:GridView
                ID="gvAttendance"
                runat="server"
                CssClass="table table-bordered text-center"
                AutoGenerateColumns="False">

                <Columns>

                    <%--<asp:BoundField HeaderText="Employee Code" DataField="EmployeeCode" />--%>

                    <%--<asp:BoundField HeaderText="Attendance Date" DataField="AttendanceDate" 
                DataFormatString="{0:yyyy-MM-dd}" />--%>

                    <asp:BoundField HeaderText="In Time" DataField="InTime"
                        DataFormatString="{0:hh:mm tt}" />

                    <asp:BoundField HeaderText="Out Time" DataField="OutTime"
                        DataFormatString="{0:hh:mm tt}" />

                    <%-- <asp:BoundField HeaderText="Total Hours" DataField="TotalHours" />

                        <asp:BoundField HeaderText="Overtime Hours" DataField="OvertimeHours" />--%>
                </Columns>

            </asp:GridView>
        </div>
        <br />
        <div>
            <span>Total Hours:
        <asp:Label ID="lblTotalhours" runat="server" Text=""></asp:Label></span>

            <br />
            <span>Over Time:
        <asp:Label ID="lblOvertime" runat="server" Text=""></asp:Label></span>
        </div>

    </div>

</asp:Content>
