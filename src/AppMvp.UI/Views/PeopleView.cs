using AppMvp.Presentation.Abstractions;
using AppMvp.Presentation;
using AppMvp.Presentation.ViewModels;
using System;
using System.ComponentModel;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppMvp.UI.Views
{
    public partial class PeopleView : UserControl, IViewWithParameter, AppMvp.Presentation.Abstractions.IAsyncView
    {
        private readonly int _instanceId = System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(new object());
        private readonly PeopleViewModel _vm;
        private readonly IBusyIndicator _busy;
        private readonly Microsoft.Extensions.Logging.ILogger<PeopleView>? _logger;
        private readonly CancellationTokenSource _cts = new();
        private readonly BindingSource _bindingSource = new();
        private object? _pendingParameter;
        private CancellationTokenSource? _operationCts;

        public PeopleView(PeopleViewModel vm, IBusyIndicator busy, Microsoft.Extensions.Logging.ILogger<PeopleView>? logger = null)
        {
            InitializeComponent();
            try { _logger?.LogDebug("PeopleView.ctor Instance={InstanceId} Thread={ThreadId}", _instanceId, Environment.CurrentManagedThreadId); } catch { }
            _vm = vm;
            _busy = busy ?? throw new ArgumentNullException(nameof(busy));
            _logger = logger;
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
            // Subscribe to busy state changes so UI can disable controls for relevant scopes (e.g. People.Edit)
            _busy.BusyStateChanged += OnBusyStateChanged;
            // Initialize UI state from current busy snapshot in case a scope is already active
            try
            {
                var ids = _busy.GetActiveScopeIds();
                OnBusyStateChanged(this, new BusyStateChangedEventArgs(_busy.IsBusy, _busy.Message, ids));
            }
            catch { }
            // Ensure we unsubscribe when disposed
            this.Disposed += (s, e) => { try { _busy.BusyStateChanged -= OnBusyStateChanged; } catch { } };
        }

        private async void TsBtnRefresh_Click(object? sender, EventArgs e)
        {
            // Prevent re-entrancy
            EnableToolStripButtons(false);
            // Cancel any existing refresh operation
            try
            {
                _operationCts?.Cancel();
                _operationCts?.Dispose();
            }
            catch { }

            _operationCts = new CancellationTokenSource();
            // Begin a named busy scope for this refresh so UI can cancel it specifically
            using var busyScope = _busy.Begin("Refreshing…", BusyScopes.PeopleRefresh);

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
                //if (!this.IsDisposed)
                    //EnableToolStripButtons(true);
            }
        }

        public async System.Threading.Tasks.Task ActivateAsync(System.Threading.CancellationToken cancellationToken = default)
        {
            try
            {
                using var linked = System.Threading.CancellationTokenSource.CreateLinkedTokenSource(_cts.Token, cancellationToken);
                var ct = linked.Token;
                //(false);
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
            finally
            {
                //EnableToolStripButtons(true);
            }
        }


        private void EnableToolStripButtons(bool enable)
        {
            if (this.IsDisposed) return;
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<bool>(EnableToolStripButtons), enable);
                return;
            }
            tsBtnRefresh.Enabled = enable;
            tsBtnEdit.Enabled = enable;
        }

        private void OnBusyStateChanged(object? sender, BusyStateChangedEventArgs e)
        {
            try
            {
                // Log busy state for easier runtime verification
                try { _logger?.LogDebug("OnBusyStateChanged: IsBusy={IsBusy}, Message={Message}, ActiveScopeIds=[{ActiveScopeIds}]", e.IsBusy, e.Message, e.ActiveScopeIds == null ? string.Empty : string.Join(",", e.ActiveScopeIds)); } catch { }

                // When People.Edit or People.Refresh scopes are active, disable edit/refresh; otherwise enable.
                var blockActive = e.ActiveScopeIds != null && (
                    e.ActiveScopeIds.Contains(BusyScopes.PeopleEdit) ||
                    e.ActiveScopeIds.Contains(BusyScopes.PeopleRefresh)
                );

                // Defensive fallback: treat messages mentioning 'save', 'load' or 'refresh' as blocking
                // operations so UI disables controls even if scope ids are not propagated in some callers.
                if (!blockActive && e.IsBusy && !string.IsNullOrEmpty(e.Message))
                {
                    var msg = e.Message;
                    if (msg.IndexOf("save", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        msg.IndexOf("load", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        msg.IndexOf("refresh", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        blockActive = true;
                    }
                }

                try { _logger?.LogDebug("PeopleView.SetButtons Thread={ThreadId} blockActive={BlockActive} IsBusy={IsBusy} Message={Message} ActiveScopeIds=[{ActiveScopeIds}]", Environment.CurrentManagedThreadId, blockActive, e.IsBusy, e.Message, string.Join(',', e.ActiveScopeIds ?? Array.Empty<string>())); } catch { }
                EnableToolStripButtons(!blockActive);
            }
            catch
            {
                // Swallow to avoid bubbling UI-thread exceptions from event handlers
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

            EnableToolStripButtons(false);
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
                // Do not force re-enable here. BusyStateChanged is the source-of-truth for
                // enabling/disabling the toolbar based on active busy scopes (e.g. People.Edit).
                // Rely on the BusyIndicatorService notifications to restore UI state.
                try { _logger?.LogDebug("TsBtnEdit_Click finally: IsDisposed={IsDisposed}, BusyIsBusy={BusyIsBusy}", this.IsDisposed, _busy.IsBusy); } catch { }
            }
        }
    }
}