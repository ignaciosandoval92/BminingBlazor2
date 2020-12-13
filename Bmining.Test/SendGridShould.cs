using BminingBlazor.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace Bmining.Test
{
    [TestClass]
    public class SendGridShould
    {
        [TestMethod]
        public async Task SendEmail()
        {
            var client = new SendGridClient(SendGridConstants.ApiKey);
            var from = new EmailAddress(SendGridConstants.SenderEmail, "Testing information");
            var subject = "Sending with SendGrid is Fun";
            var to = new EmailAddress("francisco.sotocavieres@gmail.com", "Francisco Soto");
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            var sendGridMessage = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(sendGridMessage);
        }
    }
}
