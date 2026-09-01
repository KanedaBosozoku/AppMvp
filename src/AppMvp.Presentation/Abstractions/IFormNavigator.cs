using System;
using System.Collections.Generic;
using System.Text;

namespace AppMvp.Presentation.Abstractions
{
    public interface IFormNavigator
    {
        void NavigateTo(Type formType, object? parameter, bool modal, Action<object?>? callback);
    }
}
