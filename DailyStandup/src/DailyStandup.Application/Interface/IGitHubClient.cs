using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyStandup.Application.Interface
{
    public interface IGitHubClient
    {
        Task<IEnumerable<GitHubCommitDto>> GetCommitsSince(DateTime since);
    }
}
