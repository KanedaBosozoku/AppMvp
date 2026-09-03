using AppMvp.ApplicationCore.Commands;
using AppMvp.ApplicationCore.CommandBus;
using AppMvp.Presentation.Abstractions;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AppMvp.UI.EventHandlers
{
    public class ShowEditPersonRequestHandler : INotificationHandler<ShowEditPersonRequest>
    {
        private readonly IFormNavigator _navigator;
        private readonly ICommandBus _commandBus;
        private readonly AppMvp.Presentation.Abstractions.IBusyIndicator _busy;

        public ShowEditPersonRequestHandler(IFormNavigator navigator, ICommandBus commandBus, AppMvp.Presentation.Abstractions.IBusyIndicator busy)
        {
            _navigator = navigator ?? throw new ArgumentNullException(nameof(navigator));
            _commandBus = commandBus ?? throw new ArgumentNullException(nameof(commandBus));
            _busy = busy ?? throw new ArgumentNullException(nameof(busy));
        }

        public Task Handle(ShowEditPersonRequest notification, CancellationToken cancellationToken)
        {
            // Show the PersonEditForm modally; when it closes with OK, trigger a reload of people
            _navigator.NavigateTo(typeof(AppMvp.UI.Forms.PersonEditForm), notification.PersonId, true, result =>
            {
                try
                {
                    if (result is System.Windows.Forms.DialogResult dr && dr == System.Windows.Forms.DialogResult.OK)
                    {
                        // Start a background refresh that uses the busy indicator so the UI shows progress
                        _ = RefreshAfterEditAsync();
                    }
                }
                catch { }
            });

            return Task.CompletedTask;

            async Task RefreshAfterEditAsync()
            {
                using var scope = _busy.Begin("Refreshing people…", "People.Refresh");
                try
                {
                    await _commandBus.SendAsync(new LoadPeopleCommand());
                }
                catch
                {
                    // swallow — LoadPeople failures are handled by application handlers/logging
                }
            }
        }
    }
}
