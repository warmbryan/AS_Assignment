<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="BryanToh194937Y_ASAssignment.Login" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="style" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="form" runat="server">
    <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response" />
    <div class="mx-auto" style="max-width: 403px;">
        <h2>Account Login</h2>
        <hr />
        <asp:Panel ID="panel1" runat="server" Visible="false">
            <div class="alert alert-danger" role="alert">
                <asp:Label ID="lbl_feedback" Text="text" runat="server" />
            </div>
        </asp:Panel>
        <div class="form-group">
            <label for="tb_email" class="form-label">Email Address</label>
            <asp:TextBox ID="tb_email" runat="server" TextMode="Email" CssClass="form-control"/>
        </div>

        <div class="form-group">
            <label for="tb_ln" class="form-label">Password</label>
            <asp:TextBox ID="tb_password" CssClass="form-control" runat="server" TextMode="Password"/>
        </div>

        <div class="">
            <asp:Button ID="btn_login" runat="server" Text="Sign In" CssClass="btn btn-primary btn-block" OnClick="LoginMe" />
        </div>

        <div class="mt-3">
            <label>Don't have an account?</label>
            <a href="/Register.aspx" type="button" class="btn btn-secondary btn-block">Register</a>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="scripts" runat="server">
    <script src="https://www.google.com/recaptcha/api.js?render=6LfBHxQaAAAAACSMGJ58QUx6uxHA9oj-AhOZEyrD"></script>
    <script>
        grecaptcha.ready(function () {
            grecaptcha.execute('6LfBHxQaAAAAACSMGJ58QUx6uxHA9oj-AhOZEyrD', { action: 'submit' }).then(function (token) {
                // Add your logic to submit to your backend server here.
                $("#g-recaptcha-response").val(token);
            });
        });
    </script>
</asp:Content>
