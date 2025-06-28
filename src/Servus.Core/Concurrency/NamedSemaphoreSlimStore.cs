using System;
using System.Collections.Generic;

namespace Servus.Core.Concurrency;

public static class NamedSemaphoreSlimStore
{
    private static readonly object StoreLock = new object();
    private static readonly Dictionary<string, NamedSemaphoreSlim> Store = new Dictionary<string, NamedSemaphoreSlim>();

    public static NamedSemaphoreSlim OpenOrCreate(string name, int defaultInitialCount = 1, int defaultMaximumCount = 1)
    {
        lock (StoreLock)
        {
            if (!Store.ContainsKey(name))
            {
                var semaphore = new NamedSemaphoreSlim(name, () => { return Store[name].RequestCounter == 0; }, defaultInitialCount, defaultMaximumCount);
                semaphore.Disposing += Semaphore_Disposing;
                Store.Add(name, semaphore);
            }
            else
            {
                Store[name].RequestCounter++;
            }

            return Store[name];
        }
    }

    private static void Semaphore_Disposing(object? sender, EventArgs e)
    {
        if (sender is NamedSemaphoreSlim semaphore)
        {
            Store.Remove(semaphore.Name);    
        }
    }
}