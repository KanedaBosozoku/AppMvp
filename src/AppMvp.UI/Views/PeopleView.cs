using AppMvp.Presentation.Abstractions;
using AppMvp.Presentation.ViewModels;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppMvp.UI.Views
{
    public partial class PeopleView : UserControl, IViewWithParameter, AppMvp.Presentation.Abstractions.IAsyncView
    {
        private readonly PeopleViewModel _vm;
        private readonly CancellationTokenSource _cts = new();
        private object? _pendingParameter;

        public PeopleView(PeopleViewModel vm)
        {
            InitializeComponent();
            _vm = vm;

            // Subscribe to collection changes so the ListView stays in sync.
            _vm.People.ListChanged += People_ListChanged;

            // Do not start loading here. Activation is performed by the region navigator via IAsyncView.ActivateAsync.
            this.Disposed += (s, e) => Cleanup(); // Cancel any ongoing operations when the view is disposed
        }

        public async System.Threading.Tasks.Task ActivateAsync(System.Threading.CancellationToken cancellationToken = default)
        {
            try
            {
                using var linked = System.Threading.CancellationTokenSource.CreateLinkedTokenSource(_cts.Token, cancellationToken);
                var ct = linked.Token;

                await _vm.LoadPeopleAsync(ct).ConfigureAwait(false);

                // Update UI with loaded people
                PopulateListView();

                // If a parameter was supplied prior to activation, handle it now
                if (_pendingParameter is int personId)
                {
                    await _vm.LoadPersonAsync(personId, ct).ConfigureAwait(false);
                    SelectPersonInList(personId);
                }
            }
            catch (OperationCanceledException)
            {
                // ignore — activation was cancelled
            }
        }




        public void ReceiveParameter(object parameter)
        {
            // Store parameter so activation can process it (avoids async void).
            _pendingParameter = parameter;
        }

        private void People_ListChanged(object? sender, ListChangedEventArgs e)
        {
            // Keep UI updates on the UI thread
            if (this.IsDisposed) return;
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(PopulateListView));
            }
            else
            {
                PopulateListView();
            }
        }

        private void PopulateListView()
        {
            if (this.IsDisposed) return;
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(PopulateListView));
                return;
            }

            lstPeople.BeginUpdate();
            lstPeople.Items.Clear();
            foreach (var p in _vm.People)
            {
                var item = new ListViewItem(p.DisplayName) { Tag = p.Id };
                item.SubItems.Add(p.Email);
                lstPeople.Items.Add(item);
            }
            lstPeople.EndUpdate();
        }

        private void SelectPersonInList(int personId)
        {
            if (this.IsDisposed) return;
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<int>(SelectPersonInList), personId);
                return;
            }

            var item = lstPeople.Items.Cast<ListViewItem>().FirstOrDefault(i => (int?)i.Tag == personId || (i.Tag is int t && t == personId));
            if (item != null)
            {
                item.Selected = true;
                item.Focused = true;
                item.EnsureVisible();
            }
        }

        private void Cleanup()
        {
            // 🔥 Your cleanup logic here
            _cts.Cancel();
            _cts.Dispose();
            _vm.People.ListChanged -= People_ListChanged;
        }
    }
}