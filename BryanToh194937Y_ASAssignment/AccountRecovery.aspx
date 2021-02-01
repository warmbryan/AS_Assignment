<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="AccountRecovery.aspx.cs" Inherits="BryanToh194937Y_ASAssignment.AccountRecovery" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="style" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="form" runat="server">
	<input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response" />
    <div class="mx-auto" style="max-width: 403px;">
        <h2>Self Account Recovery</h2>
		<hr />
        <asp:Panel ID="panel1" runat="server" Visible="false">
            <div class="alert alert-secondary" role="alert">
                <asp:Label ID="lbl_feedback" Text="text" runat="server" />
            </div>
        </asp:Panel>
        <h3>Account Credentials</h3>
		<div class="form-group">
			<label for="tb_email">Email address</label>
			<asp:TextBox ID="tb_email" runat="server" CssClass="form-control" TextMode="Email" required/>
		</div>

		<div class="form-group">
			<label for="tb_password">Last known password</label>
			<asp:TextBox ID="tb_password" runat="server" CssClass="form-control" TextMode="Password" minlength="8" required/>
		</div>

        <h3>Account Details</h3>
        <p>This helps us to know if you are the genuine owner of the account.</p>
		<div class="form-group">
			<label for="tb_fName">First Name</label>
			<asp:TextBox ID="tb_fName" runat="server" CssClass="form-control" placeholder="First Name" required/>
		</div>

		<div class="form-group">
			<label for="tb_lName">Last Name</label>
			<asp:TextBox ID="tb_lName" runat="server" CssClass="form-control" placeholder="Last Name" required/>
		</div>

		<div class="form-group">
			<label for="tb_ccCVV">Last 3 digit of the credit card used during registration.</label>
			<asp:TextBox ID="tb_ccCVV" runat="server" CssClass="form-control" placeholder="CVV" required/>
		</div>

        <h3>New Password</h3>
		<div class="form-group">
			<label for="tb_newPassword">Password</label>
			<asp:TextBox ID="tb_newPassword" runat="server" CssClass="form-control" TextMode="Password" required/>
		</div>

		<div class="form-group">
			<label for="tb_confirmNewPassword">New Password</label>
			<asp:TextBox ID="tb_confirmNewPassword" runat="server" CssClass="form-control" TextMode="Password" required/>
		</div>

        <asp:Button ID="btn_submit" Text="Submit" runat="server" CssClass="btn btn-block btn-primary" OnClick="btn_submit_Click" />
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
