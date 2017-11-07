using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Matryoshka
{
    /// <summary>
    /// Composite <see cref="IDisposable"/>.
    /// </summary>
    public sealed class CompositeDisposable : IDisposable
    {
        private ConcurrentStack<IDisposable> _disposables = new ConcurrentStack<IDisposable>();

        /// <summary>
        /// Add disposable to composite disposable.
        /// </summary>
        /// <param name="d">Disposable.</param>
        public void Add(IDisposable d)
        {
            ConcurrentStack<IDisposable> s;
            if ((s = Interlocked.CompareExchange(ref _disposables, null, null)) != null)
            {
                s.Push(d);
            }
            else
            {
                throw new ObjectDisposedException("CompositeDisposable");
            }
        }

        /// <summary>
        /// Add range of disposables to composite disposable.
        /// </summary>
        /// <param name="d">Disposables.</param>
        public void AddRange(params IDisposable[] d)
        {
            if (d == null)
            {
                return;
            }
            ConcurrentStack<IDisposable> s;
            if ((s = Interlocked.CompareExchange(ref _disposables, null, null)) != null)
            {
                s.PushRange(d);
            }
            else
            {
                throw new ObjectDisposedException("CompositeDisposable");
            }
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            ConcurrentStack<IDisposable> s;
            if ((s = Interlocked.Exchange(ref _disposables, null)) != null)
            {
                List<Exception> errors = null;
                while (s.TryPop(out var c))
                {
                    try
                    {
                        c?.Dispose();
                    }
                    catch (Exception e)
                    {
                        if (errors == null)
                        {
                            errors = new List<Exception>();
                        }
                        errors.Add(e);
                    }
                }
                if (errors != null)
                {
                    throw new AggregateException(errors);
                }
            }
        }
    }
}