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
        private readonly System.Collections.Generic.HashSet<System.Threading.CancellationTokenSource> _activeScopes = new();
        private readonly System.Collections.Generic.Dictionary<string, System.Collections.Generic.HashSet<System.Threading.CancellationTokenSource>> _scopesById = new(System.StringComparer.Ordinal);
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

        public IBusyScope Begin(string? message = null, string? scopeId = null)
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

            // Create a CTS for this scope and track it so RequestCancel can cancel active scopes
            var cts = new System.Threading.CancellationTokenSource();
            lock (_sync)
            {
                _activeScopes.Add(cts);
                if (!string.IsNullOrEmpty(scopeId))
                {
                    if (!_scopesById.TryGetValue(scopeId, out var set))
                    {
                        set = new System.Collections.Generic.HashSet<System.Threading.CancellationTokenSource>();
                        _scopesById[scopeId] = set;
                    }
                    set.Add(cts);
                }
            }

            return new Scope(this, cts, scopeId);
        }

        private void End(System.Threading.CancellationTokenSource? cts, string? scopeId = null)
        {
            if (cts != null)
            {
                lock (_sync)
                {
                    _activeScopes.Remove(cts);
                    if (!string.IsNullOrEmpty(scopeId) && _scopesById.TryGetValue(scopeId, out var set))
                    {
                        set.Remove(cts);
                        if (set.Count == 0)
                            _scopesById.Remove(scopeId);
                    }
                }
                try { cts.Dispose(); } catch { }
            }

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
                OnBusyStateChanged(new BusyStateChangedEventArgs(false, msg, GetActiveScopeIds()));
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

        public void RequestCancel(string? scopeId = null)
        {
            if (string.IsNullOrEmpty(scopeId))
            {
                // Cancel all active scopes
                System.Threading.CancellationTokenSource[] snapshot;
                lock (_sync)
                {
                    snapshot = new System.Threading.CancellationTokenSource[_activeScopes.Count];
                    _activeScopes.CopyTo(snapshot);
                }

                foreach (var cts in snapshot)
                {
                    try { cts.Cancel(); } catch { }
                }
            }
            else
            {
                // Cancel only scopes with the given id
                System.Threading.CancellationTokenSource[] snapshot;
                lock (_sync)
                {
                    if (!_scopesById.TryGetValue(scopeId, out var set) || set.Count == 0) return;
                    snapshot = new System.Threading.CancellationTokenSource[set.Count];
                    set.CopyTo(snapshot);
                }

                foreach (var cts in snapshot)
                {
                    try { cts.Cancel(); } catch { }
                }
            }
        }

        private sealed class Scope : IBusyScope
        {
            private BusyIndicatorService? _owner;
            private System.Threading.CancellationTokenSource? _cts;
            private string? _id;
            public CancellationToken Token => _cts?.Token ?? System.Threading.CancellationToken.None;
            public string? Id => _id;

            public Scope(BusyIndicatorService owner, System.Threading.CancellationTokenSource cts, string? id)
            {
                _owner = owner;
                _cts = cts;
                _id = id;
            }

            public void Dispose()
            {
                var o = Interlocked.Exchange(ref _owner, null);
                var c = Interlocked.Exchange(ref _cts, null);
                var id = Interlocked.Exchange(ref _id, null);
                o?.End(c, id);
            }
        }

        private string[] GetActiveScopeIds()
        {
            lock (_sync)
            {
                if (_scopesById.Count == 0) return Array.Empty<string>();
                var keys = new string[_scopesById.Count];
                _scopesById.Keys.CopyTo(keys, 0);
                return keys;
            }
        }
    }
}
