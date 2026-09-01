using AppMvp.Presentation.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppMvp.Presentation.Navigation
{
    public sealed class RegionNavigationPresenter : IRegionNavigationPresenter
    {
        private readonly IRegionNavigator _regionNavigator;

        public RegionNavigationPresenter(IRegionNavigator regionNavigator)
        {
            _regionNavigator = regionNavigator;
        }

        public void NavigateToRegion(string regionName, string viewKey, object? parameter = null)
        {
            _regionNavigator.NavigateToRegion(regionName, viewKey, parameter);
        }
    }
}
