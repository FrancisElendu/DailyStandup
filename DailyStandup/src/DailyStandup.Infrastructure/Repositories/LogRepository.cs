using DailyStandup.Application.Interface;
using DailyStandup.Domain.Entities;
using DailyStandup.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyStandup.Infrastructure.Repositories
{
    public class LogRepository : ILogRepository
    {
        private readonly DailyStandupDbContext _db;
        public LogRepository(DailyStandupDbContext db) => _db = db;


        public async Task AddAsync(LogEntry entry, CancellationToken ct = default)
        {
            _db.LogEntries.Add(entry);
            await _db.SaveChangesAsync(ct);
        }
    }
}
