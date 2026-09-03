using AppMvp.Domain.Entities;
using AppMvp.Domain.Repositories;
using AppMvp.Presentation.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppMvp.UI.Forms
{
    public class PersonEditForm : Form, IFormWithParameter
    {
        private readonly IPersonRepository _repo;
        private readonly AppMvp.Presentation.Abstractions.IBusyIndicator _busy;
        private int _personId;
        private TextBox txtName;
        private TextBox txtEmail;
        private Button btnOk;
        private Button btnCancel;
        private Label lblName;
        private Label lblEmail;
        private ErrorProvider _errors;
        private CancellationTokenSource? _loadCts;

        public PersonEditForm(IPersonRepository repo, AppMvp.Presentation.Abstractions.IBusyIndicator busy)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _busy = busy ?? throw new ArgumentNullException(nameof(busy));
            InitializeComponent();
        }

        public void ReceiveParameter(object parameter)
        {
            if (parameter is int id)
                _personId = id;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            _ = LoadPersonAsync(_personId);
        }

        private async Task LoadPersonAsync(int id)
        {
            _loadCts?.Cancel();
            _loadCts = new CancellationTokenSource();
            using var linked = CancellationTokenSource.CreateLinkedTokenSource(_loadCts.Token);
            try
            {
                var person = await _repo.GetByIdAsync(id, linked.Token);
                if (person != null)
                {
                    txtName.Text = person.Name ?? string.Empty;
                    txtEmail.Text = person.Email ?? string.Empty;
                }
            }
            catch (OperationCanceledException)
            {
                // cancelled - ignore
            }
            catch
            {
                MessageBox.Show(this, "Failed to load person.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeComponent()
        {
            lblName = new Label { Left = 12, Top = 12, Width = 50, Text = "Name:" };
            txtName = new TextBox { Left = 70, Top = 10, Width = 250 };
            lblEmail = new Label { Left = 12, Top = 44, Width = 50, Text = "Email:" };
            txtEmail = new TextBox { Left = 70, Top = 42, Width = 250 };
            btnOk = new Button { Text = "OK", Left = 164, Width = 75, Top = 80, DialogResult = DialogResult.OK };
            btnCancel = new Button { Text = "Cancel", Left = 245, Width = 75, Top = 80, DialogResult = DialogResult.Cancel };

            _errors = new ErrorProvider();
            _errors.BlinkStyle = ErrorBlinkStyle.NeverBlink;

            btnOk.Click += async (s, e) => await SaveAndCloseAsync();

            txtName.TextChanged += (s, e) => ValidateInputs();
            txtEmail.TextChanged += (s, e) => ValidateInputs();

            Controls.Add(lblName);
            Controls.Add(txtName);
            Controls.Add(lblEmail);
            Controls.Add(txtEmail);
            Controls.Add(btnOk);
            Controls.Add(btnCancel);

            AcceptButton = btnOk;
            CancelButton = btnCancel;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new System.Drawing.Size(340, 120);
            Text = "Edit Person";

            ValidateInputs();
        }

        private async Task SaveAndCloseAsync()
        {
            // validate before save
            if (!ValidateInputs())
                return;

            btnOk.Enabled = false;
            try
            {
                // Begin a busy scope for saving so global busy indicator shows progress and can cancel
                using var scope = _busy.Begin("Saving person…", "People.Edit");
                using var linked = System.Threading.CancellationTokenSource.CreateLinkedTokenSource(scope.Token);

                var person = new Person(_personId, txtName.Text.Trim(), txtEmail.Text.Trim());
                await _repo.UpdateAsync(person, linked.Token);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch
            {
                MessageBox.Show(this, "Failed to save person.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (!IsDisposed)
                    btnOk.Enabled = true;
            }
        }

        private bool ValidateInputs()
        {
            bool valid = true;

            // Name is required
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                _errors.SetError(txtName, "Name is required.");
                valid = false;
            }
            else
            {
                _errors.SetError(txtName, string.Empty);
            }

            // Email is optional but if present, validate basic format
            var email = txtEmail.Text?.Trim();
            if (!string.IsNullOrEmpty(email))
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(email);
                    _errors.SetError(txtEmail, string.Empty);
                }
                catch
                {
                    _errors.SetError(txtEmail, "Invalid email address.");
                    valid = false;
                }
            }
            else
            {
                _errors.SetError(txtEmail, string.Empty);
            }

            btnOk.Enabled = valid;
            return valid;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _loadCts?.Cancel();
                _loadCts?.Dispose();
                _errors?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
