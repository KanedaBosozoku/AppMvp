using AppMvp.Presentation.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppMvp.Presentation.Navigation
{
    public sealed class FormNavigationPresenter : IFormNavigationPresenter
    {
        private readonly IFormNavigator _navigator;

        public FormNavigationPresenter(IFormNavigator navigator)
        {
            _navigator = navigator ?? throw new ArgumentNullException(nameof(navigator));
        }

        public void NavigateTo(Type formType, object? parameter = null, bool modal = false, Action<object?>? callback = null)
        {
            if (formType == null)
                throw new ArgumentNullException(nameof(formType));

            _navigator.NavigateTo(formType, parameter, modal, callback);
        }
    }
}