using AppMvp.Presentation.Abstractions;
using AppMvp.Presentation.ViewModels;
using AppMvp.UI.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AppMvp.UI.Forms
{
    public partial class MainForm : Form
    {
        private readonly IRegionHost? _regionHost;
        private readonly IRegionNavigationPresenter? _nav;
        private readonly MainFormViewModel? _viewModel;
        private readonly NavigationControl? _navigationControl;
        private readonly NavigationControl? _headerNavigationControl;
        private readonly IBusyIndicator? _busyIndicator;


        // DESIGNER CONSTRUCTOR — must be public and parameterless
        public MainForm()
        {
            InitializeComponent();
            // Do NOT put preview logic here — designer wipes it out
        }

        // RUNTIME CONSTRUCTOR — internal so DI factory can call it
        public MainForm(MainFormViewModel vm, IRegionNavigationPresenter nav, IRegionHost regionHost, IBusyIndicator busyIndicator, Func<NavigationControl> navigationFactory)
        {
            InitializeComponent();

            _regionHost = regionHost;
            _viewModel = vm;
            _nav = nav;
           
            _busyIndicator = busyIndicator;

            // Subscribe to busy indicator changes
            if (_busyIndicator != null)
            {
                _busyIndicator.BusyStateChanged += BusyIndicator_BusyStateChanged;
                this.Disposed += (s, e) => _busyIndicator.BusyStateChanged -= BusyIndicator_BusyStateChanged;
            }
            // Create and add cancel button to status strip and wire click
            toolStripCancelButton = new ToolStripButton();
            toolStripCancelButton.Name = "toolStripCancelButton";
            toolStripCancelButton.Text = "Cancel";
            toolStripCancelButton.Enabled = false;
            toolStripCancelButton.Click += ToolStripCancelButton_Click;
            statusStrip1.Items.Add(toolStripCancelButton);

            ConfigureRegions();

            // Create navigation control for the navigation region via factory
            var navControl = navigationFactory() ?? throw new InvalidOperationException("Navigation factory returned null");
            _navigationControl = navControl;
            _navigationControl.AddNavigationButton("ContentRegion", "People", "PeopleView");
            _navigationControl.AddNavigationButton("ContentRegion", "Empty", "EmptyView");
            pnlNavigationRegion.Controls.Add(_navigationControl);
            _navigationControl.Dock = DockStyle.Top;

            // Create a second NavigationControl for the header region via factory
            var headerNav = navigationFactory() ?? throw new InvalidOperationException("Navigation factory returned null");
            _headerNavigationControl = headerNav;
            _headerNavigationControl.AddNavigationButton("ContentRegion", "People", "PeopleView");
            _headerNavigationControl.AddNavigationButton("ContentRegion", "Empty", "EmptyView");
            pnlHeaderRegion.Controls.Add(_headerNavigationControl);
            _headerNavigationControl.Dock = DockStyle.Top;
        }

        private void BusyIndicator_BusyStateChanged(object? sender, AppMvp.Presentation.Abstractions.BusyStateChangedEventArgs e)
        {
            // BusyIndicatorService now posts events to the captured UI SynchronizationContext,
            // so handlers will run on the UI thread. Just apply the state directly.
            if (this.IsDisposed) return;
            ApplyBusyState(e);
        }

        private void ApplyBusyState(AppMvp.Presentation.Abstractions.BusyStateChangedEventArgs e)
        {
            try
            {
                toolStripStatusLabel1.Text = e.Message ?? (e.IsBusy ? "Working..." : "Ready");
                toolStripProgressBar1.Visible = e.IsBusy;
                toolStripProgressBar1.Style = e.IsBusy ? ProgressBarStyle.Marquee : ProgressBarStyle.Blocks;
                // enable cancel button when busy
                try { toolStripCancelButton.Enabled = e.IsBusy; } catch { }
            }
            catch
            {
                // ignore UI errors during shutdown
            }
        }

        private void ToolStripCancelButton_Click(object? sender, EventArgs e)
        {
            if (this.IsDisposed) return;

            var result = MessageBox.Show(this, "Cancel the current operation?", "Confirm Cancel", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try { _busyIndicator?.RequestCancel(); } catch { }
            }
        }

        private void ConfigureRegions()
        {
            // Skip region registration in designer
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            _regionHost!.RegisterRegion("ContentRegion", pnlContentRegion);
            _regionHost!.RegisterRegion("NavigationRegion", pnlNavigationRegion);
            _regionHost!.RegisterRegion("SidebarRegion", pnlSidebarRegion);
            _regionHost!.RegisterRegion("HeaderRegion", pnlHeaderRegion);
        }
    }
}