using AppMvp.Presentation.Abstractions;
using AppMvp.Presentation.ViewModels;
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

        // DESIGNER CONSTRUCTOR (no DI)
        protected MainForm()
        {
            InitializeComponent();

            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                LoadDesignTimePreview();
                return;
            }
        }

        // RUNTIME CONSTRUCTOR (DI)
        public MainForm(MainFormViewModel vm, IRegionNavigationPresenter nav, IRegionHost regionHost)
        {
            InitializeComponent();
            _regionHost = regionHost ?? throw new ArgumentNullException(nameof(regionHost));
            _viewModel = vm ?? throw new ArgumentNullException(nameof(vm));
            _nav = nav ?? throw new ArgumentNullException(nameof(nav));
            ConfigureRegions();
            LoadInitialRegionContent();
        }

        private void ConfigureRegions()
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            _regionHost!.RegisterRegion("ContentRegion", pnlContentRegion);
            _regionHost!.RegisterRegion("SidebarRegion", pnlSidebarRegion);
            _regionHost!.RegisterRegion("HeaderRegion", pnlHeaderRegion);
        }

        private void LoadDesignTimePreview()
        {
            // Header preview
            pnlHeaderRegion.Controls.Add(new Label
            {
                Text = "Header Preview",
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Font = new System.Drawing.Font("Segoe UI", 14, System.Drawing.FontStyle.Bold)
            });

            // Sidebar preview
            pnlSidebarRegion.Controls.Add(new Label
            {
                Text = "Sidebar Preview",
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Font = new System.Drawing.Font("Segoe UI", 12)
            });

            // Content preview
            pnlContentRegion.Controls.Add(new Label
            {
                Text = "Content Preview",
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Font = new System.Drawing.Font("Segoe UI", 12)
            });
        }

        private void LoadInitialRegionContent()
        {
            // Initial region content: PeopleView
            _nav.NavigateToRegion("ContentRegion", "PeopleView");
        }
    }
}
