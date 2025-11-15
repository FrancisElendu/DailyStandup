using DailyStandup.Application.Commands;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DailyStandup.Infrastructure.Hosted
{
    public class DailySchedulerHostedService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DailySchedulerHostedService> _logger;
        private readonly int _hour;
        private readonly int _minute;


        public DailySchedulerHostedService(IServiceScopeFactory scopeFactory, IConfiguration config, ILogger<DailySchedulerHostedService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _hour = int.Parse(config["Scheduler:DailyHour"] ?? "8");
            _minute = int.Parse(config["Scheduler:DailyMinute"] ?? "0");
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("DailySchedulerHostedService started. Will run daily at {Hour}:{Minute}", _hour, _minute);


            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;
                var next = new DateTime(now.Year, now.Month, now.Day, _hour, _minute, 0);
                if (now > next) next = next.AddDays(1);
                var delay = next - now;


                await Task.Delay(delay, stoppingToken);


                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    await mediator.Send(new GenerateStandupCommand(DateTime.Today));
                }
                catch (TaskCanceledException) { }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while executing daily standup job");
                }
            }
        }
    }
}
