using AppMvp.ApplicationCore.Events;
using AppMvp.Presentation.ViewModels;
using AppMvp.Presentation.Abstractions;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace AppMvp.UI.EventHandlers
{
    // UI-level handler for PeopleLoadedEvent.  The application layer publishes the
    // PeopleLoadedEvent with DTOs; this handler runs in the UI project so it can
    // marshal to the UI thread and update ViewModel/controls safely. This keeps
    // presentation and UI threading concerns out of the application layer.
    public sealed class PeopleLoadedEventHandlerUi : INotificationHandler<PeopleLoadedEvent>
    {
        private readonly PeopleViewModel _vm;
        private readonly IUiDispatcher _ui;

        public PeopleLoadedEventHandlerUi(PeopleViewModel vm, IUiDispatcher ui)
        {
            _vm = vm;
            _ui = ui;
        }

        public Task Handle(PeopleLoadedEvent notification, CancellationToken cancellationToken)
        {
            // Marshal the viewmodel refresh to the UI thread
            _ui.BeginInvoke(async () => await _vm.LoadPeopleAsync(CancellationToken.None));
            return Task.CompletedTask;
        }
    }
}
