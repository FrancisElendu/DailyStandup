using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyStandup.Application.Interface
{
    public interface IJiraClient
    {
        Task<IEnumerable<JiraIssueDto>> GetIssuesUpdatedSince(DateTime since);
    }
}
