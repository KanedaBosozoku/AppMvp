using AppMvp.Presentation.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppMvp.UI.Navigation
{
    public sealed class WinFormsRegionHost : IRegionHost
    {
        private readonly Dictionary<string, Control> _regions = new();

        public void RegisterRegion(string regionName, object regionControl)
        {
            if (regionControl is Control control)
                _regions[regionName] = control;
            else
                throw new InvalidOperationException("Region must be a WinForms Control.");
        }

        public object? GetRegion(string regionName)
        {
            _regions.TryGetValue(regionName, out var control);
            return control;
        }
    }
}