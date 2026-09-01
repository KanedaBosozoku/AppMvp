using System;
using System.Collections.Generic;
using System.Text;

namespace AppMvp.Presentation.Abstractions
{
    public interface IRegionNavigationPresenter
    {
        void NavigateToRegion(string regionName, string viewKey, object? parameter = null);
    }
}
