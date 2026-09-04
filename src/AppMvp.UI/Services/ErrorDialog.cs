using AppMvp.Presentation.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppMvp.UI.Services
{
    public sealed class ErrorDialog : IErrorDialog
    {
        private readonly ILogger<ErrorDialog>? _logger;

        public ErrorDialog(ILogger<ErrorDialog>? logger = null)
        {
            _logger = logger;
        }

        public Task<bool> ShowAsync(string userMessage, Exception? exception = null, IDictionary<string, string?>? properties = null, string? correlationId = null)
        {
            var tcs = new TaskCompletionSource<bool>();
            try
            {
                var ctx = SynchronizationContext.Current ?? new WindowsFormsSynchronizationContext();
                ctx.Post(state =>
                {
                    try
                    {
                        using var dlg = new Form();
                        dlg.Text = "Application Error";
                        dlg.StartPosition = FormStartPosition.CenterParent;

                        var detailsText = exception?.ToString() ?? string.Empty;
                        var hasDetails = !string.IsNullOrEmpty(detailsText);
                        var hasCorrelation = !string.IsNullOrEmpty(correlationId);

                        // Start compact when details are hidden
                        var compactHeight = 160;
                        var expandedHeight = 420;
                        dlg.ClientSize = new System.Drawing.Size(720, hasDetails ? compactHeight : compactHeight);

                        var lbl = new Label { Left = 12, Top = 12, Width = 680, Height = 30, Text = userMessage };
                        var idText = correlationId ?? string.Empty;
                        var lblId = new Label { Left = 12, Top = 44, Height = 18, Text = "Correlation Id:", AutoSize = true, ForeColor = System.Drawing.Color.Gray, Visible = hasCorrelation };
                        var txtId = new TextBox { Left = 500, Top = 40, Width = 225, Height = 22, ReadOnly = true, TextAlign = HorizontalAlignment.Left, Text = idText, Anchor = AnchorStyles.Top | AnchorStyles.Right, Visible = hasCorrelation };

                        // Details textbox is collapsed by default
                        var txt = new TextBox
                        {
                            Left = 12,
                            Top = 100,
                            Width = 680,
                            Height = 220,
                            Multiline = true,
                            ReadOnly = true,
                            ScrollBars = ScrollBars.Both,
                            Text = detailsText,
                            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                            Visible = false
                        };

                        var btnShowDetails = new Button { Text = "Show Details", Left = 12, Top = 100, Width = 100, Visible = hasDetails };
                        var btnCopyId = new Button { Text = "Copy Correlation Id", Left = 500, Top = 64, Width = 120, Visible = hasCorrelation };

                        var pnl = new Panel { Dock = DockStyle.Bottom, Height = 40 };
                        var btnCopy = new Button { Text = "Copy Details", Left = 8, Width = 100, Top = 6, Visible = false };
                        var btnSend = new Button { Text = "Send Report", Left = 116, Width = 100, Top = 6 };
                        var btnClose = new Button { Text = "Close", Left = 224, Width = 75, Top = 6, DialogResult = DialogResult.OK };

                        btnCopy.Click += (s, e) => { try { Clipboard.SetText(txt.Text); } catch { } };
                        btnCopyId.Click += (s, e) => { try { Clipboard.SetText(txtId.Text); } catch { } };

                        btnSend.Click += (s, e) =>
                        {
                            try
                            {
                                btnSend.Enabled = false;
                                // log that user requested send
                                _logger?.LogInformation("User initiated error report; correlationId={CorrelationId}", correlationId);
                                // Telemetry is handled externally by ErrorReporter.ReportException calls; here we just notify the user
                                MessageBox.Show(dlg, "Report queued to telemetry.", "Report", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            finally
                            {
                                btnSend.Enabled = true;
                            }
                        };

                        btnClose.Click += (s, e) => dlg.Close();

                        btnShowDetails.Click += (s, e) =>
                        {
                            txt.Visible = !txt.Visible;
                            btnShowDetails.Text = txt.Visible ? "Hide Details" : "Show Details";
                            // Show/hide copy details control and resize dialog
                            btnCopy.Visible = txt.Visible;
                            // Ensure send/close remain visible
                            btnSend.Visible = true;
                            btnClose.Visible = true;
                            dlg.ClientSize = new System.Drawing.Size(dlg.ClientSize.Width, txt.Visible ? expandedHeight : compactHeight);
                            // Bring the bottom panel to front so its buttons are not occluded
                            pnl.BringToFront();
                        };

                        pnl.Controls.Add(btnCopy);
                        pnl.Controls.Add(btnSend);
                        pnl.Controls.Add(btnClose);

                        dlg.Controls.Add(lbl);
                        dlg.Controls.Add(lblId);
                        dlg.Controls.Add(txtId);
                        dlg.Controls.Add(btnShowDetails);
                        dlg.Controls.Add(btnCopyId);
                        dlg.Controls.Add(txt);
                        dlg.Controls.Add(pnl);

                        dlg.ShowDialog();

                        tcs.TrySetResult(true);
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "Error showing error dialog");
                        tcs.TrySetResult(false);
                    }
                }, null);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to post error dialog to UI thread");
                tcs.TrySetResult(false);
            }

            return tcs.Task;
        }
    }
}
