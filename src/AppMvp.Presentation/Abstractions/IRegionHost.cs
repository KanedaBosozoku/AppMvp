using System;
using System.Collections.Generic;
using System.Text;

namespace AppMvp.Presentation.Abstractions
{
    public interface IRegionHost
    {
        void RegisterRegion(string regionName, object regionControl);
        object? GetRegion(string regionName);
    }
}
