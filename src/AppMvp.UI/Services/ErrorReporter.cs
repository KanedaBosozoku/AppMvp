using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Windows.Forms;

namespace AppMvp.UI.Services
{
    public static class ErrorReporter
    {
        public static AppMvp.Presentation.Abstractions.IErrorTelemetry? TelemetryClient { get; private set; }

        public static void SetTelemetryClient(AppMvp.Presentation.Abstractions.IErrorTelemetry? client)
        {
            TelemetryClient = client;
        }

        public static AppMvp.Presentation.Abstractions.IErrorDialog? ErrorDialog { get; private set; }

        public static void SetErrorDialog(AppMvp.Presentation.Abstractions.IErrorDialog? dialog)
        {
            ErrorDialog = dialog;
        }

        /// <summary>
        /// Show a centralized error dialog with a user-friendly message, technical details,
        /// and the ability to send a report (which will include a correlation id).
        /// </summary>
        // Returns a tuple (dialogShownOk, correlationId)
        public static Task<(bool Success, string CorrelationId)> ShowErrorAsync(string userMessage, Exception? ex = null, ILogger? logger = null, System.Collections.Generic.IDictionary<string, string?>? properties = null)
        {
            // If a DI-provided error dialog is available, prefer it
            var correlationId = Guid.NewGuid().ToString("N");
            var props = new System.Collections.Generic.Dictionary<string, string?>();
            if (properties != null)
            {
                foreach (var kv in properties)
                    props[kv.Key] = kv.Value;
            }
            props["CorrelationId"] = correlationId;
            props["UserMessage"] = userMessage;

            // Log and report to telemetry
            if (ex != null)
                logger?.LogError(ex, "User error: {Message}", userMessage);
            else
                logger?.LogInformation("User error: {Message}", userMessage);

            try { TelemetryClient?.ReportException(ex, props); } catch { }

            if (ErrorDialog != null)
            {
                try
                {
                    // Show dialog through DI-provided implementation and return correlation id
                    var task = ErrorDialog.ShowAsync(userMessage, ex, props, correlationId);
                    return task.ContinueWith(t => (t.IsCompletedSuccessfully && t.Result, correlationId));
                }
                catch (Exception ex2)
                {
                    logger?.LogError(ex2, "ErrorDialog failed, falling back to inline dialog");
                }
            }

            // Fallback: simple inline dialog if no DI dialog is available
            var tcs = new TaskCompletionSource<(bool, string)>();
            try
            {
                var ctx = System.Threading.SynchronizationContext.Current ?? new WindowsFormsSynchronizationContext();
                ctx.Post(state =>
                {
                    try
                    {
                        using var dlg = new Form();
                        dlg.Text = "Application Error";
                        dlg.StartPosition = FormStartPosition.CenterParent;
                        dlg.ClientSize = new System.Drawing.Size(720, 420);

                        var lbl = new Label { Left = 12, Top = 12, Width = 680, Height = 30, Text = userMessage };
                        var lblId = new Label { Left = 12, Top = 44, Width = 680, Height = 18, Text = string.IsNullOrEmpty(correlationId) ? string.Empty : $"Correlation Id: {correlationId}", ForeColor = System.Drawing.Color.Gray };

                        var txt = new TextBox
                        {
                            Left = 12,
                            Top = 68,
                            Width = 680,
                            Height = 300,
                            Multiline = true,
                            ReadOnly = true,
                            ScrollBars = ScrollBars.Both,
                            Text = ex?.ToString() ?? string.Empty,
                            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
                        };

                        var pnl = new Panel { Dock = DockStyle.Bottom, Height = 40 };
                        var btnCopy = new Button { Text = "Copy Details", Left = 8, Width = 100, Top = 6 };
                        var btnClose = new Button { Text = "Close", Left = 116, Width = 75, Top = 6, DialogResult = DialogResult.OK };

                        btnCopy.Click += (s, e) => { try { Clipboard.SetText(txt.Text); } catch { } };
                        btnClose.Click += (s, e) => dlg.Close();

                        pnl.Controls.Add(btnCopy);
                        pnl.Controls.Add(btnClose);

                        dlg.Controls.Add(lbl);
                        dlg.Controls.Add(lblId);
                        dlg.Controls.Add(txt);
                        dlg.Controls.Add(pnl);

                        dlg.ShowDialog();

                        tcs.TrySetResult((true, correlationId));
                    }
                    catch (Exception)
                    {
                        tcs.TrySetResult((false, correlationId));
                    }
                }, null);
            }
            catch
            {
                tcs.TrySetResult((false, correlationId));
            }

            return tcs.Task;
        }

        /// <summary>
        /// Helper that shows the error dialog (or delegates to the DI-provided dialog) and logs the correlation id
        /// using the provided logger. Returns true when the dialog completed normally.
        /// </summary>
        public static async Task<bool> ShowErrorAndLogAsync(string userMessage, Exception? ex = null, ILogger? logger = null, System.Collections.Generic.IDictionary<string, string?>? properties = null)
        {
            try
            {
                var (ok, correlationId) = await ShowErrorAsync(userMessage, ex, logger, properties).ConfigureAwait(false);
                try
                {
                    if (logger != null)
                    {
                        if (ex != null)
                            logger.LogInformation(ex, "Error dialog shown to user; correlationId={CorrelationId}", correlationId);
                        else
                            logger.LogInformation("Error dialog shown to user; correlationId={CorrelationId}", correlationId);
                    }
                }
                catch { }

                return ok;
            }
            catch
            {
                // If showing the dialog fails, ensure we don't throw from the helper
                return false;
            }
        }

        public static void ShowException(Exception? ex, ILogger? logger = null)
        {
            try
            {
                if (ex != null)
                    logger?.LogError(ex, "An unhandled exception occurred");

                // Report to telemetry if configured
                try { TelemetryClient?.ReportException(ex, null); } catch { }

                // Ensure we show the dialog on the UI thread
                var ctx = SynchronizationContext.Current ?? new WindowsFormsSynchronizationContext();
                ctx.Post(state =>
                {
                    try
                    {
                        var exception = state as Exception;
                        var message = exception?.Message ?? "An unexpected error occurred.";
                        var detail = exception?.ToString() ?? string.Empty;

                        var text = message + "\n\nSee details?";
                        var result = MessageBox.Show(text, "Application Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                        if (result == DialogResult.Yes)
                        {
                            // Show details in another dialog
                            ShowDetailsDialog(detail);
                        }
                    }
                    catch { }
                }, ex);
            }
            catch { }
        }

        private static void ShowDetailsDialog(string details)
        {
            try
            {
                using var dlg = new Form();
                dlg.Text = "Error Details";
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.ClientSize = new System.Drawing.Size(700, 500);

                var txt = new TextBox
                {
                    Multiline = true,
                    ReadOnly = true,
                    ScrollBars = ScrollBars.Both,
                    Dock = DockStyle.Fill,
                    Text = details
                };

                var pnl = new Panel { Dock = DockStyle.Bottom, Height = 36 };

                var btnCopy = new Button { Text = "Copy Details", Left = 8, Width = 100, Top = 6 };
                btnCopy.Click += (s, e) =>
                {
                    try { Clipboard.SetText(details); } catch { }
                };

                var btnClose = new Button { Text = "Close", Left = 116, Width = 75, Top = 6 };
                btnClose.Click += (s, e) => dlg.Close();

                pnl.Controls.Add(btnCopy);
                pnl.Controls.Add(btnClose);

                dlg.Controls.Add(txt);
                dlg.Controls.Add(pnl);
                dlg.ShowDialog();
            }
            catch { }
        }
    }
}
