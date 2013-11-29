using System.Net.Mail;

namespace Cotecna.Vestalis.Core
{
    /// <summary>
    /// This class has the methods for send an email
    /// </summary>
    public static class EmailBusiness
    {
        /// <summary>
        /// This method is user for send an email
        /// </summary>
        /// <param name="toAddress">The address of the user</param>
        /// <param name="body">Content of the mail</param>
        /// <param name="subject">Subject</param>
        /// <param name="supportEmail">Email of support</param>
        /// <param name="supportName">The name of the support team</param>
        public static void SendEmail(string toAddress, string body, string subject, string supportEmail, string supportName)
        {
            SmtpClient emailClient = new SmtpClient();
            MailMessage message = new MailMessage();
            message.From = new MailAddress(supportEmail, supportName);
            message.Subject = subject;
            message.To.Add(toAddress);
            message.IsBodyHtml = true;
            message.Body = body;
            emailClient.Send(message);
        }
    }
}
