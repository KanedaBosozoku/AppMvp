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
        private readonly IBusyIndicator _busy;
        private readonly CancellationTokenSource _cts = new();
        private readonly BindingSource _bindingSource = new();
        private object? _pendingParameter;
        private CancellationTokenSource? _operationCts;

        public PeopleView(PeopleViewModel vm, IBusyIndicator busy)
        {
            InitializeComponent();
            _vm = vm;
            _busy = busy ?? throw new ArgumentNullException(nameof(busy));
            // Bind the BindingList<PersonViewModel> to a BindingSource and set as DataGridView DataSource.
            _bindingSource.DataSource = _vm.People;

            dgvPeople.AutoGenerateColumns = false;
            // map columns (columns created in designer)
            colId.DataPropertyName = nameof(PersonViewModel.Id);
            colName.DataPropertyName = nameof(PersonViewModel.DisplayName);
            ColumnEmail.DataPropertyName = nameof(PersonViewModel.Email);

            dgvPeople.DataSource = _bindingSource;
            // Wire refresh and edit buttons
            tsBtnRefresh.Click += TsBtnRefresh_Click;
            tsBtnEdit.Click += TsBtnEdit_Click;

            // Do not start loading here. Activation is performed by the region navigator via IAsyncView.ActivateAsync.
            this.Disposed += (s, e) => Cleanup(); // Cancel any ongoing operations when the view is disposed
        }

        private async void TsBtnRefresh_Click(object? sender, EventArgs e)
        {
            // Prevent re-entrancy
            tsBtnRefresh.Enabled = false;

            // Cancel any existing refresh operation
            try
            {
                _operationCts?.Cancel();
                _operationCts?.Dispose();
            }
            catch { }

            _operationCts = new CancellationTokenSource();
            // Begin a named busy scope for this refresh so UI can cancel it specifically
            using var busyScope = _busy.Begin("Refreshing…", "People.Refresh");

            using var linked1 = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token, _operationCts.Token);
            using var linked2 = CancellationTokenSource.CreateLinkedTokenSource(linked1.Token, busyScope.Token);
            try
            {
                // Clear any current selection/current cell first to avoid DataGridView indexing errors
                try
                {
                    if (dgvPeople.CurrentCell != null)
                        dgvPeople.CurrentCell = null;
                }
                catch { }
                dgvPeople.ClearSelection();

                // Detach the DataSource to avoid DataGridView retaining row indices while the underlying
                // BindingList is cleared and repopulated. Re-attach after the data load completes.
                var previousSource = dgvPeople.DataSource;
                try
                {
                    dgvPeople.DataSource = null;
                    await _vm.LoadPeopleAsync(linked2.Token);
                }
                finally
                {
                    // Reattach the binding source on the UI thread (this continuation runs on UI context)
                    dgvPeople.DataSource = previousSource;
                }
            }
            catch (OperationCanceledException)
            {
                // ignore
            }
            catch (Exception)
            {
                // optional: log or show error
            }
            finally
            {
                try { _operationCts?.Dispose(); } catch { }
                _operationCts = null;
                if (!this.IsDisposed)
                    tsBtnRefresh.Enabled = true;
            }
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

        private async void TsBtnEdit_Click(object? sender, EventArgs e)
        {
            // Guard
            if (this.IsDisposed) return;

            // Determine selected person
            var row = dgvPeople.CurrentRow;
            if (row == null)
            {
                MessageBox.Show(this, "Please select a person to edit.", "Edit Person", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (row.DataBoundItem is not AppMvp.Presentation.ViewModels.PersonViewModel pvm)
            {
                MessageBox.Show(this, "Selected item is not a person.", "Edit Person", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            tsBtnEdit.Enabled = false;
            try
            {
                await _vm.EditPersonAsync(pvm.Id);
            }
            catch (OperationCanceledException)
            {
                // ignore
            }
            catch (Exception ex)
            {
                try { MessageBox.Show(this, $"Failed to open editor: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); } catch { }
            }
            finally
            {
                if (!this.IsDisposed)
                    tsBtnEdit.Enabled = true;
            }
        }
    }
}