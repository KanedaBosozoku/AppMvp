using AppMvp.Application.Events;
using AppMvp.Presentation.Abstractions;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace AppMvp.UI.EventHandlers
{
    // Handles NavigationRequested notifications by delegating to the region navigator on the UI thread.
    public sealed class NavigationRequestedHandler : INotificationHandler<NavigationRequested>
    {
        private readonly AppMvp.Presentation.Abstractions.IUiDispatcher _ui;
        private readonly AppMvp.Presentation.Abstractions.IRegionNavigator _navigator;

        public NavigationRequestedHandler(AppMvp.Presentation.Abstractions.IUiDispatcher ui, AppMvp.Presentation.Abstractions.IRegionNavigator navigator)
        {
            _ui = ui;
            _navigator = navigator;
        }

        public Task Handle(NavigationRequested notification, CancellationToken cancellationToken)
        {
            // Schedule navigation on the UI thread; do not block the publisher.
            _ui.BeginInvoke(() => _navigator.NavigateToRegionAsync(notification.RegionName, notification.ViewKey, notification.Parameter, cancellationToken));
            return Task.CompletedTask;
        }
    }
}
