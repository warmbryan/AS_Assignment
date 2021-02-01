using System;

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
        }

        protected void Change_Password(object sender, EventArgs e)
        {
            // validate inputs
            if (!ValidateFields())
                return;

            if (UserUtils.AccountAgeMinute(Session["Email"].ToString()) <= 5)
            {
                showFeedback("You may not change your password again in an interval of 5 minutes or less.");
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
                if (Password.comparePasswordhash(Password.getPasswordHash(password, pSalt), pHash))
                {
                    // get string hash of the new password to check and change if there are no existance of it
                    pHashNew = Convert.ToBase64String(Password.getPasswordHash(newPassword, pSalt));

                    bool passwordHistory = false;

                    // check if it's the same as current password
                    if (pHash.Equals(pHashNew))
                    {
                        lbl_feedback.Text = "New password must not be the same as the current password.";
                        return;
                    }

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
                        lbl_feedback.Text = "Previously 2 old passwords cannot be used.";
                        return;
                    }

                    Password.updatePassword(userId, pHashNew);
                    Password.insertPasswordHistory(userId, pHash);
                    lbl_feedback.Text = "Password has been updated.";
                    lbl_feedback.ForeColor = Color.Green;
                }
                else
                {
                    lbl_feedback.Text = "Current password is invalid, please try again.";
                    return;
                }
            }
        }

        protected bool ValidateFields()
        {
            if (String.IsNullOrEmpty(tb_curPassword.Text))
            {
                lbl_feedback.Text = "Current password field must not be empty.";
                return false;
            }

            if (String.IsNullOrEmpty(tb_newPassword.Text))
            {
                lbl_feedback.Text = "New password field must not be empty.";
                return false;
            }

            if (!tb_newPassword.Text.Trim().Equals(tb_confirmNewPassword.Text.Trim()))
            {
                lbl_feedback.Text = "New password fields must be the same.";
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