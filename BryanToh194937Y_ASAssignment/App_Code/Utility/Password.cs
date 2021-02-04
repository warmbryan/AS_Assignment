using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;

namespace BryanToh194937Y_ASAssignment.App_Code.Utility
{
    public static class Password
    {
        // hashes plaintext password into salted hash
        public static byte[] GetPasswordHash(string password, string passwordSalt)
        {
            byte[] hash = null;
            try
            {
                SHA512Managed hasher = new SHA512Managed();
                string pwdWithHash = password + passwordSalt;
                hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(pwdWithHash));

            }
            catch (Exception ex)
            {
                // throw ex;
            }
            return hash;
        }

        public static bool ComparePasswordHash(byte[] inputPasswordHash, string existingPasswordHash)
        {
            bool success = false;

            try
            {
                if (Convert.ToBase64String(inputPasswordHash).Equals(existingPasswordHash))
                {
                    success = true;
                }
            }
            catch (Exception ex)
            {
                // throw ex;
            }

            return success;
        }

        // updates the user password
        public static bool UpdatePassword(string userId, string newPasswordHash)
        {
            bool success = false;

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("UPDATE [dbo].[Users] SET PasswordHash = @PHash, LastPassDate = getdate() WHERE Id = @UserId;", con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@PHash", newPasswordHash);

                    con.Open();
                    success = (cmd.ExecuteNonQuery() > 0);
                }
            }

            return success;
        }

        public static bool SavePasswordHashToHistory(string userId, string passwordHash)
        {
            bool success = false;

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("INSERT INTO [dbo].[PasswordHistory] ([Hash], [UserId]) VALUES (@Hash, @UserId);", con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Hash", passwordHash);
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    con.Open();
                    success = (cmd.ExecuteNonQuery() > 0);
                }
            }

            return success;
        }

        public static int TestPasswordStrength(string password, Label passwordLabel, Label feedbackLabel)
        {
            int scores = checkPassword(password);
            string status = "";
            switch (scores)
            {
                case 1:
                    status = "Very Weak";
                    break;
                case 2:
                    status = "Weak";
                    break;
                case 3:
                    status = "Medium";
                    break;
                case 4:
                    status = "Strong";
                    break;
                case 5:
                    status = "Excellent";
                    break;
                default:
                    break;
            }

            if (scores < 4)
            {
                passwordLabel.Text = status;
                passwordLabel.ForeColor = Color.Red;

                feedbackLabel.Text += "Password strength is " + status + "<br>" +
                    "Password should have at least 8 characters, " +
                    "1 lower capital, 1 upper capital, 1 number, and 1 special character";
            }

            return (scores < 4) ? 1 : 0;
        }

        public static int TestPasswordStrength(string password, Label passwordLabel)
        {
            int scores = checkPassword(password);
            string status = "";
            switch (scores)
            {
                case 1:
                    status = "Very Weak";
                    break;
                case 2:
                    status = "Weak";
                    break;
                case 3:
                    status = "Medium";
                    break;
                case 4:
                    status = "Strong";
                    break;
                case 5:
                    status = "Excellent";
                    break;
                default:
                    break;
            }

            if (scores < 4)
            {
                passwordLabel.Text = status;
                passwordLabel.ForeColor = Color.Red;
            }

            return (scores < 4) ? 1 : 0;
        }

        private static int checkPassword(string password)
        {
            int score = 0;

            // score 1
            if (password.Length < 8)
            {
                return 1;
            }
            else
            {
                score = 1;
            }

            // score 2 weak
            if (Regex.IsMatch(password, "[a-z]"))
            {
                score++;
            }

            // score 3 medium
            if (Regex.IsMatch(password, "[A-Z]"))
            {
                score++;
            }

            // score 4 strong
            if (Regex.IsMatch(password, "[0-9]"))
            {
                score++;
            }

            // score 5 excellent
            if (Regex.IsMatch(password, "[^A-Za-z0-9]"))
            {
                score++;
            }

            return score;
        }
    }
}