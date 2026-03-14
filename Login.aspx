<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Attendance.Login" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <script>

        const phoneInput = document.getElementById("<%= txtPhone.ClientID %>");
        const togglePhone = document.getElementById("togglePhone");

        togglePhone.addEventListener("click", () => {
            if (phoneInput.type === "password") {
                phoneInput.type = "text";
                togglePhone.style.color = "#0072ff";
            } else {
                phoneInput.type = "password";
                togglePhone.style.color = "#555";
            }
        });
    </script>
    <script type="text/javascript">
        function preventBack() {
            window.history.forward();
        }

        setTimeout(preventBack, 0);
        window.onunload = function () { null };
    </script>
    <script>
        function showLoader() {
            document.getElementById("loader").style.display = "flex";
        }
    </script>

    <title>Employee Login</title>

    <!-- Google Fonts -->
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@400;500;600&display=swap" rel="stylesheet">

    <style>
         /* Background Overlay */
    .loader-overlay {
        position: fixed;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background: rgba(255, 255, 255, 0.85);
        display: flex;
        justify-content: center;
        align-items: center;
        z-index: 9999;
        backdrop-filter: blur(3px);
    }

    /* Modern Blue Spinner */
    .loader {
        width: 70px;
        height: 70px;
        border: 6px solid #cce5ff;
        border-top-color: #007bff;
        border-radius: 50%;
        animation: spin 1s ease-in-out infinite;
        box-shadow: 0 0 15px rgba(0, 123, 255, 0.4);
    }

    @keyframes spin {
        to { transform: rotate(360deg); }
    }
        /* Body & Center Card */
        body {
            font-family: 'Poppins', sans-serif; /* Google font applied */
            background: linear-gradient(to right, #00c6ff, #0072ff);
            display: flex;
            justify-content: center;
            align-items: center;
            min-height: 100vh;
            margin: 0;
        }

        /* Card Container */
        .login-container {
            width: 90%;
            max-width: 400px;
            padding: 40px;
            background-color: #fff;
            border-radius: 16px;
            box-shadow: 0 10px 25px rgba(0, 0, 0, 0.2);
            text-align: center;
            position: relative;
        }

        /* Welcome Message */
        .welcome-header {
            color: #0072ff;
            font-size: 1.5em;
            margin-bottom: 5px;
            font-weight: 500;
        }

        .password-wrapper {
            position: relative;
            width: 100%;
        }

        .password-box {
            width: 100%;
            padding: 12px 40px 12px 12px;
            border-radius: 8px;
            border: 1px solid #ccc;
            box-sizing: border-box;
            font-family: 'Poppins', sans-serif;
        }

        .toggle-eye {
            position: absolute;
            right: 12px;
            top: 50%;
            transform: translateY(-50%);
            cursor: pointer;
            font-size: 18px;
            color: #555;
            user-select: none;
        }


        /* Heading */
        .login-container h2 {
            color: #333;
            margin-bottom: 30px;
            font-weight: 600;
        }

        /* Input Group */
        .input-group {
            position: relative;
            margin-bottom: 25px;
            text-align: left;
        }

            .input-group label {
                font-weight: 500;
                color: #555;
                display: block;
                margin-bottom: 8px;
            }

            /* Input Fields */
            .input-group input {
                width: 100%;
                padding: 12px 40px 12px 12px;
                border: 1px solid #ccc;
                border-radius: 8px;
                box-sizing: border-box;
                transition: all 0.3s ease;
                font-family: 'Poppins', sans-serif; /* Google font applied */
            }

                .input-group input:focus {
                    border-color: #0072ff;
                    outline: none;
                    box-shadow: 0 0 5px rgba(0, 114, 255, 0.5);
                }

        /* Button */
        .login-btn {
            width: 100%;
            padding: 14px;
            background: linear-gradient(to right, #0072ff, #00c6ff);
            color: white;
            border: none;
            border-radius: 8px;
            cursor: pointer;
            font-size: 16px;
            font-weight: 600;
            transition: all 0.3s ease;
            font-family: 'Poppins', sans-serif; /* Google font applied */
        }

            .login-btn:hover {
                transform: translateY(-2px);
                box-shadow: 0 5px 15px rgba(0, 114, 255, 0.4);
            }

        /* Message Label */
        .message-label {
            margin-top: 20px;
            display: block;
            font-weight: 500;
        }

        /* Forgot phone link */
        .forgot-phone {
            display: block;
            margin-top: 8px;
            font-size: 0.85em;
            color: #0072ff;
            text-decoration: none;
        }

        .forgot-phone:hover {
                text-decoration: underline;
        }
    </style>
</head>
<body>
    <div id="loader" class="loader-overlay" style="display:none;">
    <div class="loader"></div>
</div>
    <form id="form1" runat="server">
        <div class="login-container">
            <p class="welcome-header">Welcome to Acsys Biometric Web System</p>
            <h2>Employee Login</h2>

            <div class="input-group">
                <asp:Label ID="Label1" runat="server" Text="Employee Code / Username"></asp:Label>
                <asp:TextBox ID="txtUserName" runat="server"></asp:TextBox>
            </div>

            <div class="input-group">
                <asp:Label ID="Label2" runat="server" Text="Phone Number"></asp:Label>

                <div class="password-wrapper">
                    <asp:TextBox ID="txtPhone" runat="server" TextMode="Password" CssClass="password-box"></asp:TextBox>
                    <%-- <span id="togglePhone" class="toggle-eye">&#128065;</span>--%>
                </div>
            </div>

            <asp:Button ID="btnLogin" runat="server" Text="Login" OnClick="btnLogin_Click"  OnClientClick="showLoader();" CssClass="login-btn" />

            <asp:Label ID="lblMessage" runat="server" ForeColor="Red" CssClass="message-label"></asp:Label>
        </div>
    </form>
</body>
</html>
