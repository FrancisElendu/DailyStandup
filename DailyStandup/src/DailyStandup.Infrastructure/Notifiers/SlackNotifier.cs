using DailyStandup.Application.Interface;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace DailyStandup.Infrastructure.Notifiers
{
    public class SlackNotifier : INotifier
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _http;

        public SlackNotifier(IConfiguration config, IHttpClientFactory factory)
        {
            _config = config;
            _http = factory.CreateClient("slack");
        }

        public async Task NotifyAsync(string message, CancellationToken ct = default)
        {
            var url = _config["Slack:WebhookUrl"];
            if (string.IsNullOrEmpty(url)) throw new InvalidOperationException("Slack webhook not configured");
            await _http.PostAsJsonAsync(url, new { text = message }, ct);
        }
    }
}
