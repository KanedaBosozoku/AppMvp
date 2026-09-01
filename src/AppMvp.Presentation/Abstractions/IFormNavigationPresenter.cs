using System;
using System.Collections.Generic;
using System.Text;

namespace AppMvp.Presentation.Abstractions
{
    public interface IFormNavigationPresenter
    {
        void NavigateTo(Type formType, object? parameter = null, bool modal = false, Action<object?>? callback = null);
    }
}