using System;
using System.Collections.Generic;
using System.Text;

namespace AppMvp.Presentation.Abstractions
{
    public interface IRegionNavigator
    {
        void NavigateToRegion(string regionName, string viewKey, object? parameter = null);
    }
}
