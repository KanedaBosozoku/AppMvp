using AppMvp.Presentation.Abstractions;
using AppMvp.Presentation.ViewModels;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppMvp.UI.Views
{
    public partial class PeopleView : UserControl, IViewWithParameter, AppMvp.Presentation.Abstractions.IAsyncView
    {
        private readonly PeopleViewModel _vm;
        private readonly CancellationTokenSource _cts = new();
        private readonly BindingSource _bindingSource = new();
        private object? _pendingParameter;

        public PeopleView(PeopleViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
            // Bind the BindingList<PersonViewModel> to a BindingSource and set as DataGridView DataSource.
            _bindingSource.DataSource = _vm.People;

            dgvPeople.AutoGenerateColumns = false;
            // map columns (columns created in designer)
            colId.DataPropertyName = nameof(PersonViewModel.Id);
            colName.DataPropertyName = nameof(PersonViewModel.DisplayName);
            ColumnEmail.DataPropertyName = nameof(PersonViewModel.Email);

            dgvPeople.DataSource = _bindingSource;

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
                // DataGridView is bound to the BindingList; it will update automatically.
                if (_pendingParameter is int personId)
                {
                    await _vm.LoadPersonAsync(personId, ct).ConfigureAwait(false);
                    SelectPersonInGrid(personId);
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

        private void SelectPersonInGrid(int personId)
        {
            if (this.IsDisposed) return;
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<int>(SelectPersonInGrid), personId);
                return;
            }

            // Find index in the BindingList
            var index = _vm.People.Select((p, i) => new { p, i }).FirstOrDefault(x => x.p.Id == personId)?.i ?? -1;
            if (index >= 0 && index < dgvPeople.Rows.Count)
            {
                var row = dgvPeople.Rows[index];
                if (row != null)
                {
                    row.Selected = true;
                    dgvPeople.CurrentCell = row.Cells.Cast<DataGridViewCell>().FirstOrDefault(c => c.Visible) ?? row.Cells[0];
                }
            }
        }

        private void Cleanup()
        {
            // 🔥 Your cleanup logic here
            _cts.Cancel();
            _cts.Dispose();
            _bindingSource.Dispose();
        }
    }
}