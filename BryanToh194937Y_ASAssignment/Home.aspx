<%@ Page Title="" Language="C#" MasterPageFile="~/Authenticated.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="BryanToh194937Y_ASAssignment.Home" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="style" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="form" runat="server">
    <h2>Your credit card details</h2>
    <hr />
    <table>
        <tr>
            <td>Number:</td>
            <td>
                <asp:Label ID="lbl_ccNo" Text="" runat="server" />
            </td>
        </tr>
        <tr>
            <td>Expiry:</td>
            <td>
                <asp:Label ID="lbl_ccExpiry" Text="" runat="server" />
            </td>
        </tr>
        <tr>
            <td>CVV / Security Code:</td>
            <td>
                <asp:Label ID="lbl_ccCVV" Text="" runat="server" />
            </td>
        </tr>
    </table>
</asp:Content>
