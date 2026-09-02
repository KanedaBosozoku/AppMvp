using System;
using System.Collections.Generic;
using System.Text;

namespace AppMvp.Presentation.Abstractions
{
    public interface IRegionNavigationPresenter
    {
        System.Threading.Tasks.Task NavigateToRegionAsync(string regionName, string viewKey, object? parameter = null, System.Threading.CancellationToken cancellationToken = default);
    }
}
