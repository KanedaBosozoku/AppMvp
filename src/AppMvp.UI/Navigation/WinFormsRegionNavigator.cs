using AppMvp.Presentation.Abstractions;
using AppMvp.UI.Registry;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppMvp.UI.Navigation
{
    //public sealed class WinFormsRegionNavigator : IRegionNavigator
    //{
    //    private readonly IServiceProvider _provider;
    //    private readonly IRegionHost _regionHost;

    //    public WinFormsRegionNavigator(IServiceProvider provider, IRegionHost regionHost)
    //    {
    //        _provider = provider;
    //        _regionHost = regionHost;
    //    }

    //    public void NavigateToRegion(string regionName, Type viewType, object? parameter)
    //    {
    //        var region = _regionHost.GetRegion(regionName) as Control
    //            ?? throw new InvalidOperationException($"Region '{regionName}' not found.");

    //        var view = _provider.GetRequiredService(viewType) as Control
    //            ?? throw new InvalidOperationException($"View '{viewType.Name}' must be a WinForms Control.");

    //        if (parameter is not null && view is IViewWithParameter receiver)
    //            receiver.ReceiveParameter(parameter);

    //        region.Controls.Clear();
    //        region.Controls.Add(view);
    //        view.Dock = DockStyle.Fill;
    //    }
    //}

    public sealed class WinFormsRegionNavigator : IRegionNavigator
    {
        private readonly IServiceProvider _provider;
        private readonly IRegionHost _regionHost;
        private readonly IViewRegistry _viewRegistry;

        public WinFormsRegionNavigator(IServiceProvider provider, IRegionHost regionHost, IViewRegistry viewRegistry)
        {
            _provider = provider;
            _regionHost = regionHost;
            _viewRegistry = viewRegistry;
        }

        public void NavigateToRegion(string regionName, string viewKey, object? parameter)
        {
            var region = _regionHost.GetRegion(regionName) as Control
                ?? throw new InvalidOperationException($"Region '{regionName}' not found.");

            var viewType = _viewRegistry.Resolve(viewKey);

            var view = (Control)_provider.GetRequiredService(viewType);

            if (parameter is not null && view is IViewWithParameter receiver)
                receiver.ReceiveParameter(parameter);

            region.Controls.Clear();
            region.Controls.Add(view);
            view.Dock = DockStyle.Fill;
        }
    }

}