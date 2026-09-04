using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using AppMvp.Presentation.Services;
using AppMvp.Presentation;
using AppMvp.Presentation.ViewModels;
using AppMvp.Presentation.Abstractions;

namespace PeopleViewTests
{
    public class PeopleViewUiQueuedTests
    {
        [Fact]
        public void PeopleView_ToolStripButtons_Disabled_When_BusyRaisedFromBackgroundThread()
        {
            StaTestHelper.RunInSta(() =>
            {
                // Arrange - do NOT set a synchronization context on the busy service so the
                // BusyStateChanged handler will be invoked on the background thread and the
                // view will use Control.BeginInvoke to marshal UI updates.
                var busy = new BusyIndicatorService();
                var repo = new FakePersonRepository();
                var eventBus = new FakeEventBus();

                var vm = new PeopleViewModel(repo, busy, eventBus);
                using var view = new AppMvp.UI.Views.PeopleView(vm, busy);

                // Access private ToolStripButtons via reflection
                var type = typeof(AppMvp.UI.Views.PeopleView);
                var fEdit = type.GetField("tsBtnEdit", BindingFlags.Instance | BindingFlags.NonPublic);
                var fRefresh = type.GetField("tsBtnRefresh", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.NotNull(fEdit);
                Assert.NotNull(fRefresh);

                var btnEdit = (System.Windows.Forms.ToolStripButton)fEdit!.GetValue(view)!;
                var btnRefresh = (System.Windows.Forms.ToolStripButton)fRefresh!.GetValue(view)!;

                // Precondition: buttons enabled
                Assert.True(btnEdit.Enabled);
                Assert.True(btnRefresh.Enabled);

                // Begin the busy scope on a background thread so the view's handler will use BeginInvoke
                IBusyScope? createdScope = null;
                var created = new ManualResetEventSlim(false);

                var t = Task.Run(() =>
                {
                    createdScope = busy.Begin("Saving…", BusyScopes.PeopleEdit);
                    created.Set();
                    // keep the scope active until the test disposes it
                });

                // Wait for the scope to be created
                Assert.True(created.Wait(TimeSpan.FromSeconds(2)), "Timed out waiting for background scope to be created");

                // Wait for the PeopleView's queued UI updates to run. We can't block the UI thread
                // directly, so we arrange a probe: queue a delegate on the UI thread that signals
                // a ManualResetEventSlim after all previously queued delegates (including the view's)
                // have executed. Then pump messages until the signal is observed (deterministic).
                var probe = new ManualResetEventSlim(false);
                var probeCtl = new System.Windows.Forms.Control();
                var _ = probeCtl.Handle; // ensure handle
                // Queue signal delegate which will run after the view's BeginInvoke delegates
                probeCtl.BeginInvoke(new Action(() => probe.Set()));

                var sw = System.Diagnostics.Stopwatch.StartNew();
                while (!probe.IsSet && sw.Elapsed < TimeSpan.FromSeconds(2))
                {
                    System.Windows.Forms.Application.DoEvents();
                    Thread.Sleep(1);
                }
                if (!probe.IsSet)
                    throw new TimeoutException("Timed out waiting for UI queued delegates to run");

                // The view's queued delegate should have run and disabled the buttons
                Assert.False(btnEdit.Enabled);
                Assert.False(btnRefresh.Enabled);

                // End scope (dispose) and probe again to wait for re-enable
                createdScope?.Dispose();
                var probe2 = new ManualResetEventSlim(false);
                var probeCtl2 = new System.Windows.Forms.Control();
                var __ = probeCtl2.Handle;
                probeCtl2.BeginInvoke(new Action(() => probe2.Set()));

                sw.Restart();
                while (!probe2.IsSet && sw.Elapsed < TimeSpan.FromSeconds(2))
                {
                    System.Windows.Forms.Application.DoEvents();
                    Thread.Sleep(1);
                }
                if (!probe2.IsSet)
                    throw new TimeoutException("Timed out waiting for UI queued delegates to run after disposing scope");

                Assert.True(btnEdit.Enabled);
                Assert.True(btnRefresh.Enabled);
            });
        }
    }
}
