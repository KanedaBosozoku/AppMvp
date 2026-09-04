using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using AppMvp.Presentation.Services;
using AppMvp.Presentation;
using AppMvp.Presentation.ViewModels;
using AppMvp.Domain.Entities;
using AppMvp.Domain.Repositories;
using AppMvp.Presentation.Abstractions;

namespace PeopleViewTests
{
    internal class SlowPersonRepository : IPersonRepository
    {
        public Task AddAsync(Person person, CancellationToken token) => Task.CompletedTask;
        public Task DeleteAsync(Person person) => Task.CompletedTask;
        public Task<Person?> GetByIdAsync(int id, CancellationToken token) => Task.FromResult<Person?>(new Person(id, "Test", "test@example.com"));
        public Task<System.Collections.Generic.List<Person>> GetAllAsync(CancellationToken token) => Task.FromResult(new System.Collections.Generic.List<Person>());
        public async Task UpdateAsync(Person person, CancellationToken token)
        {
            // Simulate a slow save so BusyIndicator scope remains active for a short time
            await Task.Delay(300, token).ConfigureAwait(false);
        }
    }

    internal class FakeErrorDialog : AppMvp.Presentation.Abstractions.IErrorDialog
    {
        public System.Threading.Tasks.Task<bool> ShowAsync(string userMessage, Exception? exception = null, System.Collections.Generic.IDictionary<string, string?>? properties = null, string? correlationId = null)
        {
            return System.Threading.Tasks.Task.FromResult(true);
        }
    }

    public class PeopleViewIntegrationTests
    {
        [Fact]
        public void PeopleView_Toolbar_Disabled_While_PersonEditForm_Saves()
        {
            StaTestHelper.RunInSta(() =>
            {
                var busy = new BusyIndicatorService();
                busy.SetSynchronizationContext(System.Threading.SynchronizationContext.Current);

                var repo = new SlowPersonRepository();
                var eventBus = new FakeEventBus();

                var vm = new PeopleViewModel(repo, busy, eventBus);

                var view = new AppMvp.UI.Views.PeopleView(vm, busy);

                // Host in a real form so controls have a window handle
                using var host = new System.Windows.Forms.Form();
                host.Controls.Add(view);
                host.Show();

                // Access private ToolStripButtons via reflection
                var type = typeof(AppMvp.UI.Views.PeopleView);
                var fEdit = type.GetField("tsBtnEdit", BindingFlags.Instance | BindingFlags.NonPublic);
                var fRefresh = type.GetField("tsBtnRefresh", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.NotNull(fEdit);
                Assert.NotNull(fRefresh);

                var btnEdit = (System.Windows.Forms.ToolStripButton)fEdit!.GetValue(view)!;
                var btnRefresh = (System.Windows.Forms.ToolStripButton)fRefresh!.GetValue(view)!;

                Assert.True(btnEdit.Enabled);
                Assert.True(btnRefresh.Enabled);

                // Prepare PersonEditForm
                using var editForm = new AppMvp.UI.Forms.PersonEditForm(repo, busy, new FakeErrorDialog());
                editForm.ReceiveParameter(1);

                // Observer: subscribe to BusyStateChanged and capture button state when People.Edit becomes active
                var observed = new ManualResetEventSlim(false);
                bool observedDisabled = false;

                EventHandler<AppMvp.Presentation.Abstractions.BusyStateChangedEventArgs>? handler = null;
                handler = (s, e) =>
                {
                    try
                    {
                        if (e.IsBusy && e.ActiveScopeIds != null && System.Linq.Enumerable.Contains(e.ActiveScopeIds, BusyScopes.PeopleEdit))
                        {
                            // Handler runs on the UI synchronization context (BusyIndicatorService sends to captured context),
                            // so we can read button states directly.
                            try { observedDisabled = !btnEdit.Enabled && !btnRefresh.Enabled; } catch { observedDisabled = false; }
                            observed.Set();
                        }
                    }
                    catch { }
                };

                busy.BusyStateChanged += handler;

                // Schedule a UI-invoked call to start the save after the dialog is shown
                Task.Run(async () =>
                {
                    // give ShowDialog a moment to start
                    await Task.Delay(50).ConfigureAwait(false);
                    // Queue SaveAndCloseAsync on the form's UI thread
                    editForm.BeginInvoke(new Action(async () =>
                    {
                        // Use reflection to call private SaveAndCloseAsync
                        var mi = editForm.GetType().GetMethod("SaveAndCloseAsync", BindingFlags.Instance | BindingFlags.NonPublic);
                        if (mi != null)
                        {
                            var task = (Task?)mi.Invoke(editForm, null);
                            if (task != null)
                                await task.ConfigureAwait(false);
                        }
                    }));
                });

                // Show modal dialog (blocks this thread until closed)
                editForm.ShowDialog(host);

                // Wait for observer to report
                Assert.True(observed.Wait(TimeSpan.FromSeconds(2)), "Did not observe busy state during save");
                Assert.True(observedDisabled, "Expected toolbar to be disabled while save in progress");

                // Unsubscribe handler
                try { busy.BusyStateChanged -= handler; } catch { }
            });
        }
    }
}
