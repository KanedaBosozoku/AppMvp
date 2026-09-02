using System;
using System.Threading;

namespace AppMvp.Presentation.Abstractions
{
    public sealed class BusyStateChangedEventArgs : EventArgs
    {
        public bool IsBusy { get; }
        public string? Message { get; }

        public BusyStateChangedEventArgs(bool isBusy, string? message)
        {
            IsBusy = isBusy;
            Message = message;
        }
    }

    public interface IBusyIndicator
    {
        bool IsBusy { get; }
        string? Message { get; }

        IDisposable Begin(string? message = null);
        /// <summary>
        /// Optionally set a SynchronizationContext which the service will use to raise BusyStateChanged events.
        /// Call this from the UI thread (e.g. in Form.Shown) to have events posted to the UI context.
        /// </summary>
        void SetSynchronizationContext(System.Threading.SynchronizationContext? context);

        event EventHandler<BusyStateChangedEventArgs>? BusyStateChanged;
    }
}
