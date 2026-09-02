using AppMvp.Presentation.Abstractions;
using System;
using System.Threading;

namespace AppMvp.Presentation.Services
{
    public sealed class BusyIndicatorService : IBusyIndicator
    {
        private int _count;
        private string? _message;
        private readonly object _sync = new();
        private System.Threading.SynchronizationContext? _syncContext;

        public bool IsBusy => Volatile.Read(ref _count) > 0;
        public string? Message
        {
            get
            {
                lock (_sync) { return _message; }
            }
        }

        public event EventHandler<BusyStateChangedEventArgs>? BusyStateChanged;

        public IDisposable Begin(string? message = null)
        {
            var newCount = Interlocked.Increment(ref _count);

            bool raise = false;
            lock (_sync)
            {
                if (message != null)
                    _message = message;
                if (newCount == 1)
                    raise = true;
            }

            if (raise)
                OnBusyStateChanged(new BusyStateChangedEventArgs(true, Message));

            return new Scope(this);
        }

        private void End()
        {
            var newCount = Interlocked.Decrement(ref _count);

            bool raise = false;
            string? msg = null;
            lock (_sync)
            {
                if (newCount <= 0)
                {
                    _count = 0;
                    _message = null;
                    raise = true;
                }
                else
                {
                    msg = _message;
                }
            }

            if (raise)
                OnBusyStateChanged(new BusyStateChangedEventArgs(false, msg));
        }

        private void OnBusyStateChanged(BusyStateChangedEventArgs e)
        {
            var handler = BusyStateChanged;
            if (handler == null) return;

            var ctx = _syncContext;
            if (ctx != null)
            {
                // Post to the captured synchronization context so UI handlers run on UI thread
                ctx.Post(state => handler(this, (BusyStateChangedEventArgs)state!), e);
            }
            else
            {
                handler(this, e);
            }
        }

        public void SetSynchronizationContext(System.Threading.SynchronizationContext? context)
        {
            lock (_sync)
            {
                _syncContext = context;
            }
        }

        private sealed class Scope : IDisposable
        {
            private BusyIndicatorService? _owner;
            public Scope(BusyIndicatorService owner) => _owner = owner;
            public void Dispose()
            {
                var o = Interlocked.Exchange(ref _owner, null);
                o?.End();
            }
        }
    }
}
