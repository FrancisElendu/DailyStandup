using DailyStandup.Application.Interface;
using DailyStandup.Application;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace DailyStandup.Infrastructure.Services
{
    public class GitHubClient : IGitHubClient
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _config;


        public GitHubClient(HttpClient client, IConfiguration config)
        {
            _client = client;
            _config = config;
        }


        // This example fetches commits from a configured repo. Update to loop repos as needed.
        public async Task<IEnumerable<GitHubCommitDto>> GetCommitsSince(DateTime since)
        {
            var owner = _config["GitHub:Owner"] ?? "your-org";
            var repo = _config["GitHub:Repo"] ?? "your-repo";
            var uri = $"/repos/{owner}/{repo}/commits?since={since:O}";


            var list = await _client.GetFromJsonAsync<List<GitHubCommitResponse>>(uri);
            if (list == null) return Array.Empty<GitHubCommitDto>();


            return list.Select(c => new GitHubCommitDto(c.Sha, c.Commit.Message, c.Commit.Author.Name, c.HtmlUrl));
        }


        private record GitHubCommitResponse(string Sha, GitHubCommit Commit, string HtmlUrl);
        private record GitHubCommit(GitHubCommitAuthor Author, string Message);
        private record GitHubCommitAuthor(string Name, string Email, DateTime Date);
    }
}
