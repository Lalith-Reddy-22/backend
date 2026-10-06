using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Medtrix_PmTool_V2_Api.Services.Email
{
    public class EmailService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;

        private const string MandrillUrl = "https://mandrillapp.com/api/1.0/messages/send";

        public EmailService(HttpClient http, IConfiguration config)
        {
            _http   = http;
            _config = config;
        }

        public Task<bool> SendAsync(string toEmail, string toName, string subject, string htmlBody)
            => PostAsync(toEmail, toName, subject, htmlBody, ccEmail: null);

        public Task<bool> SendWithAdminCcAsync(string toEmail, string toName, string subject, string htmlBody)
            => PostAsync(toEmail, toName, subject, htmlBody, ccEmail: _config["Mandrill:AdminCcEmail"]);

        private async Task<bool> PostAsync(string toEmail, string toName, string subject, string htmlBody, string? ccEmail)
        {
            var to = new List<MandrillRecipient>
            {
                new() { Email = toEmail, Name = toName, Type = "to" }
            };

            if (!string.IsNullOrWhiteSpace(ccEmail))
                to.Add(new() { Email = ccEmail, Type = "cc" });

            var payload = new MandrillRequest
            {
                Key = _config["Mandrill:ApiKey"]!,
                Message = new MandrillMessage
                {
                    Html      = htmlBody,
                    Subject   = subject,
                    FromEmail = _config["Mandrill:FromEmail"]!,
                    FromName  = _config["Mandrill:FromName"]!,
                    To        = to
                }
            };

            var json    = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _http.PostAsync(MandrillUrl, content);
                var body     = await response.Content.ReadAsStringAsync();

                var results = JsonSerializer.Deserialize<List<MandrillResponse>>(body);
                return results?.Any(r => r.Status == "sent") ?? false;
            }
            catch
            {
                return false;
            }
        }
    }

    file class MandrillRequest
    {
        [JsonPropertyName("key")]     public string Key { get; set; } = string.Empty;
        [JsonPropertyName("message")] public MandrillMessage Message { get; set; } = new();
    }

    file class MandrillMessage
    {
        [JsonPropertyName("html")]       public string Html { get; set; } = string.Empty;
        [JsonPropertyName("subject")]    public string Subject { get; set; } = string.Empty;
        [JsonPropertyName("from_email")] public string FromEmail { get; set; } = string.Empty;
        [JsonPropertyName("from_name")]  public string FromName { get; set; } = string.Empty;
        [JsonPropertyName("to")]         public List<MandrillRecipient> To { get; set; } = new();
        [JsonPropertyName("important")]  public bool Important { get; set; } = true;
        [JsonPropertyName("inline_css")] public bool InlineCss { get; set; } = true;
        [JsonPropertyName("track_clicks")] public bool TrackClicks { get; set; } = true;
    }

    file class MandrillRecipient
    {
        [JsonPropertyName("email")] public string Email { get; set; } = string.Empty;
        [JsonPropertyName("name")]  public string? Name { get; set; }
        [JsonPropertyName("type")]  public string Type { get; set; } = "to";
    }

    file class MandrillResponse
    {
        [JsonPropertyName("email")]         public string Email { get; set; } = string.Empty;
        [JsonPropertyName("status")]        public string Status { get; set; } = string.Empty;
        [JsonPropertyName("_id")]           public string Id { get; set; } = string.Empty;
        [JsonPropertyName("reject_reason")] public string? RejectReason { get; set; }
    }
}
