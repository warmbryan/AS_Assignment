using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;

using System.Configuration;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;

using System.Text.RegularExpressions;

using System.Net;
using System.IO;
using System.Web.Script.Serialization;

using BryanToh194937Y_ASAssignment.App_Code.Utility;

namespace BryanToh194937Y_ASAssignment
{
    public partial class Register : System.Web.UI.Page
    {
        private string serverConnString = ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        private string pHash;
        private string pSalt;
        byte[] IV;
        byte[] Key;

        protected void Page_Load(object sender, EventArgs e) {}

        protected void RegisterMe(object sender, EventArgs e)
        {
            lbl_userFeedback.Text = "";
            if (ValidateCaptcha())
            {
                if (ValidateInput())
                    return;

                if (UserUtils.Exist(tb_email.Text.Trim()))
                {
                    lbl_userFeedback.ForeColor = Color.Red;
                    lbl_userFeedback.Text = "The email has been registered on this site";
                    return;
                }

                // hash password
                HashPassword();

                // generate new iv and key for encryption in the next step
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.GenerateKey();
                Key = cipher.Key;
                IV = cipher.IV;

                DateTime dt = DateTime.Parse(tb_dob.Text.Trim());

                string query = "INSERT INTO dbo.[Users] ([FirstName], [LastName], [CCNo], [CCExpiry], [CCCVV], [Email], [PasswordHash], [PasswordSalt], [DateOfBirth], [IV], [Key]) VALUES (@FirstName, @LastName, @CCNo, @CCExpiry, @CCCVV, @Email, @PasswordHash, @PasswordSalt, @DateOfBirth, @IV, @Key);";
                using (SqlConnection con = new SqlConnection(serverConnString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@FirstName", tb_firstName.Text.Trim());
                            cmd.Parameters.AddWithValue("@LastName", tb_lastName.Text.Trim());
                            cmd.Parameters.AddWithValue("@CCNo", Convert.ToBase64String(encryptData(tb_ccNo.Text.Trim())));
                            cmd.Parameters.AddWithValue("@CCExpiry", Convert.ToBase64String(encryptData(tb_ccExpiry.Text.Trim())));
                            cmd.Parameters.AddWithValue("@CCCVV", Convert.ToBase64String(encryptData(tb_ccCVV.Text.Trim())));
                            cmd.Parameters.AddWithValue("@Email", tb_email.Text.Trim());
                            cmd.Parameters.AddWithValue("@PasswordHash", pHash);
                            cmd.Parameters.AddWithValue("@PasswordSalt", pSalt);
                            cmd.Parameters.AddWithValue("@DateOfBirth", dt.ToString("yyyy-MM-dd"));
                            cmd.Parameters.AddWithValue("@IV", Convert.ToBase64String(IV));
                            cmd.Parameters.AddWithValue("@Key", Convert.ToBase64String(Key));
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();

                            Response.Redirect("Login.aspx", false);
                        }
                    }
                }
            }
            else
            {
                lbl_userFeedback.ForeColor = Color.Red;
                lbl_userFeedback.Text += "Captcha challenge not solved<br>";
                return;
            }
        }

        protected void HashPassword()
        {
            string password = tb_password.Text.Trim();
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] bSalt = new byte[8];

            rng.GetBytes(bSalt);
            pSalt = Convert.ToBase64String(bSalt);

            SHA512Managed hasher = new SHA512Managed();
            string pwdWithSalt = password + pSalt;

            // to store in for password history
            byte[] plainHash = hasher.ComputeHash(Encoding.UTF8.GetBytes(password));

            byte[] hashWithSalt = hasher.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));

            pHash = Convert.ToBase64String(hashWithSalt);
        }

        protected byte[] encryptData(string data)
        {
            byte[] ciphertext = null;
            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                ICryptoTransform cryptoTransform = cipher.CreateEncryptor();

                byte[] plaintext = Encoding.UTF8.GetBytes(data);
                ciphertext = cryptoTransform.TransformFinalBlock(plaintext, 0, plaintext.Length);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
            return ciphertext;
        }

        // validates registration textboxes
        protected bool ValidateInput()
        {
            int invalid = 0;
            if (tb_firstName.Text.Trim() == String.Empty)
            {
                lbl_userFeedback.Text += "First name is empty<br>";
                invalid++;
            }

            if (tb_lastName.Text.Trim() == String.Empty)
            {
                lbl_userFeedback.Text += "Last name is empty<br>";
                invalid++;
            }

            if (tb_ccNo.Text.Trim() == String.Empty)
            {
                lbl_userFeedback.Text += "Credit Card Number is empty<br>";
                invalid++;
            }

            if (tb_ccExpiry.Text.Trim() == String.Empty)
            {
                lbl_userFeedback.Text += "Credit Card Expiry is empty<br>";
                invalid++;
            }

            if (tb_ccCVV.Text.Trim() == String.Empty)
            {
                lbl_userFeedback.Text += "Credit Card security code is empty<br>";
                invalid++;
            }

            if (tb_email.Text.Trim() == String.Empty)
            {
                lbl_userFeedback.Text += "Email is empty<br>";
                invalid++;
            }
            
            if (tb_password.Text.Trim() == String.Empty)
            {
                lbl_userFeedback.Text += "Password is empty<br>";
                invalid++;
            }

            if (tb_dob.Text.Trim() == String.Empty)
            {
                lbl_userFeedback.Text += "Date of Birth is empty<br>";
                invalid++;
            }

            // password checker
            invalid += Password.TestPasswordStrength(tb_password.Text.Trim(), lbl_fbPassword, lbl_userFeedback);

            return Convert.ToBoolean(invalid);
        }

        protected bool ValidateCaptcha()
        {
            bool result = false;

            string captchaResponse = Request.Form["g-recaptcha-response"];

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://www.google.com/recaptcha/api/siteverify?secret=6LfBHxQaAAAAAJMjwemONlNFvySwx-Mizj--VEqr&response=" + captchaResponse);

            try
            {
                using (WebResponse wRes = req.GetResponse())
                {
                    using (StreamReader sRdr = new StreamReader(wRes.GetResponseStream()))
                    {
                        string jsonRes = sRdr.ReadToEnd();
                        // lbl_userFeedback.Text = jsonRes.ToString();

                        JavaScriptSerializer js = new JavaScriptSerializer();

                        MyObject jsonObj = js.Deserialize<MyObject>(jsonRes);

                        result = Convert.ToBoolean(jsonObj.success);
                    }
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }

            return result;
        }
    }

    public class MyObject
    {
        public string success { get; set; }

        public List<string> ErrorMessage { get; set; }
    }
}