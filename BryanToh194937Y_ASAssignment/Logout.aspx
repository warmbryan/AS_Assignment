<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="Logout.aspx.cs" Inherits="BryanToh194937Y_ASAssignment.Logout" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="style" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="form" runat="server">
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="body" runat="server">
    <div class="container h-100">
        <div class="text-center">
        <h2>Successfully logged out, you may close this window.</h2>
        <p>You will be redirected to the login page in 5 seconds.</p>
    </div>
    </div>
    
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="scripts" runat="server">
    <script>
        setTimeout(function () {
            window.location.href = "/Login.aspx";
        }, 5000);
    </script>
</asp:Content>
