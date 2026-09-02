using AppMvp.ApplicationCore.Events;
using AppMvp.Presentation.ViewModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppMvp.Presentation.EventHandlers
{
    public sealed class PeopleLoadedEventHandler : INotificationHandler<PeopleLoadedEvent>
    {
        private readonly PeopleViewModel _vm;

        public PeopleLoadedEventHandler(PeopleViewModel vm)
        {
            _vm = vm;
        }

        public Task Handle(PeopleLoadedEvent evt, CancellationToken ct)
        {
            //_vm.LoadPeople(evt.People);
            return Task.CompletedTask;
        }
    }
}
