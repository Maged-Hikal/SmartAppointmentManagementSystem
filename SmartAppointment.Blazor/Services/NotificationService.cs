//using SmartAppointment.Domain.Entities;
//using System.Net;
//using System.Net.Mail;

//namespace SmartAppointment.Blazor.Services
//{
//    public interface INotificationService
//    {
//        //Task SendStatusUpdateAsync(Appointment appointment, string newStatus);
//        Task SendStatusUpdateAsync(string customerName, string email, string phone, string date, string status);
//    }

//public class NotificationService : INotificationService
//    {
//        private readonly IConfiguration _config;
//        public NotificationService(IConfiguration config) => _config = config;

//        public async Task SendStatusUpdateAsync(string customerName, string email, string phone, string date, string status)
//        {
//            var subject = $"Appointment {status}: {customerName}";
//            var body = $@"
//            <h3>Appointment Update</h3>
//            <p>Hej {customerName},</p>
//            <p>Your appointment for <b>{date}</b> has been <b>{status.ToLower()}</b>.</p>
//            <p>Thank you for using SmartAppointment.</p>";

//            // 1. Send Email to User and CC Admin
//            await SendEmailAsync(email, subject, body, true);

//            // 2. Send SMS to User (Placeholder for your SMS provider)
//           // await SendSmsAsync(phone, stripHtml: $"Hey {customerName}! Your appointment for {date} is {status}.");
//        }

//        private async Task SendEmailAsync(string to, string subject, string body, bool ccAdmin)
//        {
//            // Get settings from appsettings.json
//            var smtpServer = _config["EmailSettings:Host"];
//            var smtpPort = int.Parse(_config["EmailSettings:Port"]);
//            var fromEmail = _config["EmailSettings:From"];
//            var password = _config["EmailSettings:Password"];
//            var adminEmail = _config["EmailSettings:AdminEmail"];

//            using var client = new SmtpClient(smtpServer, smtpPort)
//            {
//                Credentials = new NetworkCredential(fromEmail, password),
//                EnableSsl = true
//            };

//            var mailMessage = new MailMessage
//            {
//                From = new MailAddress(fromEmail, "Smart Appointment System"),
//                Subject = subject,
//                Body = body,
//                IsBodyHtml = true
//            };

//            mailMessage.To.Add(to);
//            if (ccAdmin) mailMessage.CC.Add(adminEmail);

//            await client.SendMailAsync(mailMessage);
//        }

//        //private async Task SendSmsAsync(string phone, string message)
//        //{
//        //    // Integration point for providers like Sinch or Twilio
//        //    // Example: await _sinchClient.SendSms(phone, message);
//        //    Console.WriteLine($"SMS Logic: Sending to {phone} -> {message}");
//        //}
//    }
//}
