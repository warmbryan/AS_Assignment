<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="BryanToh194937Y_ASAssignment.Register" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="CurPageStyle" ContentPlaceHolderID="style" runat="server">
</asp:Content>
<asp:Content ID="form" ContentPlaceHolderID="form" runat="server">
    <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response" />
    <div class="mx-auto" style="max-width: 403px;">
        <h2>Account Registration</h2>
        <hr />
        <asp:Label ID="lbl_userFeedback" Text="" runat="server" />
        <div class="form-group">
            <label for="tb_fn" class="form-label">First Name</label>
            <asp:TextBox ID="tb_firstName" CssClass="form-control" runat="server" required />
            <asp:Label ID="lbl_fbFName" Text="" runat="server" />
        </div>
        <div class="form-group">
            <label for="tb_ln" class="form-label">Last Name</label>
            <asp:TextBox ID="tb_lastName" CssClass="form-control" runat="server" required />
            <asp:Label ID="lbl_fbLName" Text="" runat="server" />
        </div>

        <div class="form-group">
            <label for="tb_email" class="form-label">Email Address</label>
            <asp:TextBox ID="tb_email" runat="server" TextMode="Email" CssClass="form-control" required/>
            <asp:Label ID="lbl_fbEmail" Text="" runat="server" />
        </div>

        <div class="form-group">
            <label for="tb_ln" class="form-label">Password</label>
            <asp:TextBox ID="tb_password" CssClass="form-control" runat="server" TextMode="Password" onkeyup="javascript:validate()" required />
            <asp:Label ID="lbl_fbPassword" Text="" runat="server" />
        </div>

        <div class="form-group">
            <label for="tb_dateOfBirth" class="form-label">Date of Birth</label>
            <asp:TextBox ID="tb_dob" runat="server" TextMode="Date" CssClass="form-control" required/>
            <asp:Label ID="lbl_fbDob" Text="" runat="server" />
        </div>

        <div>
            <h2>Credit Card Details</h2>
            <div class="form-group">
                <label for="tb_fn" class="form-label">Number</label>
                <asp:TextBox ID="tb_ccNo" CssClass="form-control" runat="server" required />
                <asp:Label ID="lbl_fbCcNo" Text="" runat="server" />
            </div>
            <div class="row">
                <div class="form-group col-6">
                    <label for="tb_ln" class="form-label">Expiry (MM/YY)</label>
                    <asp:TextBox ID="tb_ccExpiry" CssClass="form-control" runat="server" required />
                    <asp:Label ID="lbl_fbCcExpiry" Text="" runat="server" />
                </div>
                <div class="form-group col-6">
                    <label for="tb_ln" class="form-label">CVV / Security Code</label>
                    <asp:TextBox ID="tb_ccCVV" CssClass="form-control" runat="server" required />
                    <asp:Label ID="lbl_fbCcCCV" Text="" runat="server" />
                </div>
            </div>
        </div>
        <div class="">
            <asp:Button ID="btn_register" runat="server" Text="Register" CssClass="btn btn-primary btn-block" OnClick="RegisterMe" />
        </div>
    </div>
</asp:Content>
<asp:Content ContentPlaceHolderID="scripts" runat="server">
    <script src="https://www.google.com/recaptcha/api.js?render=6LfBHxQaAAAAACSMGJ58QUx6uxHA9oj-AhOZEyrD"></script>
    <script>
        grecaptcha.ready(function () {
            grecaptcha.execute('6LfBHxQaAAAAACSMGJ58QUx6uxHA9oj-AhOZEyrD', { action: 'submit' }).then(function (token) {
                // Add your logic to submit to your backend server here.
                $("#g-recaptcha-response").val(token);
            });
        });

        const passwordFieldElem = "<%=tb_password.ClientID%>", labelElem = "<%=lbl_fbPassword.ClientID%>";
    </script>
    <script src="/Public/js/validation.js"></script>
</asp:Content>
