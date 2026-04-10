using MailKit.Net.Smtp;
using MimeKit;

namespace ReachAPaw.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public void SendOtp(string toEmail, string otp)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Reach.A.Paw", _config["EmailSettings:From"]));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = "Your OTP - Reach.A.Paw";

            message.Body = new TextPart("html")
            {
                Text = $@"
                    <div style='font-family: sans-serif; max-width: 400px; margin: 0 auto;'>
                        <h2 style='color: #5d4037;'>Welcome to Reach.A.Paw!</h2>
                        <p>Your OTP verification code is:</p>
                        <h1 style='color: #5d4037; letter-spacing: 8px;'>{otp}</h1>
                        <p style='color: #999; font-size: 0.85rem;'>This code expires in 10 minutes.</p>
                    </div>"
            };

            using var client = new SmtpClient();
            client.Connect(_config["EmailSettings:Host"], int.Parse(_config["EmailSettings:Port"]), false);
            client.Authenticate(_config["EmailSettings:From"], _config["EmailSettings:Password"]);
            client.Send(message);
            client.Disconnect(true);
        }
    }
}