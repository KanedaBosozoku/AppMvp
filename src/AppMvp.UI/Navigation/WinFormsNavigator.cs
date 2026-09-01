using AppMvp.Presentation.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppMvp.UI.Navigation
{
    public sealed class WinFormsNavigator : IFormNavigator
    {
        private readonly IServiceProvider _provider;

        public WinFormsNavigator(IServiceProvider provider)
        {
            _provider = provider;
        }

        public void NavigateTo(Type formType, object? parameter, bool modal, Action<object?>? callback)
        {
            var form = (Form)_provider.GetRequiredService(formType);

            if (parameter is not null && form is IFormWithParameter receiver)
                receiver.ReceiveParameter(parameter);

            if (modal)
            {
                var result = form.ShowDialog();
                callback?.Invoke(result);
            }
            else
            {
                form.Show();
                callback?.Invoke(null);
            }
        }
    }
}
