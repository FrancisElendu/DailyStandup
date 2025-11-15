using DailyStandup.Application;
using DailyStandup.Application.Interface;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace DailyStandup.Infrastructure.Services
{
    public class JiraClient : IJiraClient
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _config;


        public JiraClient(HttpClient client, IConfiguration config)
        {
            _client = client;
            _config = config;
            var token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{config["Jira:Email"]}:{config["Jira:ApiToken"]}"));
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", token);
        }


        // Very small example: search for issues updated since `since`.
        public async Task<IEnumerable<JiraIssueDto>> GetIssuesUpdatedSince(DateTime since)
        {
            var jql = System.Web.HttpUtility.UrlEncode($"updated >= \"{since:yyyy-MM-dd}\"");
            var uri = $"/rest/api/2/search?jql={jql}&fields=summary,status";
            var resp = await _client.GetFromJsonAsync<JiraSearchResponse?>(uri);
            if (resp?.Issues == null) return Array.Empty<JiraIssueDto>();
            return resp.Issues.Select(i => new JiraIssueDto(i.Key, i.Fields.Summary, i.Fields.Status.Name));
        }


        private record JiraSearchResponse(List<JiraIssue> Issues);
        private record JiraIssue(string Key, JiraFields Fields);
        private record JiraFields(string Summary, JiraStatus Status);
        private record JiraStatus(string Name);
    }
}
