<%@ Page Title="Dashboard"
    Language="C#" AutoEventWireup="true"
    CodeBehind="AdminDashboard.aspx.cs"
    MasterPageFile="~/Dash.Master"
    Inherits="Attendance.AdminDashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <!-- Google Font -->
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;600&display=swap" rel="stylesheet"/>

    <style>
        body {
            font-family: 'Poppins', sans-serif;
            background: #f4f6f9;
            margin: 0;
            height: 100%;
        }

        .container {
            width: 100%;
            margin: auto;
        }

        .header {
            font-size: 26px;
            font-weight: 600;
            margin-bottom: 20px;
            color: #333;
        }

        .search-box {
            margin-bottom: 20px;
        }

        input[type=text] {
            padding: 10px;
            width: 250px;
            border: 1px solid #aaa;
            border-radius: 5px;
        }

        .btn {
            padding: 9px 15px;
            border: none;
            background: #007bff;
            color: white;
            border-radius: 5px;
            cursor: pointer;
            margin-left: 10px;
        }
        .lbl{
             font-size: 26px;
 font-weight: 600;
        }

        .btn:hover {
            background: #005dc1;
        }

        .gridview {
            width: 100%;
            border-collapse: collapse;
            margin-top: 15px;
        }

        .gridview th, .gridview td {
            padding: 10px;
            border: 1px solid #ddd;
            text-align: left;
        }

        .gridview th {
            background: #007bff;
            color: white;
        }

        .btn-small {
            padding: 6px 8px;
            font-size: 12px;
            border-radius: 5px;
        }

        .btn-view {
            background: #28a745;
        }

        .btn-view:hover {
            background: #1e7e34;
        }

        .btn-report {
            background: #ff9800;
        }

        .btn-report:hover {
            background: #d87c00;
        }
    </style>

    <div class="container">
        <!-- PAGE HEADER -->
        <div class="header">Admin Dashboard – Employee Attendance & Reports</div>

        <!-- Search Area -->
        <div class="search-box">
            <asp:TextBox ID="txtSearch" runat="server" Placeholder="Search name or Employee Code" OnTextChanged="txtSearch_TextChanged"></asp:TextBox>
            <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn" OnClick="btnSearch_Click" />
            <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btn" OnClick="btnReset_Click" />
        </div>
        <div class="lbl">
    <asp:Label ID="lblnoRcord" runat="server" 
        ForeColor="Red" 
        CssClass="message-label"
        Visible="false">
    </asp:Label>
</div>

        <!-- Employee Grid -->
        <asp:GridView ID="gvEmployees" runat="server" OnRowCommand="gvEmployees_RowCommand" AutoGenerateColumns="False" CssClass="gridview">
            <Columns>
                <asp:BoundField DataField="EmployeeCode" HeaderText="Employee Code" />
                <asp:BoundField DataField="FullName" HeaderText="Full Name" />
                <asp:BoundField DataField="Email" HeaderText="Email" />
                <asp:BoundField DataField="Phone" HeaderText="Phone" />

                <asp:TemplateField HeaderText="Actions">
                    <ItemTemplate>
                        <asp:Button ID="btnView" runat="server" Text="View Attendance" CssClass="btn-small btn-view"
                            CommandName="ViewAttendance" CommandArgument='<%# Eval("EmployeeID") %>' />

                        <asp:Button ID="btnGenerate" runat="server" Text="Generate Monthly Report" CssClass="btn-small btn-report"
                            CommandName="GenerateReport" CommandArgument='<%# Eval("EmployeeID") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

    </div>
</asp:Content>
