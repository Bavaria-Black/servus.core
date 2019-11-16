using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DevTools.Core.Concurrency
{
    public static class NamedSemaphoreSlimStore
    {
        private static readonly object _storeLock = new object();
        private static readonly Dictionary<string, NamedSemaphoreSlim> _store = new Dictionary<string, NamedSemaphoreSlim>();

        public static NamedSemaphoreSlim OpenOrCreate(string name, int defaultInitialCount = 1, int defaultMaximumCount = 1)
        {
            lock (_storeLock)
            {
                if (!_store.ContainsKey(name))
                {
                    var semaphore = new NamedSemaphoreSlim(name, () => { return _store[name].RequestCounter == 0; }, defaultInitialCount, defaultMaximumCount);
                    semaphore.Disposing += Semaphore_Disposing;
                    _store.Add(name, semaphore);
                }
                else
                {
                    _store[name].RequestCounter++;
                }

                return _store[name];
            }
        }

        private static void Semaphore_Disposing(object sender, EventArgs e)
        {
            var semaphore = sender as NamedSemaphoreSlim;
            _store.Remove(semaphore.Name);
        }
    }
}
