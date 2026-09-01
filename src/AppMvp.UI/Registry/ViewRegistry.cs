using System;
using System.Collections.Generic;
using System.Text;

namespace AppMvp.UI.Registry
{
    public sealed class ViewRegistry : IViewRegistry
    {
        private readonly Dictionary<string, Type> _views = new();

        public void Register<TView>(string key) where TView : Control
        {
            _views[key] = typeof(TView);
        }

        public Type Resolve(string viewKey)
        {
            if (!_views.TryGetValue(viewKey, out var type))
                throw new InvalidOperationException($"View '{viewKey}' is not registered.");

            return type;
        }
    }
}