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

                if (Accounts.AccountExists(tb_email.Text.Trim()))
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
            {
                int scores = checkPassword(tb_password.Text);
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
                    lbl_fbPassword.Text = status;
                    lbl_fbPassword.ForeColor = Color.Red;

                    lbl_userFeedback.Text += "Password strength is " + status + "<br>" +
                        "Password should have at least 8 characters, " +
                        "1 lower capital, 1 upper capital, 1 number, and 1 special character";

                    invalid++;
                }
            }

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

        private int checkPassword(string password)
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

    public class MyObject
    {
        public string success { get; set; }

        public List<string> ErrorMessage { get; set; }
    }

    public class Accounts
    {
        public static bool AccountExists(string email)
        {
            bool exists = false;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter("SELECT * FROM Users WHERE Email = @Email", con))
                {
                    sda.SelectCommand.Parameters.AddWithValue("@Email", email.Trim());
                    DataSet ds = new DataSet();
                    sda.Fill(ds);
                    exists = Convert.ToBoolean(ds.Tables[0].Rows.Count);
                }
            }
            return exists;
        }
    }
}