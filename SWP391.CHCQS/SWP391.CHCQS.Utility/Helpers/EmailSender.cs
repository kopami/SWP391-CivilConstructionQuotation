﻿
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MimeKit;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;
using System.Net.Mail;
using static System.Net.Mime.MediaTypeNames;

namespace SWP391.CHCQS.Utility.Helpers
{
	public class EmailSender : IEmailSender
	{
		public string SendGridSecret { get; set; }
<<<<<<< HEAD
=======
		private readonly IConfiguration _configuration;
		private readonly IWebHostEnvironment _environment;
>>>>>>> Demostration

		public EmailSender(IConfiguration _config, IWebHostEnvironment environment)
		{
			SendGridSecret = _config.GetValue<string>("SendGrid:SecretKey");
<<<<<<< HEAD
=======
			_configuration = _config;
			_environment = environment;
>>>>>>> Demostration
		}

		public Task SendEmailAsync(string email, string subject, string htmlMessage)
		{
			//var emailToSend = new MimeMessage();
			//emailToSend.From.Add(MailboxAddress.Parse("hello@dotnetmastery.com"));
			//emailToSend.To.Add(MailboxAddress.Parse(email));
			//emailToSend.Subject = subject;
			//emailToSend.Body = new TextPart(MimeKit.Text.TextFormat.Html){ Text = htmlMessage};

			////send email
			//using (var emailClient = new SmtpClient())
			//{
			//    emailClient.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
			//    emailClient.Authenticate("dotnetmastery@gmail.com", "DotNet123$");
			//    emailClient.Send(emailToSend);
			//    emailClient.Disconnect(true);
			//}

			//return Task.CompletedTask;

			var client = new SendGridClient(SendGridSecret);
			var from = new EmailAddress("hello@dotnetmastery.com", "Bulky Book");
			var to = new EmailAddress(email);
			var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlMessage);
			return client.SendEmailAsync(msg);

		}
	
		/* HOW TO USE BODY BUILDER TO RENDER HTML
				var builder = new BodyBuilder();
		 Set the html version of the message text
			builder.HtmlBody = string.Format(@"<p>Hey Alice,<br>
		<p>What are you up to this weekend? Monica is throwing one of her parties on
		Saturday and I was hoping you could make it.<br>
		<p>Will you be my +1?<br>
		<p>-- Joey<br>
		<center><img src=""cid:{0}""></center>", image.ContentId);
		We may also want to attach a calendar event for Monica's party...
		builder.Attachments.Add(@"C:\Users\Joey\Documents\party.ics");

		 Now we just need to set the message body and we're done
		 message.Body = builder.ToMessageBody();
		*/
<<<<<<< HEAD
		public static Task SendInfoToEmail(string toEmail, string subject, string html)
		{
			var bodyBuilder = new BodyBuilder();
			string fromMail = "ourhomeswp391@gmail.com";
			string fromPassword = "kusvqhpurbksspvb";

			MailMessage mess = new MailMessage();
			mess.From = new MailAddress(fromMail);
			mess.Subject = subject;
			mess.To.Add(new MailAddress(toEmail));

			bodyBuilder.HtmlBody = String.Format(html); 

			mess.Body = bodyBuilder.HtmlBody;
=======
		/// <summary>
		/// Hàm để gửi mail cho user với format sẵn, hàm trả về boolean
		/// False: nếu có gì đó xảy ra trong lúc lỗi
		/// True: nếu thành công, chạy ngon lành
		/// </summary>
		/// <param name="toEmail"></param>
		/// <param name="customerName"></param>
		/// <param name="quoteId"></param>
		/// <returns></returns>
		public bool SendInfoToEmail(string toEmail, string customerName, string quoteId)
        {
			try
			{
				if (toEmail == null || toEmail.Equals(String.Empty))
					return false;
				var fromMail = _configuration["MailSettings:Mail"];
				var fromPassword = _configuration["MailSettings:FromKeyPassword"];
				MailMessage mess = new MailMessage();
				mess.From = new MailAddress(fromMail);
				mess.Subject = _configuration["MailSettings:Subject"];

				mess.To.Add(new MailAddress(toEmail));
				var hostAddress = String.Empty;
				if (_environment.IsDevelopment())
					hostAddress = _configuration["Environment:LocalDomain"];
				if (_environment.IsProduction())
					hostAddress = _configuration["Environment:IISExpress"];
				string url = $"{hostAddress}{_configuration["MailSettings:ActionLinkNoPara"]}?quoteId={quoteId}";
				string mailContent = $@"
<p>Welcome {customerName} !</p>
<p>Thank you for contacting us to use our construction quote request service. We received your request and it was reviewed by our team.</p>
<p>We've tried to respond to you as quickly as possible and made sure to thoroughly review your requests.</p>
<p>Once again, we would like to express our sincere thanks for choosing our service. Wishing you a wonderful day ahead!</p>
    
<p>For further information of quotation, please visit <a href='{url}'>here</a>.</p>
    
<p>Yours sincerely,</p>
<p>Our Home Architecture</p>
";
				var bodyBuilder = new BodyBuilder();
				bodyBuilder.HtmlBody = String.Format(mailContent);
				mess.Body = bodyBuilder.HtmlBody;
>>>>>>> Demostration

				mess.IsBodyHtml = true;

				var smtp = new SmtpClient("smtp.gmail.com")
				{
					Port = 587,
					Credentials = new NetworkCredential(fromMail, fromPassword),
					EnableSsl = true
				};
				smtp.Send(mess);
			}catch (Exception e)
			{
				Console.Write(e.ToString());
				return false;
			}
			return true;
		}

	}
}
