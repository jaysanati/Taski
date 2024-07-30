using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Taski.Scheduler.Core
{
    public enum DynamicTaskStatus
    {
        Ready,
        Running,
    }

    public interface IDynamicTask
    {
        DynamicTaskStatus Status { get; }
        Task ExecuteAsync(ILogger logger, IConfiguration configuration, CancellationToken cancellationToken);
    }
}