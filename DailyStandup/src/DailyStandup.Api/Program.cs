using DailyStandup.Application.Behaviours;
using DailyStandup.Application.Commands;
using DailyStandup.Application.Interface;
using DailyStandup.Infrastructure.Hosted;
using DailyStandup.Infrastructure.Notifiers;
using DailyStandup.Infrastructure.Persistence;
using DailyStandup.Infrastructure.Repositories;
using DailyStandup.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


// Configuration and Serilog
Log.Logger = new LoggerConfiguration()
.ReadFrom.Configuration(builder.Configuration)
.Enrich.FromLogContext()
.CreateLogger();


builder.Host.UseSerilog();


// Add services
builder.Services.AddControllers();


// DbContext (Infrastructure project)
builder.Services.AddDbContext<DailyStandupDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Update the AddMediatR call to specify the assembly containing the handlers
builder.Services.AddMediatR(typeof(GenerateStandupCommand).Assembly);

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));


// Infrastructure clients
builder.Services.AddHttpClient<IJiraClient, JiraClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Jira:BaseUrl"]);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient<IGitHubClient, GitHubClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["GitHub:BaseUrl"]);
    client.DefaultRequestHeaders.Add("User-Agent", "DailyStandupService");
    if (!string.IsNullOrEmpty(builder.Configuration["GitHub:Token"]))
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("token", builder.Configuration["GitHub:Token"]);
});


// Notifiers
builder.Services.AddSingleton<INotifier, SlackNotifier>();
builder.Services.AddSingleton<INotifier, TeamsNotifier>();


// Register hosted worker that triggers the mediation command daily
builder.Services.AddHostedService<DailySchedulerHostedService>();


// Infra repositories & services
builder.Services.AddScoped<ILogRepository, LogRepository>();

var app = builder.Build();

app.MapControllers();


app.Run();
