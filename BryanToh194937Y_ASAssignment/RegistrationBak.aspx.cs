using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Configuration;
using System.Data;
using System.Data.SqlClient;

using System.Security.Cryptography;

using System.Drawing;

namespace BryanToh194937Y_ASAssignment
{
    public partial class Registration : System.Web.UI.Page
    {
        string sqlServerConnString = ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btn_register_Click(object sender, EventArgs e)
        {
            lbl_userFeedback.Text = "";
            if (!accountExists(tb_email.Text.Trim()))
            {
                string query = "INSERT INTO UserInfo VALUES(@FirstName, @LastName, @CCNo, @CCExpiry, @CCCVV, @Email, @Password, @DateOfBirth)";
                using (SqlConnection con = new SqlConnection(sqlServerConnString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@FirstName", tb_firstName.Text.Trim());
                            cmd.Parameters.AddWithValue("@LastName", tb_lastName.Text.Trim());
                            cmd.Parameters.AddWithValue("@CCNo", tb_ccNo.Text.Trim());
                            cmd.Parameters.AddWithValue("@CCExpiry", tb_ccExpiry.Text.Trim());
                            cmd.Parameters.AddWithValue("@CCCVV", tb_ccCVV.Text.Trim());
                            cmd.Parameters.AddWithValue("@Email", tb_email.Text.Trim());
                            cmd.Parameters.AddWithValue("@Password", tb_password.Text.Trim());
                            cmd.Parameters.AddWithValue("@DateOfBirth", tb_dob.Text.Trim());
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }
                Response.Redirect("Login.aspx");
            } else
            {
                lbl_userFeedback.ForeColor = Color.Red;
                lbl_userFeedback.Text = "The email has been registered on this site";
            }
        }
 

        bool accountExists(string email)
        {
            bool exists = false;
            using (SqlConnection con = new SqlConnection(sqlServerConnString))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter("SELECT * FROM UserInfo WHERE Email = @Email", con))
                {
                    sda.SelectCommand.Parameters.AddWithValue("@Email", email);
                    DataSet ds = new DataSet();
                    sda.Fill(ds);
                    exists = Convert.ToBoolean(ds.Tables[0].Rows.Count);
                }
            }
            return exists;
        }
    }
}