using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Servus.Core.Application.Startup;

public partial class AppRunner
{
    private async Task RunStartupGatesAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var delay = TimeSpan.FromSeconds(1); // Start with 1 sec
        var maxDelay = TimeSpan.FromSeconds(60);
        var logger =serviceProvider.GetService<ILogger<AppRunner>>();
        
        while (!cancellationToken.IsCancellationRequested)
        {
            var tasks = _startupGates.Select(gate => gate.CheckAsync(cancellationToken));
            var results = await Task.WhenAll(tasks);
        
            if (results.All(result => result))
            { 
                logger?.LogDebug("All startup gates are cleared!");
                return; // All gates are ready
            }

            // Wait with exponential backoff
            await Task.Delay(delay, cancellationToken);
            var delayMs = CalculateNetExponentialBackoffDelay(delay).Milliseconds;
            delay = TimeSpan.FromMilliseconds(Math.Min(delayMs, maxDelay.Milliseconds));
            
            logger?.LogWarning("Not all startup gates are clear. Next retry in [{CurrentDelay}]", delay);
        }
    }

    internal TimeSpan CalculateNetExponentialBackoffDelay(TimeSpan currentDelay)
    {
        return TimeSpan.FromMilliseconds(currentDelay.TotalMilliseconds * 2);
    }
}