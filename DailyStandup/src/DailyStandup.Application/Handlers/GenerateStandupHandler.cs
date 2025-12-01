using DailyStandup.Application.Commands;
using DailyStandup.Application.Interface;
using DailyStandup.Domain.Entities;
using MediatR;

namespace DailyStandup.Application.Handlers
{
    public class GenerateStandupHandler : IRequestHandler<GenerateStandupCommand, StandupResultDto>
    {
        private readonly IJiraClient _jira;
        private readonly IGitHubClient _gitHub;
        private readonly IEnumerable<INotifier> _notifiers;
        private readonly ILogRepository _logRepo;


        public GenerateStandupHandler(IJiraClient jira, IGitHubClient gitHub, IEnumerable<INotifier> notifiers, ILogRepository logRepo)
        {
            _jira = jira;
            _gitHub = gitHub;
            _notifiers = notifiers;
            _logRepo = logRepo;
        }


        public async Task<StandupResultDto> Handle(GenerateStandupCommand request, CancellationToken cancellationToken)
        {
            // 1. Gather data from Jira & GitHub
            var jiraUpdates = await _jira.GetIssuesUpdatedSince(request.ForDate.Date);
            var gitHubActivity = await _gitHub.GetCommitsSince(request.ForDate.Date);


            // 2. Create a concise summary (simple template here — you can plug an LLM for better summaries)
            var summary = BuildSummary(request.ForDate, jiraUpdates, gitHubActivity);


            // 3. Send to notifiers (Slack + Teams)
            foreach (var notifier in _notifiers)
            {
                try
                {
                    await notifier.NotifyAsync(summary, cancellationToken);
                }
                catch (Exception ex)
                {
                    await _logRepo.AddAsync(new LogEntry { Level = "Error", Message = ex.Message, Details = ex.ToString() }, cancellationToken);
                }
            }


            // 4. Persist an informational log
            await _logRepo.AddAsync(new DailyStandup.Domain.Entities.LogEntry
            {
                Level = "Information",
                Message = "Standup summary generated",
                Details = summary
            }, cancellationToken);


            return new StandupResultDto(true, summary, DateTime.UtcNow);
        }


        private string BuildSummary(DateTime forDate, IEnumerable<JiraIssueDto> jiraIssues, IEnumerable<GitHubCommitDto> commits)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"Daily Standup — {forDate:yyyy-MM-dd}");
            sb.AppendLine();
            sb.AppendLine("Jira updates:");
            foreach (var i in jiraIssues.Take(10))
                sb.AppendLine($"- [{i.Key}] {i.Summary} ({i.Status})");


            sb.AppendLine();
            sb.AppendLine("GitHub activity (commits):");
            foreach (var c in commits.Take(10))
                sb.AppendLine($"- {c.Author}: {c.Message.Split('\n').First()} ({c.Url})");


            return sb.ToString();
        }
    }
}
