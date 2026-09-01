using System;
using System.Collections.Generic;
using System.Text;

namespace AppMvp.UI.Registry
{
    public interface IViewRegistry
    {
        Type Resolve(string viewKey);
    }
}