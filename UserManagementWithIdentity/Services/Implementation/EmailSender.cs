using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using UserManagementWithIdentity.Utilities;

namespace UserManagementWithIdentity.Services.Implementation
{
	public class EmailSender : IEmailSender
	{
		private readonly ApplicationEmail _emailSettings;
		private readonly ILogger<EmailSender> _logger;

		public EmailSender(IOptions<ApplicationEmail> options,
			ILogger<EmailSender> logger)
		{
			_emailSettings = options.Value;
			_logger = logger;
		}

		public async Task SendEmailAsync(string email, string subject, string htmlMessage)
		{
			var mimeMessage = new MimeMessage()
			{
				Sender = MailboxAddress.Parse(_emailSettings.Email),
				Subject = subject,
				Body = new BodyBuilder() { HtmlBody = htmlMessage }.ToMessageBody()
			};

			mimeMessage.From.Add(new MailboxAddress(_emailSettings.DisplayName, _emailSettings.Email));
			mimeMessage.To.Add(MailboxAddress.Parse(email));

			using var client = new SmtpClient();
			try
			{
				await client.ConnectAsync(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls);
				await client.AuthenticateAsync(_emailSettings.Email, _emailSettings.Password);
				await client.SendAsync(mimeMessage);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error: {Message}", ex.Message);
				throw;
			}
			finally
			{
				await client.DisconnectAsync(true);
			}
		}
	}
}
