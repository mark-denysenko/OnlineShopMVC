using Domain.Abstract;
using Domain.Entities;
using System;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Domain.Concrete
{
    public class EmailSettings
    {
        public string MailToAdress = "orders@bookstore.com";
        public string MailFromAdress = "bookstore@bookstore.com";
        public bool UseSSl = true;
        public string Username = "Name";
        public string Password = "Password";
        public string ServeName = "smtp.server.com";
        public int ServerPort = 587;
        public bool WriteAsFile = true;
        public string FileLocation = @"D:\others";
    }

    public class EmailOrderProcessor : IOrderProcessor
    {
        private EmailSettings emailSettings;

        public EmailOrderProcessor(EmailSettings settings)
        {
            emailSettings = settings;
        }

        public void ProcessOrder(Cart cart, ShippingDetails shippingDetails)
        {
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.EnableSsl = emailSettings.UseSSl;
                smtpClient.Host = emailSettings.ServeName;
                smtpClient.Port = emailSettings.ServerPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password);

                if(emailSettings.WriteAsFile)
                {
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtpClient.PickupDirectoryLocation = emailSettings.FileLocation;
                    smtpClient.EnableSsl = false;
                }

                StringBuilder body = new StringBuilder()
                    .AppendLine("Order completed!")
                    .AppendLine("=================")
                    .AppendLine("Items:");

                foreach(var line in cart.Lines)
                {
                    var linePrice = line.Quantity * line.Book.Price;
                    body.AppendFormat("{0} x {1} (total {2:c}", line.Quantity, line.Book.Name, linePrice);
                }

                body.AppendFormat("Total value: {0:c}", cart.ComputeTotalValue())
                        .AppendLine("- - - - - - -")
                        .AppendLine("Delivery:")
                        .AppendLine(shippingDetails.Street)
                        .AppendLine(shippingDetails.City)
                        .AppendLine(shippingDetails.Country)
                        .AppendLine("-          -")
                        .AppendLine("With gift wrap?")
                        .AppendFormat("Gift wrap: {0}", shippingDetails.GiftWrap ? "Yes" : "No");

                MailMessage mailMessage = new MailMessage(
                    emailSettings.MailFromAdress,
                    emailSettings.MailToAdress,
                    "New order was sent!",
                    body.ToString());

                if(emailSettings.WriteAsFile)
                {
                    mailMessage.BodyEncoding = Encoding.UTF8;
                }

                //smtpClient.Send(mailMessage);
            }
        }
    }
}
