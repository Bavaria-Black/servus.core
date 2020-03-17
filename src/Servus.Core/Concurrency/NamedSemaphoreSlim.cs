using System;
using System.Threading;

namespace Servus.Core.Concurrency
{
    public class NamedSemaphoreSlim : SemaphoreSlim
    {
        private readonly Func<bool> _disposingAllowed;

        internal event EventHandler Disposing;

        public string Name { get; }
        internal int RequestCounter { get; set; } = 1;

        internal NamedSemaphoreSlim(string name, Func<bool> disposingAllowed, int initialCount = 1, int maxCount = 1)
            : base(initialCount, maxCount)
        {
            Name = name;
            _disposingAllowed = disposingAllowed;
        }

        protected override void Dispose(bool disposing)
        {
            RequestCounter--;
            if (_disposingAllowed())
            {
                Disposing?.Invoke(this, EventArgs.Empty);
                base.Dispose(disposing);
            }
        }
    }
}
