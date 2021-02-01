<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RegistrationBak.aspx.cs" Inherits="BryanToh194937Y_ASAssignment.Registration" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
        tbody td:nth-child(1) {
            text-align: right;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table>
                <tr>
                    <td colspan="2" style="text-align: center;">
                        <h1>Account Registration</h1>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Label ID="lbl_userFeedback" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tbody>
                    <tr>
                        <td>First Name:</td>
                        <td>
                            <asp:TextBox ID="tb_firstName" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>Last Name:</td>
                        <td>
                            <asp:TextBox ID="tb_lastName" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                        <td>Credit Card Info</td>
                    </tr>
                    <tr>
                        <td>Credit Card Number:</td>
                        <td>
                            <asp:TextBox ID="tb_ccNo" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>Expiry:</td>
                        <td><asp:TextBox ID="tb_ccExpiry" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>CVV:</td>
                        <td><asp:TextBox ID="tb_ccCVV" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>Email Address:</td>
                        <td>
                            <asp:TextBox ID="tb_email" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>Password:</td>
                        <td><asp:TextBox ID="tb_password" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>Date of Birth (DOB):</td>
                        <td><asp:TextBox ID="tb_dob" runat="server" TextMode="Date"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td>
                            <asp:Button ID="btn_register" runat="server" Text="Register" OnClick="btn_register_Click" />
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </form>
</body>
</html>
