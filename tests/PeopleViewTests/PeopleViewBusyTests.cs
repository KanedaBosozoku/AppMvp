using System;
using System.Reflection;
using Xunit;
using AppMvp.Presentation.ViewModels;
using AppMvp.Presentation;

namespace PeopleViewTests
{
    public class PeopleViewBusyTests
    {
        [Fact]
        public void PeopleView_DisablesEditAndRefresh_When_PeopleEditScopeActive()
        {
            StaTestHelper.RunInSta(() =>
            {
                // Arrange
                var fakeBusy = new FakeBusyIndicator();
                var repo = new FakePersonRepository();
                var eventBus = new FakeEventBus();

                var vm = new PeopleViewModel(repo, fakeBusy, eventBus);

                using var view = new AppMvp.UI.Views.PeopleView(vm, fakeBusy);

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
                using (var scope = fakeBusy.Begin("Saving person…", BusyScopes.PeopleEdit))
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
    }
}
