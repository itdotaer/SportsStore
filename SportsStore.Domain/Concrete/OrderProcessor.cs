﻿using System.Net;
using System.Net.Mail;
using System.Text;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;

namespace SportsStore.Domain.Concrete
{
    public class EmailSettings
    {
        public string MailToAddress = "hujiangtao1235@gmail.com";
        public string MailFromAddress = "290013424@qq.com";
        public bool UseSsl = true;
        public string UserName = "290013424@qq.com";
        public string Password = "dh19900828";
        public string ServerName = "smtp.qq.com";
        public int ServerPort = 587;
        public bool WriteAsFile = false;
        public string FileLocation = "";
    }

    public class OrderProcessor:IOrderProcessor
    {
        private EmailSettings emailSettings ;

        public OrderProcessor(EmailSettings settings)
        {
            emailSettings = settings;
        }

        public void ProcessOrder(Cart cart, ShippingDetails shippingInfo)
        {
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.EnableSsl = emailSettings.UseSsl;
                smtpClient.Host = emailSettings.ServerName;
                smtpClient.Port = emailSettings.ServerPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(emailSettings.UserName, emailSettings.Password);

                if (emailSettings.WriteAsFile)
                {
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtpClient.PickupDirectoryLocation = emailSettings.FileLocation;
                    smtpClient.EnableSsl = false;
                }
                var body = new StringBuilder().AppendLine("A new order has been submitted.")
                    .AppendLine("---")
                    .AppendLine("Items:");

                foreach (var line in cart.Lines)
                {
                    var subtotal = line.Product.Price * line.Quantity;
                    body.AppendFormat("{0} x {1} (subtotal: {2:c}", line.Quantity,line.Product.Name,subtotal);
                }

                body.AppendFormat("Total order value: {0:c}", cart.CoputeTotalValue())
                    .AppendLine("---").AppendLine("Ship to:")
                    .AppendLine(shippingInfo.Name)
                    .AppendLine(shippingInfo.Line1)
                    .AppendLine(shippingInfo.Line2 ?? "")
                    .AppendLine(shippingInfo.Line3 ?? "")
                    .AppendLine(shippingInfo.City)
                    .AppendLine(shippingInfo.State ?? "")
                    .AppendLine(shippingInfo.Country)
                    .AppendLine(shippingInfo.Zip)
                    .AppendLine("---")
                    .AppendFormat("Gift wrap: {0}",shippingInfo.GiftWrap ? "Yes" : "No");  
                var mailMessage = new MailMessage(emailSettings.MailFromAddress,// From 
                    emailSettings.MailToAddress, // To 
                    "New order submitted!", // Subject
                    body.ToString());  // Body
                if (emailSettings.WriteAsFile)
                {
                    mailMessage.BodyEncoding = Encoding.ASCII;
                }  
                smtpClient.Send(mailMessage);
            }
        }
    }
}
