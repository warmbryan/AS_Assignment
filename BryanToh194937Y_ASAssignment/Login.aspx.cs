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

using System.Threading.Tasks;

using BryanToh194937Y_ASAssignment.App_Code.Utility;

namespace BryanToh194937Y_ASAssignment
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e) { }

        protected void LoginMe(object sender, EventArgs e)
        {
            resetFeedback();

            string email = tb_email.Text.Trim();
            string pwd = tb_password.Text.Trim();

            if (!UserUtils.Exist(email))
            {
                showFeedback("Invalid email or password. Try again.");
                return;
            }

            if (UserUtils.IsAccountDisabled(email))
            {
                showFeedback("Account is disabled.");
                return;
            }

            if (!UserUtils.Authenticate(email, pwd))
            {
                UserUtils.AddFailedAuthAttempt(email);
                showFeedback("Invalid email or password. Try again.");
                return;
            }

            // success
            Session["Email"] = email;

            string guid = Guid.NewGuid().ToString();
            Session["AuthToken"] = guid;

            Response.Cookies.Add(new HttpCookie("AuthToken", guid));
            Response.Redirect("~/Home.aspx");
        }

        protected void showFeedback(string message)
        {
            panel1.Visible = true;
            lbl_feedback.Visible = true;
            lbl_feedback.Text = message.Trim();
        }

        protected void resetFeedback()
        {
            panel1.Visible = false;
            lbl_feedback.Visible = false;
            lbl_feedback.Text = String.Empty;
        }
    }
}