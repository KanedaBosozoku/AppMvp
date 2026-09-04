using System;
using System.Reflection;
using Xunit;
using AppMvp.Presentation.ViewModels;
using AppMvp.Presentation.Services;
using AppMvp.Presentation;

namespace PeopleViewTests
{
    public class PeopleViewBusyServiceTests
    {
        [Fact]
            public void PeopleView_DisablesEditAndRefresh_When_PeopleEditScopeActive_WithRealBusyService()
            {
                StaTestHelper.RunInSta(() =>
                {
                    // Arrange - use the real BusyIndicatorService
                    var busy = new BusyIndicatorService();
                    // Capture the test thread's synchronization context so the service posts events to the test UI context
                    busy.SetSynchronizationContext(System.Threading.SynchronizationContext.Current);
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

                // Act: begin a People.Edit scope
                using (var scope = busy.Begin("Saving person…", BusyScopes.PeopleEdit))
                {
                    // Assert: buttons are disabled
                    Assert.False(btnEdit.Enabled);
                    Assert.False(btnRefresh.Enabled);
                }

                // After disposing the scope buttons should be re-enabled
                Assert.True(btnEdit.Enabled);
                Assert.True(btnRefresh.Enabled);
            });
        }

        [Fact]
            public void PeopleView_OverlappingScopes_PeopleRefreshAndPeopleEdit_FinalStateIsCorrect()
            {
                StaTestHelper.RunInSta(() =>
                {
                    // Arrange - use the real BusyIndicatorService
                    var busy = new BusyIndicatorService();
                    // Capture the test thread's synchronization context so the service posts events to the test UI context
                    busy.SetSynchronizationContext(System.Threading.SynchronizationContext.Current);
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

                // Act: start a refresh scope first
                var refreshScope = busy.Begin("Refreshing…", BusyScopes.PeopleRefresh);
                try
                {
                    // With the new UI policy refresh is considered blocking: buttons should be disabled
                    Assert.False(btnEdit.Enabled);
                    Assert.False(btnRefresh.Enabled);

                    // Now start an edit scope overlapping the refresh
                    var editScope = busy.Begin("Saving person…", BusyScopes.PeopleEdit);
                    try
                    {
                        // While edit scope active: buttons should be disabled
                        Assert.False(btnEdit.Enabled);
                        Assert.False(btnRefresh.Enabled);
                    }
                    finally
                    {
                        // End edit scope
                        editScope.Dispose();
                    }

                    // After disposing edit scope, refresh scope is still active: buttons should remain disabled
                    Assert.False(btnEdit.Enabled);
                    Assert.False(btnRefresh.Enabled);
                }
                finally
                {
                    refreshScope.Dispose();
                }
            });
        }
    }
}
