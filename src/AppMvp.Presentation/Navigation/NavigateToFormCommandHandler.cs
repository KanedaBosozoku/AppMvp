using AppMvp.ApplicationCore.Commands;
using AppMvp.Presentation.Abstractions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppMvp.Presentation.Navigation
{
    public sealed class NavigateToFormCommandHandler
        : IRequestHandler<NavigateToFormCommand>
    {
        private readonly IFormNavigator _navigator;

        public NavigateToFormCommandHandler(IFormNavigator navigator)
        {
            _navigator = navigator;
        }

        public Task Handle(NavigateToFormCommand cmd, CancellationToken token)
        {
            _navigator.NavigateTo(cmd.FormType, cmd.Parameter, cmd.Modal, cmd.Callback);
            return Task.CompletedTask;
        }
    }
}