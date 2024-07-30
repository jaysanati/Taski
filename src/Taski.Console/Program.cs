using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Taski.Scheduler;

var builder = new ConfigurationBuilder();
builder.SetBasePath(Directory.GetCurrentDirectory())
       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
IConfiguration config = builder.Build();

ILogger<Scheduler> logger = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
}).CreateLogger<Scheduler>();

Scheduler scheduler = new Scheduler(logger, config);
scheduler.LoadPlugins();

await scheduler.Run();

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Execution Completed!");
