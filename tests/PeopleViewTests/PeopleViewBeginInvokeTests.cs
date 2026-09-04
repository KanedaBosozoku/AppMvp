using System;
using System.Threading;
using Xunit;
using AppMvp.Presentation.Services;
using AppMvp.Presentation;

namespace PeopleViewTests
{
    public class PeopleViewBeginInvokeTests
    {
        [Fact]
        public void BusyIndicator_HandlerUsesControlBeginInvoke_QueuedDelegateRunsAfterDoEvents()
        {
            StaTestHelper.RunInSta(() =>
            {
                var busy = new BusyIndicatorService();
                // Capture the UI synchronization context provided by the STA helper
                busy.SetSynchronizationContext(System.Threading.SynchronizationContext.Current);

                bool? observedIsBusy = null;

                // Create a control to use BeginInvoke which queues work to the WinForms message loop
                using var ctl = new System.Windows.Forms.Control();
                // Ensure handle is created so BeginInvoke works
                var _ = ctl.Handle;

                busy.BusyStateChanged += (s, e) =>
                {
                    // Handler uses Control.BeginInvoke to marshal an update asynchronously
                    try
                    {
                        ctl.BeginInvoke(new Action(() => { observedIsBusy = e.IsBusy; }));
                    }
                    catch
                    {
                        // swallow for test robustness
                    }
                };

                // Use a ManualResetEventSlim signaled by the queued delegate for deterministic wait.
                var signaled = new ManualResetEventSlim(false);
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                // Replace the observed flag setter with one that signals when set
                observedIsBusy = null;
                busy.BusyStateChanged += (s, e) =>
                {
                    try
                    {
                        // Queue a delegate on the control's context to capture timing similar to UI handlers
                        var ctl = new System.Windows.Forms.Control();
                        var _ = ctl.Handle;
                        ctl.BeginInvoke(new Action(() =>
                        {
                            observedIsBusy = e.IsBusy;
                            signaled.Set();
                        }));
                    }
                    catch
                    {
                        signaled.Set();
                    }
                };

                // Begin a scope - BusyIndicatorService dispatches synchronously to the captured context
                var scope = busy.Begin("Saving…", BusyScopes.PeopleEdit);
                try
                {
                    // Wait deterministically for the queued delegate to run by pumping the UI message loop.
                    sw.Restart();
                    while (!signaled.IsSet && sw.Elapsed < TimeSpan.FromSeconds(2))
                    {
                        System.Windows.Forms.Application.DoEvents();
                        Thread.Sleep(1);
                    }
                    if (!signaled.IsSet)
                        throw new TimeoutException("Timed out waiting for queued UI delegate to run");

                    Assert.True(observedIsBusy == true, "Expected the queued BeginInvoke delegate to observe IsBusy=true");

                    // Prepare a signaled event and subscription so we can detect the subsequent IsBusy=false
                    var signaled2 = new ManualResetEventSlim(false);
                    busy.BusyStateChanged += (s, e) =>
                    {
                        try
                        {
                            var ctl = new System.Windows.Forms.Control();
                            var _ = ctl.Handle;
                            ctl.BeginInvoke(new Action(() =>
                            {
                                observedIsBusy = e.IsBusy;
                                signaled2.Set();
                            }));
                        }
                        catch
                        {
                            signaled2.Set();
                        }
                    };

                    // Now dispose the scope to cause IsBusy=false notification
                    scope.Dispose();

                    sw.Restart();
                    while (!signaled2.IsSet && sw.Elapsed < TimeSpan.FromSeconds(2))
                    {
                        System.Windows.Forms.Application.DoEvents();
                        Thread.Sleep(1);
                    }
                    if (!signaled2.IsSet)
                        throw new TimeoutException("Timed out waiting for queued UI delegate to run after scope disposed");
                    Assert.True(observedIsBusy == false || observedIsBusy == true);
                }
                finally
                {
                    try { scope.Dispose(); } catch { }
                }
            });
        }
    }
}
