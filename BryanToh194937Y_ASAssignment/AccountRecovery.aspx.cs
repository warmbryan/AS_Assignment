using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

using System.Drawing;

using BryanToh194937Y_ASAssignment.App_Code.Utility;

namespace BryanToh194937Y_ASAssignment
{
	public partial class AccountRecovery : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
		}

		protected void btn_submit_Click(object sender, EventArgs e)
		{
			// validate fields
			if (!Validate_Fields())
				return;

			string email = tb_email.Text.Trim();
			string password = tb_password.Text.Trim();

			string input_fName = tb_fName.Text.Trim();
			string input_lName = tb_lName.Text.Trim();

			string input_ccCVV = tb_ccCVV.Text.Trim();

			string newPassword = tb_newPassword.Text.Trim();
			string confirmNewPassword = tb_confirmNewPassword.Text.Trim();

			if (!UserUtils.Exist(email))
			{
				showFeedback("Invalid email address.");
				return;
			}

			if (!UserUtils.Authenticate(email, password))
            {
				showFeedback("Sorry, with the information you've provided. We still can't verify that you're the account owner.");
				return;
			}

			string userId = null;

			string firstName = null, lastName = null;
			string cipherText = null;
			string iv = null;
			string key = null;

			string existPassSalt = null;
			string existPassHash = null;
			using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString))
			{
				using (SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[Users] WHERE Email = @Email", con))
				{
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.AddWithValue("@Email", email);

					if (con.State == ConnectionState.Closed || con.State == ConnectionState.Broken)
						con.Open();

					SqlDataReader sdr = cmd.ExecuteReader();
					if (sdr.Read())
					{
						userId = sdr["Id"].ToString();

						firstName = sdr["FirstName"].ToString();
						lastName = sdr["LastName"].ToString();

						existPassSalt = sdr["PasswordSalt"].ToString();
						existPassHash = sdr["PasswordHash"].ToString();

						cipherText = sdr["CCCVV"].ToString();
						iv = sdr["IV"].ToString();
						key = sdr["Key"].ToString();
					}
				}
			}
			string plainText = DataCrypt.Decrypt(cipherText, iv, key);
			if (!(plainText.Equals(input_ccCVV) && firstName.Equals(input_fName) && lastName.Equals(input_lName)))
			{
				showFeedback("Invalid details provided.");
				return;
			}

			if (Password.comparePasswordhash(Password.getPasswordHash(newPassword, existPassSalt), existPassHash))
            {
				showFeedback("Your new password cannot be a password you've used before.");
				return;

			}

			Password.updatePassword(userId, Convert.ToBase64String(Password.getPasswordHash(tb_newPassword.Text.Trim(), existPassSalt)));
			UserUtils.UnlockAccount(email);
			lbl_feedback.ForeColor = Color.Green;
			showFeedback("Password has been updated.");
		}

		protected bool Validate_Fields()
		{
			try
            {
				if (String.IsNullOrWhiteSpace(tb_email.Text))
				{
					showFeedback("Email field is empty.");
					throw new Exception("empty field");
				}

				if (String.IsNullOrWhiteSpace(tb_password.Text))
				{
					showFeedback("Password field is empty");
					throw new Exception("empty field");
				}

				if (String.IsNullOrWhiteSpace(tb_fName.Text))
                {
					showFeedback("First Name field is empty");
					throw new Exception("empty field");
                }

				if (String.IsNullOrWhiteSpace(tb_lName.Text))
                {
					showFeedback("Last Name field is empty");
					throw new Exception("empty field");
                }

				if (String.IsNullOrWhiteSpace(tb_ccCVV.Text))
                {
					showFeedback("CVV field is empty");
					throw new Exception("empty field");
                }

				if (String.IsNullOrWhiteSpace(tb_newPassword.Text) || String.IsNullOrWhiteSpace(tb_confirmNewPassword.Text))
                {
					showFeedback("One of the new password fields are empty.");
					throw new Exception("empty field");
				}

				if (!tb_newPassword.Text.Equals(tb_confirmNewPassword.Text))
                {
					showFeedback("New password fields does not match.");
					throw new Exception("empty field");
				}

				return true;
			}
			catch
            {
				return false;
            }
		}

		protected void showFeedback(string message)
		{
			panel1.Visible = true;
			lbl_feedback.Visible = true;
			lbl_feedback.Text = message.Trim();
		}

		protected void resetFeedback()
		{
			panel1.Visible = false;
			lbl_feedback.Visible = false;
			lbl_feedback.Text = String.Empty;
		}
	}
}