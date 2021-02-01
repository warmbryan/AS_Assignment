using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

using SendGrid;
using SendGrid.Helpers.Mail;


namespace BryanToh194937Y_ASAssignment.App_Code.Utility
{
    public static class Email
    {
        public static async Task SendEmail(string email)
        {
            var apiKey = "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";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("bryan@envorial.com", "Bryan AS");
            var subject = "Automated Account Recovery";
            var to = new EmailAddress(email, "User");
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = "<strong>You account has been disabled because of multiple invalid attempts.</strong><br>Click on the link below to reset your password.<br>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}