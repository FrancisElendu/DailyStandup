using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyStandup.Application
{
    public record GitHubCommitDto(string Sha, string Message, string Author, string Url);
}
