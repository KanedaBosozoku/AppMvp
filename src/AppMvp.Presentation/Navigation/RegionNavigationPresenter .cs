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

        public async System.Threading.Tasks.Task NavigateToRegionAsync(string regionName, string viewKey, object? parameter = null, System.Threading.CancellationToken cancellationToken = default)
        {
            await _regionNavigator.NavigateToRegionAsync(regionName, viewKey, parameter, cancellationToken).ConfigureAwait(false);
        }
    }
}
