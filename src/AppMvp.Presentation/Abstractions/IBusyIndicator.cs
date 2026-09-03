using System;
using System.Threading;

namespace AppMvp.Presentation.Abstractions
{
    public sealed class BusyStateChangedEventArgs : EventArgs
    {
        public bool IsBusy { get; }
        public string? Message { get; }
        public IReadOnlyCollection<string> ActiveScopeIds { get; }

        public BusyStateChangedEventArgs(bool isBusy, string? message, IReadOnlyCollection<string>? activeScopeIds = null)
        {
            IsBusy = isBusy;
            Message = message;
            ActiveScopeIds = activeScopeIds ?? Array.Empty<string>();
        }
    }

    public interface IBusyIndicator
    {
        bool IsBusy { get; }
        string? Message { get; }

        /// <summary>
        /// Begin a busy scope. Returns an IBusyScope whose Token can be linked with operation CancellationTokenSources.
        /// Optionally provide a scopeId to allow targeted cancellation.
        /// </summary>
        IBusyScope Begin(string? message = null, string? scopeId = null);

        /// <summary>
        /// Request cancellation for all active busy scopes or for a specific scope id when provided.
        /// </summary>
        void RequestCancel(string? scopeId = null);

        /// <summary>
        /// Optionally set a SynchronizationContext which the service will use to raise BusyStateChanged events.
        /// Call this from the UI thread (e.g. in Program.Main) to have events posted to the UI context.
        /// </summary>
        void SetSynchronizationContext(System.Threading.SynchronizationContext? context);

        event EventHandler<BusyStateChangedEventArgs>? BusyStateChanged;
    }
}
