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

            lock (_sync)
            {
                if (message != null)
                    _message = message;
            }

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

            // Always notify subscribers of the updated busy state so they can update UI based on
            // the active scope ids (for example when a People.Edit scope starts while other scopes remain).
            // optional logging removed; consumers may use IBusyIndicator notifications
            OnBusyStateChanged(new BusyStateChangedEventArgs(true, Message, GetActiveScopeIds()));

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

            string? msg = null;
            lock (_sync)
            {
                if (newCount <= 0)
                {
                    _count = 0;
                    _message = null;
                }
                else
                {
                    msg = _message;
                }
            }

            // Always notify subscribers of the updated busy state so they can update UI based on
            // the active scope ids (for example when a People.Edit scope ends but other scopes remain).
            OnBusyStateChanged(new BusyStateChangedEventArgs(newCount > 0, msg, GetActiveScopeIds()));
        }

        private void OnBusyStateChanged(BusyStateChangedEventArgs e)
        {
            var handler = BusyStateChanged;
            if (handler == null) return;

            var ctx = _syncContext;
            // optional logging removed; consumers may use IBusyIndicator notifications
            if (ctx != null)
            {
                // Dispatch to the captured synchronization context so UI handlers run on the UI thread.
                // Use Send so the caller observes the updated UI state immediately (tests expect synchronous delivery).
                try { ctx.Send(state => handler(this, (BusyStateChangedEventArgs)state!), e); } catch { }
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

        // Public API to satisfy IBusyIndicator.GetActiveScopeIds
        public System.Collections.Generic.IReadOnlyCollection<string> GetActiveScopeIdsSnapshot()
        {
            return GetActiveScopeIds();
        }

        // Implement interface method name
        System.Collections.Generic.IReadOnlyCollection<string> AppMvp.Presentation.Abstractions.IBusyIndicator.GetActiveScopeIds()
        {
            return GetActiveScopeIds();
        }
    }
}
