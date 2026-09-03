using AppMvp.Presentation.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AppMvp.UI.Services
{
    /// <summary>
    /// UI dispatcher that lazily captures a SynchronizationContext the first time it is needed.
    /// This is safer when DI/service construction timing is uncertain: the context will be
    /// taken from the calling thread on first use (or a WindowsFormsSynchronizationContext will
    /// be created if none is available).
    /// </summary>
    public sealed class UiDispatcher : IUiDispatcher
    {
        private SynchronizationContext? _ctx;
        private readonly Microsoft.Extensions.Logging.ILogger<UiDispatcher>? _logger;

        public UiDispatcher(Microsoft.Extensions.Logging.ILogger<UiDispatcher>? logger = null)
        {
            _ctx = null;
            _logger = logger;
        }

        private SynchronizationContext EnsureContext()
        {
            var ctx = Volatile.Read(ref _ctx);
            if (ctx != null) return ctx;

            // Try to capture the current context; if none, create a WindowsForms one.
            var current = SynchronizationContext.Current;
            if (current == null)
            {
                var wf = new System.Windows.Forms.WindowsFormsSynchronizationContext();
                var prev0 = Interlocked.CompareExchange(ref _ctx, wf, null);
                _logger?.LogDebug("UiDispatcher captured WindowsFormsSynchronizationContext as fallback.");
                return prev0 ?? wf;
            }

            // If we captured a context that's not the usual UI contexts, warn in debug.
            var typeName = current.GetType().FullName ?? current.GetType().Name;
            var isWf = current is System.Windows.Forms.WindowsFormsSynchronizationContext;
            var isWpf = current.GetType().FullName?.StartsWith("System.Windows.Threading.DispatcherSynchronizationContext") ?? false;
            if (!isWf && !isWpf)
            {
                _logger?.LogWarning("UiDispatcher captured a non-UI SynchronizationContext: {ContextType}. This may cause UI dispatches to run on a non-UI thread.", typeName);
            }

            var prev = Interlocked.CompareExchange(ref _ctx, current, null);
            return prev ?? current;
        }

        public void BeginInvoke(Action action)
        {
            var ctx = EnsureContext();
            ctx.Post(_ => action(), null);
        }

        public void BeginInvoke(Func<Task> action)
        {
            var ctx = EnsureContext();
            ctx.Post(async _ =>
            {
                try { await action().ConfigureAwait(false); } catch { }
            }, null);
        }
    }
}
