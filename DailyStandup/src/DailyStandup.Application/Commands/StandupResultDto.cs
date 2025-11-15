using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyStandup.Application.Commands
{
    public record StandupResultDto(bool Success, string Summary, DateTime GeneratedAt);
}
