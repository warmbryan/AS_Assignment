using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Security.Cryptography;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace BryanToh194937Y_ASAssignment.App_Code.Utility
{
    public static class Password
    {
        public static byte[] getPasswordHash(string password, string passwordSalt)
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

        public static bool comparePasswordhash(byte[] inputPasswordHash, string existingPasswordHash)
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
        public static bool updatePassword(string userId, string newPasswordHash)
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

        public static bool insertPasswordHistory(string userId, string passwordHash)
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
    }
}