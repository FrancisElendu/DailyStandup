using DailyStandup.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyStandup.Application.Interface
{
    public interface ILogRepository
    {
        Task AddAsync(LogEntry entry, CancellationToken ct = default);
    }
}
