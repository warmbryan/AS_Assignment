﻿using System;

using System.Data;
using System.Data.SqlClient;

using System.Drawing;

using System.Configuration;

using BryanToh194937Y_ASAssignment.App_Code.Utility;

namespace BryanToh194937Y_ASAssignment
{
    public partial class AccountSettings : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!(Session["Email"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null))
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT [FirstName], [LastName], [Email] FROM Users WHERE Email = @Email", con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@Email", Session["Email"].ToString());
                        con.Open();
                        SqlDataReader user = cmd.ExecuteReader();
                        if (user != null)
                        {
                            user.Read();
                            tb_email.Text = user["Email"].ToString();
                            tb_fName.Text = user["FirstName"].ToString();
                            tb_lName.Text = user["LastName"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (UserUtils.AccountAgeMinute(Session["Email"].ToString()) >= 15)
            {
                showFeedback("You need to change your password after 15 minutes.");
            }
        }

        protected void Change_Password(object sender, EventArgs e)
        {
            // validate inputs
            if (!ValidateFields())
                return;

            if (UserUtils.AccountAgeMinute(Session["Email"].ToString()) <= 5)
            {
                showFeedback("You have previously changed your password, you may reset again after 5 minutes after previous reset.");
                return;
            }

            string email = Session["email"].ToString();
            string password = tb_curPassword.Text.Trim();
            string newPassword = tb_newPassword.Text.Trim();

            string pHash = null;
            string pSalt = null;
            string userId = null;

            string pHashNew = null;

            string queryString = "SELECT * FROM dbo.[Users] WHERE [Email] = @Email;";
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString))
            {
                // Create the Command and Parameter objects.
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@Email", email);

                // Open the connection in a try/catch block.
                // Create and execute the DataReader, writing the result
                // set to the console window.
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        pHash = reader["PasswordHash"].ToString();
                        pSalt = reader["PasswordSalt"].ToString();
                        userId = reader["Id"].ToString();
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            // ensure
            if (pHash != null && pSalt != null)
            {
                // ensure authentication before authorizing
                if (Password.ComparePasswordHash(Password.GetPasswordHash(password, pSalt), pHash))
                {
                    // get string hash of the new password to check and change if there are no existance of it
                    pHashNew = Convert.ToBase64String(Password.GetPasswordHash(newPassword, pSalt));

                    bool passwordHistory = false;

                    // checks in password history if password has been used before
                    // https://docs.microsoft.com/en-us/sql/t-sql/queries/select-order-by-clause-transact-sql?view=sql-server-ver15#a-specifying-integer-constants-for-offset-and-fetch-values
                    string qStr = "SELECT [Hash] FROM [dbo].[PasswordHistory] WHERE UserId = @UserId and Hash = @Hash ORDER BY CreatedOn DESC OFFSET 0 ROW FETCH first 2 ROWS ONLY;";
                    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter(qStr, con))
                        {
                            sda.SelectCommand.CommandType = CommandType.Text;
                            sda.SelectCommand.Parameters.AddWithValue("@UserId", userId);
                            sda.SelectCommand.Parameters.AddWithValue("@Hash", pHashNew);

                            DataSet da = new DataSet();
                            sda.Fill(da);
                            passwordHistory = (da.Tables[0].Rows.Count > 0);
                        }
                    }

                    if (passwordHistory)
                    {
                        showFeedback("Previously 2 old passwords cannot be used.");
                        return;
                    }

                    Password.UpdatePassword(userId, pHashNew);
                    Password.SavePasswordHashToHistory(userId, pHash);
                    showFeedback("Password has been updated.");
                    lbl_feedback.ForeColor = Color.Green;
                }
                else
                {
                    showFeedback("Current password is invalid, please try again.");
                    return;
                }
            }
        }

        protected bool ValidateFields()
        {
            if (String.IsNullOrEmpty(tb_curPassword.Text))
            {
                showFeedback("Current password field must not be empty.");
                return false;
            }

            if (String.IsNullOrEmpty(tb_newPassword.Text))
            {
                showFeedback("New password field must not be empty.");
                return false;
            }

            if (!tb_newPassword.Text.Trim().Equals(tb_confirmNewPassword.Text.Trim()))
            {
                showFeedback("New password fields must be the same.");
                return false;
            }

            if (tb_curPassword.Text.Trim().Equals(tb_newPassword.Text.Trim()))
            {
                showFeedback("New password should not be the same as your current one.");
                return false;
            }

            if (Password.TestPasswordStrength(tb_newPassword.Text.Trim(), lbl_fbNewPassword) != 0)
            {
                showFeedback(
                    "Password should have at least 8 characters, " +
                    "1 lower capital, 1 upper capital, 1 number, and 1 special character"
                    );
                return false;
            }

            return true;
        }

        protected void showFeedback(string message)
        {
            panel1.Visible = true;
            lbl_feedback.Visible = true;
            lbl_feedback.Text = message.Trim();
        }
    }
}