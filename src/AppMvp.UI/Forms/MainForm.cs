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
    //public partial class MainForm : Form
    //{
    //    private readonly IRegionHost? _regionHost;
    //    private readonly IRegionNavigationPresenter? _nav;
    //    private readonly MainFormViewModel? _viewModel;

    //    // DESIGNER CONSTRUCTOR
    //    protected MainForm()
    //    {
    //        throw new NotImplementedException("Design-time preview is not implemented yet.");
    //        InitializeComponent();

    //        bool isDesignTime =
    //            LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
    //            (Site?.DesignMode ?? false);

    //        if (isDesignTime)
    //        {
    //            LoadDesignTimePreview();
    //            return;
    //        }
    //    }

    //    // RUNTIME CONSTRUCTOR — NOT PUBLIC
    //    public MainForm(MainFormViewModel vm, IRegionNavigationPresenter nav, IRegionHost regionHost)
    //    {
    //        InitializeComponent();
    //        _regionHost = regionHost;
    //        _viewModel = vm;
    //        _nav = nav;
    //        ConfigureRegions();

    //        this.Shown += async (s, e) =>
    //        {
    //            await _nav!.NavigateToRegionAsync("ContentRegion", "PeopleView");
    //        };
    //    }


    //    private void ConfigureRegions()
    //    {
    //        if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
    //            return;

    //        _regionHost!.RegisterRegion("ContentRegion", pnlContentRegion);
    //        _regionHost!.RegisterRegion("SidebarRegion", pnlSidebarRegion);
    //        _regionHost!.RegisterRegion("HeaderRegion", pnlHeaderRegion);
    //    }

    //    private void LoadDesignTimePreview()
    //    {
    //        throw new NotImplementedException("Design-time preview is not implemented yet.");
    //        // Header preview
    //        pnlHeaderRegion.Controls.Add(new Label
    //        {
    //            Text = "Header Preview",
    //            Dock = DockStyle.Fill,
    //            TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
    //            Font = new System.Drawing.Font("Segoe UI", 14, System.Drawing.FontStyle.Bold)
    //        });

    //        // Sidebar preview
    //        pnlSidebarRegion.Controls.Add(new Label
    //        {
    //            Text = "Sidebar Preview",
    //            Dock = DockStyle.Fill,
    //            TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
    //            Font = new System.Drawing.Font("Segoe UI", 12)
    //        });

    //        // Content preview
    //        pnlContentRegion.Controls.Add(new Label
    //        {
    //            Text = "Content Preview",
    //            Dock = DockStyle.Fill,
    //            AutoSize = false,
    //            TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
    //            Font = new System.Drawing.Font("Segoe UI", 12),
    //            BackColor = System.Drawing.Color.Red
    //        });
    //    }

    //    protected override void OnLoad(EventArgs e)
    //    {
    //        throw new NotImplementedException("Design-time preview is not implemented yet.");
    //        base.OnLoad(e);

    //        bool isDesignTime =
    //            LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
    //            (Site?.DesignMode ?? false);

    //        if (isDesignTime)
    //        {
    //            LoadDesignTimePreview();
    //        }
    //    }


    //}

    public partial class MainForm : Form
    {
        private readonly IRegionHost? _regionHost;
        private readonly IRegionNavigationPresenter? _nav;
        private readonly MainFormViewModel? _viewModel;


        // DESIGNER CONSTRUCTOR — must be public and parameterless
        public MainForm()
        {
            InitializeComponent();
            // Do NOT put preview logic here — designer wipes it out
        }

        // RUNTIME CONSTRUCTOR — internal so DI factory can call it
        public MainForm(MainFormViewModel vm, IRegionNavigationPresenter nav, IRegionHost regionHost)
        {
            InitializeComponent();

            _regionHost = regionHost;
            _viewModel = vm;
            _nav = nav;

            ConfigureRegions();

            this.Shown += async (s, e) =>
            {
                await _nav!.NavigateToRegionAsync("ContentRegion", "PeopleView");
            };
        }

        private void ConfigureRegions()
        {
            // Skip region registration in designer
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
                AutoSize = false,
                Dock = DockStyle.Fill,
                Text = "Header Preview",
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 14, FontStyle.Bold)
            });

            // Sidebar preview
            pnlSidebarRegion.Controls.Add(new Label
            {
                AutoSize = false,
                Dock = DockStyle.Fill,
                Text = "Sidebar Preview",
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 12)
            });

            // Content preview
            pnlContentRegion.Controls.Add(new Label
            {
                AutoSize = false,
                Dock = DockStyle.Fill,
                Text = "Content Preview",
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 12),
                BackColor = Color.LightGray
            });
        }
    }
}