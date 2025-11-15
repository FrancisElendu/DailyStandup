using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyStandup.Application
{
    public record JiraIssueDto(string Key, string Summary, string Status);
}
