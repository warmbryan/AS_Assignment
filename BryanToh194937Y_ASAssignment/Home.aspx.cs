using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Configuration;
using System.Text;
using System.Data.SqlClient;
using System.Security.Cryptography;

using BryanToh194937Y_ASAssignment.App_Code.Utility;

namespace BryanToh194937Y_ASAssignment
{
    public partial class Home : System.Web.UI.Page
    {
        byte[] IV = null;
        byte[] Key = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!(Session["Email"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null))
            {
                Response.Redirect("~/Login.aspx", false);
                return;
            }

            if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
            {
                Response.Redirect("~/Login.aspx", false);
                return;
            }

            if (UserUtils.AccountAgeMinute(Session["Email"].ToString()) >= 15)
            {
                Response.Redirect("~/AccountSettings.aspx");
                return;
            }

            // obtain the credit card information and decrypt
            byte[] ccNo = null;
            byte[] ccExpiry = null;
            byte[] ccCVV = null;

            // t-sql query string
            string queryString = "SELECT [CCNo], [CCExpiry], [CCCVV], [IV], [Key] FROM dbo.[Users] WHERE Email = @Email;";
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString))
            {
                // Create the Command and Parameter objects.
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@Email", Session["Email"].ToString());

                // Open the connection in a try/catch block.
                // Create and execute the DataReader, writing the result
                // set to the console window.
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        IV = Convert.FromBase64String(reader["IV"].ToString());
                        Key = Convert.FromBase64String(reader["Key"].ToString());
                        ccNo = Convert.FromBase64String(reader["CCNo"].ToString());
                        ccExpiry = Convert.FromBase64String(reader["CCExpiry"].ToString());
                        ccCVV = Convert.FromBase64String(reader["CCCVV"].ToString());
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            lbl_ccNo.Text = decryptData(ccNo);
            lbl_ccExpiry.Text = decryptData(ccExpiry);
            lbl_ccCVV.Text = decryptData(ccCVV);
        }

        // https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.rijndael?view=netframework-4.7.2#examples
        protected string decryptData(byte[] cipherText)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            string plaintext = null;

            using (RijndaelManaged cipherAlgorithm = new RijndaelManaged())
            {
                cipherAlgorithm.IV = IV;
                cipherAlgorithm.Key = Key;
                ICryptoTransform decryptTransform = cipherAlgorithm.CreateDecryptor();

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptTransform, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
    }
}