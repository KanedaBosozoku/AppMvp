using AppMvp.Presentation;
using AppMvp.Presentation.Abstractions;
using AppMvp.Presentation.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms;
using static System.ComponentModel.Design.ObjectSelectorEditor;


namespace AppMvp.UI.Controls
{
    public partial class NavigationControl : UserControl
    {
        private readonly IRegionNavigator _navigator = null!;
        private readonly IBusyIndicator _busy = null!;
        private readonly AppMvp.ApplicationCore.EventBus.IApplicationEventBus _eventBus = null!;
        private bool _wasCheckedBeforeClick;


        // DESIGNER CONSTRUCTOR
        protected NavigationControl()
        {
            InitializeComponent();

            // Skip runtime logic at design time
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;
        }

        public NavigationControl(IRegionNavigator navigator, IBusyIndicator busy, AppMvp.ApplicationCore.EventBus.IApplicationEventBus eventBus)
        {
            InitializeComponent();
            tsNavigation.Renderer = new ModernNavRenderer();

            _navigator = navigator ?? throw new ArgumentNullException(nameof(navigator));
            _busy = busy ?? throw new ArgumentNullException(nameof(busy));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            // Subscribe to busy state changes so UI can disable controls for relevant scopes (e.g. People.Edit)
            _busy.BusyStateChanged += OnBusyStateChanged;

            SetCheckedd();
        }


        public void AddNavigationButton(string regionName, string text, string viewName)
        {
            var btn = CreateNavButton(regionName, text, viewName);
            tsNavigation.Items.Add(btn);
            SetCheckedd();
        }

        private ToolStripButton CreateNavButton(string regionName, string text, string viewName)
        {
            var btn = new ToolStripButton(text)
            {
                Tag = viewName,
                AutoSize = true
            };

            btn.Click += async (sender, e) =>
            {
                if (!ClickHandler(sender, e))
                {
                    // Publish navigation intent to the application event bus; the registered handler
                    // will perform the actual navigation (decouples the control from navigation implementation).
                    await _eventBus.PublishAsync(new AppMvp.Application.Events.NavigationRequested(regionName, viewName, null));
                }
            };
            btn.MouseDown += ToolStripButton_MouseDown;
            return btn;
        }


        private void SetCheckedd()
        {
            foreach (ToolStripItem item in tsNavigation.Items)
            {
                if (item is ToolStripButton btn)
                {
                    btn.CheckOnClick = true;
                }
            }

        }

        private void ToolStripButton_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var btn = (ToolStripButton?)sender;
                _wasCheckedBeforeClick = btn?.Checked ?? false;
            }
        }


        private bool ClickHandler(object? sender, EventArgs e)
        {
            if (sender is not ToolStripButton)
                return true;

            var clicked = (ToolStripButton)sender;

            // Uncheck all other buttons in the same ToolStrip
            foreach (var item in tsNavigation.Items)
            {
                if (item is ToolStripButton btn && btn != clicked)
                    btn.Checked = false;
            }

            // Ensure the clicked one is checked
            clicked.Checked = true;

            return _wasCheckedBeforeClick;
        }
        private void OnBusyStateChanged(object? sender, BusyStateChangedEventArgs e)
        {
            try
            {
 
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


                tsNavigation.Enabled = !blockActive;
            }
            catch
            {
                // Swallow to avoid bubbling UI-thread exceptions from event handlers
            }
        }
    }
}