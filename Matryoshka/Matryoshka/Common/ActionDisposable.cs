using System;
using System.Threading;

namespace Matryoshka
{
    /// <summary>
    /// Action wrapped as <see cref="IDisposable"/>.
    /// </summary>
    public sealed class ActionDisposable : IDisposable
    {
        private int _isDisposed;

        private readonly Action _onDispose;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="onDispose">Action to wrap.</param>
        public ActionDisposable(Action onDispose)
        {
            _onDispose = onDispose;
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (Interlocked.Exchange(ref _isDisposed, 1) == 0)
            {
                _onDispose?.Invoke();
            }
        }
    }
}