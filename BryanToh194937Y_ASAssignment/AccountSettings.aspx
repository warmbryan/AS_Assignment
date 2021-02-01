<%@ Page Title="" Language="C#" MasterPageFile="~/Authenticated.Master" AutoEventWireup="true" CodeBehind="AccountSettings.aspx.cs" Inherits="BryanToh194937Y_ASAssignment.AccountSettings" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="style" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="form" runat="server">
	<h2>Account Settings</h2>
	<hr />
	<asp:Panel ID="panel1" runat="server" Visible="false">
		<div class="alert alert-danger" role="alert">
			<asp:Label ID="lbl_feedback" Text="text" runat="server" />
		</div>
	</asp:Panel>
	<div class="form-group">
		<label for="tb_fName">First Name</label>
		<asp:TextBox ID="tb_fName" runat="server" CssClass="form-control" ReadOnly="true"/>
	</div>

	<div class="form-group">
		<label for="tb_lName">Last Name</label>
		<asp:TextBox ID="tb_lName" runat="server" CssClass="form-control" ReadOnly="true"/>
	</div>

	<div class="form-group">
		<label for="tb_email">Email Address</label>
		<asp:TextBox ID="tb_email" runat="server" CssClass="form-control" ReadOnly="true"/>
	</div>

	<div class="form-group">
		<label>Current Password</label>
		<asp:TextBox ID="tb_curPassword" runat="server" TextMode="Password" CssClass="form-control" />
	</div>

	<div class="form-group">
		<label>New Password</label>
		<asp:TextBox ID="tb_newPassword" runat="server" TextMode="Password" CssClass="form-control" />
	</div>

	<div class="form-group">
		<label>Confirm New Password</label>
		<asp:TextBox ID="tb_confirmNewPassword" runat="server" TextMode="Password" CssClass="form-control" />
	</div>

	<div class="mt-3">
		<asp:Button ID="btn_update" Text="Update Account Details" runat="server" CssClass="btn btn-primary btn-block" OnClick="Change_Password" />
	</div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="scripts" runat="server">
</asp:Content>
