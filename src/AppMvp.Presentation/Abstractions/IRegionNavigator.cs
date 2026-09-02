using System;
using System.Collections.Generic;
using System.Text;

namespace AppMvp.Presentation.Abstractions
{
    public interface IRegionNavigator
    {
        Task NavigateToRegionAsync(string regionName, string viewKey, object? parameter = null, CancellationToken cancellationToken = default);
    }
}
