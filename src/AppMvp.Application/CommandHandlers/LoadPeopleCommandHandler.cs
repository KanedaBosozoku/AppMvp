using AppMvp.ApplicationCore.Commands;
using AppMvp.ApplicationCore.DTOs;
using AppMvp.ApplicationCore.EventBus;
using AppMvp.ApplicationCore.Events;
using AppMvp.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppMvp.ApplicationCore.CommandHandlers
{
    public class LoadPeopleCommandHandler : IRequestHandler<LoadPeopleCommand, Unit>
    {
        private readonly IApplicationEventBus _eventBus;
        private readonly IPersonRepository _repo;

        public LoadPeopleCommandHandler(IApplicationEventBus eventBus, IPersonRepository repo)
        {
            _eventBus = eventBus;
            _repo = repo;
        }

        public async Task<Unit> Handle(LoadPeopleCommand command, CancellationToken ct)
        {
            var people = await _repo.GetAllAsync(ct);
            var dtos = people.Select(p => new PersonDto(p.Id, p.Name)).ToList().AsReadOnly();
            await _eventBus.PublishAsync(new PeopleLoadedEvent(dtos));

            return Unit.Value;
        }
    }
}
