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
        public static bool Exist(string email)
        {
            bool exist = false;

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString))
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
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return exist;
        }

        public static bool Authenticate(string email, string password)
        {
            bool success;
            string pHash = null;
            string pSalt = null;            
            
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT [PasswordHash], [PasswordSalt] FROM dbo.[Users] WHERE [Email] = @Email;", con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@Email", email);
                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            pHash = reader["PasswordHash"].ToString();
                            pSalt = reader["PasswordSalt"].ToString();
                        }
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            success = Password.ComparePasswordHash(Password.GetPasswordHash(password, pSalt), pHash);

            return success;
        }

        public static bool AddFailedAuthAttempt(string email)
        {
            bool success = false;

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE [dbo].[Users] SET [FailedLogin] = [FailedLogin] + 1 WHERE Email = @Email and 3 > [FailedLogin];", con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@Email", email);

                        con.Open();

                        success = (cmd.ExecuteNonQuery() > 0);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
            return success;
        }

        public static bool IsAccountDisabled(string email)
        {
            DataSet sd = new DataSet();

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter("SELECT [Id] FROM [dbo].[Users] WHERE [Email] = @Email and [FailedLogin] = 3;", con))
                    {
                        sda.SelectCommand.CommandType = CommandType.Text;
                        sda.SelectCommand.Parameters.AddWithValue("@Email", email);

                        sda.Fill(sd);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            
            return (sd.Tables[0].Rows.Count > 0);
        }

        public static bool UnlockAccount(string email)
        {
            bool success = false;

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE [dbo].[Users] SET FailedLogin = 0 WHERE Email = @Email;", con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@Email", email);
                        con.Open();

                        success = (cmd.ExecuteNonQuery() > 0);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return success;
        }

        public static int AccountAgeMinute(string email)
        {
            int minutes = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT DATEDIFF(minute, [LastPassDate], GETDATE()) TimeDiff FROM [dbo].[Users] WHERE Email = @Email;", con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@Email", email);
                        con.Open();

                        SqlDataReader sdr = cmd.ExecuteReader();

                        if (sdr.Read())
                            minutes = Convert.ToInt32(sdr["TimeDiff"]);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
            return minutes;
        }
    }
}