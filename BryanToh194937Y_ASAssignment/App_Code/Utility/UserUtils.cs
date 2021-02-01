using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Text;
using System.Security.Cryptography;

using System.Configuration;

using System.Data;
using System.Data.SqlClient;

using System.Threading.Tasks;

namespace BryanToh194937Y_ASAssignment.App_Code.Utility
{
    public static class UserUtils
    {
        private static SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString);
        private static SqlCommand cmd = new SqlCommand();

        public static void RefreshConnection()
        {
            if (con.State == ConnectionState.Closed || con.State == ConnectionState.Broken)
                con.Open();

            cmd.Connection = con;
            cmd.Parameters.Clear();
            cmd.Dispose();
        }

        public static bool Exist(string email)
        {
            RefreshConnection();
            bool exist = false;

            try
            {
                using (SqlDataAdapter sda = new SqlDataAdapter("SELECT [Id] FROM [dbo].[Users] WHERE [Email] = @Email;", con))
                {
                    sda.SelectCommand.CommandType = CommandType.Text;
                    sda.SelectCommand.Parameters.AddWithValue("@Email", email);
                    DataSet ds = new DataSet();
                    sda.Fill(ds);
                    exist = (ds.Tables[0].Rows.Count > 0);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return exist;
        }

        public static bool Authenticate(string email, string password)
        {
            RefreshConnection();
            bool success;
            string pHash = null;
            string pSalt = null;
            cmd.CommandText = "SELECT [PasswordHash], [PasswordSalt] FROM dbo.[Users] WHERE [Email] = @Email;";
            cmd.Parameters.AddWithValue("@Email", email);
            
            try
            {
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    pHash = reader["PasswordHash"].ToString();
                    pSalt = reader["PasswordSalt"].ToString();
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                return false;
                // throw ex
            }

            success = Password.comparePasswordhash(Password.getPasswordHash(password, pSalt), pHash);

            return success;
        }

        public static bool AddFailedAuthAttempt(string email)
        {
            RefreshConnection();
            
            cmd.CommandText = "UPDATE [dbo].[Users] SET [FailedLogin] = [FailedLogin] + 1 WHERE Email = @Email and 3 > [FailedLogin];";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@Email", email);
            bool success = (cmd.ExecuteNonQuery() > 0);

            return success;
        }

        public static bool IsAccountDisabled(string email)
        {
            RefreshConnection();

            SqlDataAdapter sda = new SqlDataAdapter("SELECT [Id] FROM [dbo].[Users] WHERE [Email] = @Email and [FailedLogin] = 3;", con);
            sda.SelectCommand.CommandType = CommandType.Text;
            sda.SelectCommand.Parameters.AddWithValue("@Email", email);

            DataSet sd = new DataSet();
            sda.Fill(sd);

            return (sd.Tables[0].Rows.Count > 0);
        }

        public static bool UnlockAccount(string email)
        {
            RefreshConnection();

            cmd.CommandText = "UPDATE [dbo].[Users] SET FailedLogin = 0 WHERE Email = @Email;";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@Email", email);
            bool success = (cmd.ExecuteNonQuery() > 0);

            return success;
        }

        public static int AccountAgeMinute(string email)
        {
            RefreshConnection();

            cmd.CommandText = "SELECT DATEDIFF(minute, [LastPassDate], GETDATE()) TimeDiff FROM [dbo].[Users] WHERE Email = @Email;";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@Email", email);
            int minutes = 0;

            SqlDataReader sdr = cmd.ExecuteReader();

            if (sdr.Read())
                minutes = Convert.ToInt32(sdr["TimeDiff"]);

            return minutes;
        }
    }
}