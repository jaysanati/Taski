using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Taski.Scheduler.Core;

namespace UserSync;

public class UserSynchronizer : IDynamicTask
{
    public DynamicTaskStatus Status { get; private set; }

    public async Task ExecuteAsync(ILogger logger, IConfiguration configuration, CancellationToken cancellationToken)
    {
        Debug.WriteLine(configuration["ConnectionStrings:DefaultConnection"]);
        Status = DynamicTaskStatus.Running;
        Debug.WriteLine("UserSynchronizer Started.");
        await Task.Delay(10000);
        Debug.WriteLine("UserSynchronizer Executed.");
        Status = DynamicTaskStatus.Ready;
    }
}
