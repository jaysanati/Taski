using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Taski.Scheduler.Core;

namespace FileSync;

public class FileSynchronizer : IDynamicTask
{
    //One class to handle all Orgs
    public DynamicTaskStatus Status { get; private set; }

    public async Task ExecuteAsync(ILogger logger, IConfiguration configuration, CancellationToken cancellationToken)
    {
        logger.LogInformation(GetType().Name + ":" + configuration["ConnectionStrings:DefaultConnection"]);
        Status = DynamicTaskStatus.Running;
        Debug.WriteLine($"{GetType().Name} Started.");
        await Task.Delay(7000);
        Debug.WriteLine($"{GetType().Name} Executed.");
        Status = DynamicTaskStatus.Ready;
    }
}
