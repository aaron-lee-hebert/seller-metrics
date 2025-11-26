using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace SellerMetrics.Infrastructure.Services;

/// <summary>
/// Configuration options for SendGrid email service.
/// </summary>
public class SendGridOptions
{
    /// <summary>
    /// The configuration section name.
    /// </summary>
    public const string SectionName = "SendGrid";

    /// <summary>
    /// SendGrid API key.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Sender email address.
    /// </summary>
    public string SenderEmail { get; set; } = string.Empty;

    /// <summary>
    /// Sender display name.
    /// </summary>
    public string SenderName { get; set; } = "SellerMetrics";
}

/// <summary>
/// Email sender implementation using Twilio SendGrid.
/// </summary>
public class SendGridEmailSender : IEmailSender
{
    private readonly SendGridOptions _options;
    private readonly ILogger<SendGridEmailSender> _logger;

    public SendGridEmailSender(
        IOptions<SendGridOptions> options,
        ILogger<SendGridEmailSender> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        if (string.IsNullOrEmpty(_options.ApiKey))
        {
            _logger.LogWarning("SendGrid API key is not configured. Email to {Email} with subject '{Subject}' was not sent.", email, subject);
            return;
        }

        var client = new SendGridClient(_options.ApiKey);
        var from = new EmailAddress(_options.SenderEmail, _options.SenderName);
        var to = new EmailAddress(email);
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent: null, htmlMessage);

        var response = await client.SendEmailAsync(msg);

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Email sent successfully to {Email} with subject '{Subject}'", email, subject);
        }
        else
        {
            var responseBody = await response.Body.ReadAsStringAsync();
            _logger.LogError(
                "Failed to send email to {Email}. Status: {StatusCode}, Response: {Response}",
                email,
                response.StatusCode,
                responseBody);
        }
    }
}
