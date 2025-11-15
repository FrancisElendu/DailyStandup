using DailyStandup.Application.Interface;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace DailyStandup.Infrastructure.Notifiers
{
    public class TeamsNotifier : INotifier
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _http;


        public TeamsNotifier(IConfiguration config, IHttpClientFactory factory)
        {
            _config = config;
            _http = factory.CreateClient("teams");
        }


        public async Task NotifyAsync(string message, CancellationToken ct = default)
        {
            var url = _config["Teams:WebhookUrl"];
            if (string.IsNullOrEmpty(url)) throw new InvalidOperationException("Teams webhook not configured");
            await _http.PostAsJsonAsync(url, new { text = message }, ct);
        }
    }
}
